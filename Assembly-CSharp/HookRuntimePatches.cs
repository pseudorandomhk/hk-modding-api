using HutongGames.PlayMaker;
using Modding.Patches;
using Modding.Utils;
using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;

namespace Modding;

/// <summary>
/// Helper class to inject runtime patches
/// </summary>
public static class HookRuntimePatches
{
    private static void InjectTakeDamage(PlayMakerFSM fsm)
    {
        if (fsm.FsmName != "damages_enemy") return;

        foreach (FsmState state in fsm.FsmStates)
        {
            if (state.Name is not "Send Event" or "Parent" or "Grandparent") continue;

            TakeDamage inject = new TakeDamage()
            {
                Target = null,  // not needed for ModHooks
                AttackType = fsm.FsmVariables.FindFsmInt("attackType"),
                CircleDirection = fsm.FsmVariables.FindFsmBool("circleDirection"),
                DamageDealt = fsm.FsmVariables.FindFsmInt("damageDealt"),
                Direction = fsm.FsmVariables.FindFsmFloat("direction"),
                IgnoreInvulnerable = fsm.FsmVariables.FindFsmBool("Ignore Invuln"),
                MagnitudeMultiplier = fsm.FsmVariables.FindFsmFloat("magnitudeMult"),
                MoveAngle = fsm.FsmVariables.FindFsmFloat("Move Angle"),
                MoveDirection = fsm.FsmVariables.FindFsmBool("moveDirection"),
                Multiplier = fsm.FsmVariables.FindFsmFloat("multiplier"),
                SpecialType = fsm.FsmVariables.FindFsmInt("Special Type")
            };
            state.AppendAction(inject);
        }
    }

    private static void InjectEnemyDeathEffects(PlayMakerFSM fsm)
    {
        if (fsm.FsmName != "death_control") return;

        FsmState injectionTarget = fsm.FsmStates.FirstOrDefault(s => s.Name == "Journal Entry?");
        if (injectionTarget == null) return;

        injectionTarget.InsertAction(0, new FsmLambda(() =>
        {
            (new EnemyDeathEffects(fsm.FsmVariables.FindFsmString("PlayerData Name").Value))
            .RecieveDeathEvent(fsm.FsmVariables.FindFsmFloat("Attack Direction")?.Value, false,
                               fsm.FsmVariables.FindFsmBool("spellBurn").Value, fsm.FsmVariables.FindFsmBool("Water").Value);
        }));
    }

    /// <summary>
    /// Delegate for patching FSMs
    /// </summary>
    /// <param name="fsm">FSM to patch</param>
    public delegate void PatchFsm(PlayMakerFSM fsm);
    /// <summary>
    /// Called for every loaded FSM
    /// </summary>
    public static event PatchFsm PatchFsms;
    private static void DoPatchFsm(PlayMakerFSM fsm) => PatchFsms?.Invoke(fsm);

    private static void HookUserPatches()
    {
        PatchFsms += InjectTakeDamage;
        PatchFsms += InjectEnemyDeathEffects;
    }

    private static void PlayMakerFSM_Start(Action<PlayMakerFSM> orig, PlayMakerFSM self)
    {
        orig(self);
        DoPatchFsm(self);
    }

    private static void HookHelpers()
    {
        IDetour fsmHook = new Hook(
            ReflectionHelper.GetMethodInfo(typeof(PlayMakerFSM), "Start", true),
            ReflectionHelper.GetMethodInfo(typeof(HookRuntimePatches), nameof(PlayMakerFSM_Start), false)
        );

        HookUserPatches();

        foreach (var fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
            DoPatchFsm(fsm);
    }

    internal static void Hook() => HookHelpers();
}
