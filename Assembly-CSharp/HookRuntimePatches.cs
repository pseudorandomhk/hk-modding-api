using HutongGames.PlayMaker;
using Modding.Patches;
using Modding.Utils;
using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using UnityEngine;
using MonoMod.Cil;
using BindingFlags = System.Reflection.BindingFlags;
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
                Multiplier = fsm.FsmVariables.FindFsmFloat("Multiplier"),
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

    // remove "FSM not Preprocessed: {fsm name}" debug message
    private static void PlayMakerFSM_AddEventHandlerComponents_SuppressDebugMessage(ILContext il)
    {
        var c = new ILCursor(il);

        if (c.TryGotoNext(
            instr => instr.MatchCall(typeof(PlayMakerGlobals).GetProperty("IsEditor", BindingFlags.Static | BindingFlags.Public).GetGetMethod()),
            instr => instr.MatchBrtrue(out ILLabel _)))
        {
            c.RemoveRange(8);
        }
    }

    // filter certain messages from unity debug log
    private static void Debug_Log(Action<object> orig, object message)
    {
        if (message is string msg)
        {
            if (msg.StartsWith("AddEventHandlerComponent: "))
                return;
        }
        orig(message);
    }

    private static void HookHelpers()
    {
        new Hook(
            typeof(PlayMakerFSM).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance),
            PlayMakerFSM_Start
        );
        new Hook(
            typeof(Debug).GetMethod(nameof(Debug.Log), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(object) }, null),
            Debug_Log
        );

        new ILHook(
            typeof(PlayMakerFSM).GetMethod("AddEventHandlerComponents", BindingFlags.Public | BindingFlags.Instance),
            PlayMakerFSM_AddEventHandlerComponents_SuppressDebugMessage
        );

        HookUserPatches();

        foreach (var fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
            DoPatchFsm(fsm);
    }

    internal static void Hook() => HookHelpers();
}
