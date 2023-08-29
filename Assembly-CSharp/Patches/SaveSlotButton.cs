using MonoMod;

#pragma warning disable CS1591

namespace Modding.Patches
{
    [MonoModPatch("UnityEngine.UI.SaveSlotButton")]
    public class SaveSlotButton : UnityEngine.UI.SaveSlotButton
    {
        private int SaveSlotIndex
        {
            get
            {
                switch (this.saveSlot)
                {
                    case SaveSlot.SLOT_1: return 1;
                    case SaveSlot.SLOT_2: return 2;
                    case SaveSlot.SLOT_3: return 3;
                    case SaveSlot.SLOT_4: return 4;
                    default: return 0;
                }
            }
        }
    }
}
