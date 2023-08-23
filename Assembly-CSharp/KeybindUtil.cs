using Shims.NET.System;
using InControl;

namespace Modding
{
    /// <summary>
    /// Utils for interacting with InControl keybindings.
    /// </summary>
    public static class KeybindUtil
    {
        /// <summary>
        /// Gets a <c>Key</c> from a player action.
        /// </summary>
        /// <param name="action">The player action</param>
        /// <returns></returns>
        public static Key GetKeyBinding(this PlayerAction action)
        {
            foreach (BindingSource src in action.Bindings)
            {
                Key ret = src switch
                {
                    KeyBindingSource { Control.IncludeCount: 1 } kbs => kbs.Control.GetInclude(0),
                    _ => default
                };
                
                if (ret != Key.None)
                {
                    return ret;
                }
            }
            
            return default;
        }

        /// <summary>
        /// Adds a binding to the player action based on a <c>KeyOrMouseBinding</c>.
        /// </summary>
        /// <param name="action">The player action</param>
        /// <param name="binding">The binding</param>
        public static void AddKeyOrMouseBinding(this PlayerAction action, Key binding)
        {
            if (binding != Key.None)
            {
                action.AddBinding(new KeyBindingSource(new KeyCombo(binding)));
            }
        }

        /// <summary>
        /// Parses a key or mouse binding from a string.
        /// </summary>
        /// <param name="src">The source string</param>
        /// <returns></returns>
        public static Key? ParseBinding(string src)
        {
            if (Enum.TryParse<Key>(src, out var key))
            {
                return key;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a controller button binding for a player action.
        /// </summary>
        /// <param name="ac">The player action.</param>
        /// <returns></returns>
        public static InputControlType GetControllerButtonBinding(this PlayerAction ac)
        {
            foreach (var src in ac.Bindings)
            {
                if (src is DeviceBindingSource dsrc)
                {
                    return dsrc.Control;
                }
            }
            return InputControlType.None;
        }

        
        /// <summary>
        /// Adds a controller button binding to the player action based on a <c>InputControlType</c>.
        /// </summary>
        /// <param name="action">The player action</param>
        /// <param name="binding">The binding</param>
        public static void AddInputControlType(this PlayerAction action, InputControlType binding)
        {
            if (binding != InputControlType.None)
            {
                action.AddBinding(new DeviceBindingSource(binding));
            }
        }

        /// <summary>
        /// Parses a InputControlType binding from a string.
        /// </summary>
        /// <param name="src">The source string</param>
        /// <returns></returns>
        public static InputControlType? ParseInputControlTypeBinding(string src)
        {
            if (Enum.TryParse<InputControlType>(src, out var key))
            {
                return key;
            }
            else
            {
                return null;
            }
        }
    }
}