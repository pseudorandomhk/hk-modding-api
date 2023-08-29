using MonoMod;

// ReSharper disable All
#pragma warning disable 1591, 0108, 0169, 0649, 0414
#pragma warning disable CS0649, CS0626

namespace Modding.Patches
{
    public class EnemyDeathEffects
    {
        private bool didFire;

        public void orig_RecieveDeathEvent(float? attackDirection, bool resetDeathEvent = false, bool spellBurn = false, bool isWatery = false)
        {
            if (didFire) return;
            didFire = !resetDeathEvent;
            RecordKillForJournal();
        }

        //Use this to hook into when an enemy dies. Check EnemyDeathEffects.didFire to prevent doing any actions on redundant invokes.
        public void RecieveDeathEvent(float? attackDirection, bool resetDeathEvent = false, bool spellBurn = false, bool isWatery = false)
        {
            ModHooks.OnRecieveDeathEvent(this, didFire, ref attackDirection, ref resetDeathEvent, ref spellBurn, ref isWatery);
            
            orig_RecieveDeathEvent(attackDirection, resetDeathEvent, spellBurn, isWatery);
        }

        private string playerDataName;

        private void orig_RecordKillForJournal() { }

        private void RecordKillForJournal()
        {
            string boolName = "killed" + this.playerDataName;
            string intName = "kills" + this.playerDataName;
            string boolName2 = "newData" + this.playerDataName;
            
            ModHooks.OnRecordKillForJournal(this, playerDataName, boolName, intName, boolName2);
            
            orig_RecordKillForJournal();
        }

        public EnemyDeathEffects(string playerDataName)
        {
            this.playerDataName = playerDataName;
            this.didFire = false;
        }
    }
}