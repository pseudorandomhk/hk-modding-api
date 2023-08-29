using InControl;
using MonoMod;
using UnityEngine;

#pragma warning disable 1591
#pragma warning disable CS0649, CS0108

namespace Modding.Patches
{
    [MonoModPatch("global::InputHandler")]
    public class InputHandler : global::InputHandler
    {
        [MonoModIgnore]
        private bool isTitleScreenScene;

        [MonoModIgnore]
        private bool isMenuScene;

        [MonoModIgnore]
        private GameManager gm;

        // Reverted cursor behavior
        [MonoModReplace]
        private void OnGUI()
        {
            Cursor.lockState = CursorLockMode.None;
            if (isTitleScreenScene)
            {
                Cursor.visible = false;
                return;
            }

            if (!isMenuScene)
            {
                ModHooks.OnCursor(gm);
                return;
            }

            Cursor.visible = true;
        }

        public static InputHandler Instance;

        [MonoModIgnore]
        public extern void orig_Awake();

        public void Awake()
        {
            Instance = this;
            orig_Awake();
        }
    }
}