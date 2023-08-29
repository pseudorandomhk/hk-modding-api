using System.Collections;
using System.Threading;
using MonoMod;
using UnityEngine;

// ReSharper disable All
#pragma warning disable 1591, CS0649
// ReSharper disable All
#pragma warning disable 1591, CS0626

namespace Modding.Patches
{
    [MonoModPatch("global::StartManager")]
    public class StartManager : global::StartManager
    {
        [MonoModIgnore]
        private RuntimePlatform platform;

        [MonoModIgnore]
        private extern IEnumerator LoadMainMenu();

        [MonoModIgnore]
        private AsyncOperation loadop;

        private void Start()
        {
            if (this.showProgessIndicator)
            {
                this.progressIndicator.gameObject.SetActive(true);
            }
            else
            {
                this.progressIndicator.gameObject.SetActive(false);
            }
            if (this.platform == RuntimePlatform.WindowsPlayer || this.platform == RuntimePlatform.WindowsEditor || this.platform == RuntimePlatform.LinuxPlayer)
            {
                this.controllerImage.sprite = this.winController;
            }
            else if (this.platform == RuntimePlatform.OSXPlayer || this.platform == RuntimePlatform.OSXEditor)
            {
                this.controllerImage.sprite = this.osxController;
            }
            base.StartCoroutine(this.LoadMainMenu());
            base.StartCoroutine(this.EagerlyActivateMainMenu());
        }

        private IEnumerator EagerlyActivateMainMenu()
        {
            yield return new WaitUntil(() => this.loadop != null);
            this.ControllerNoticeFinished();
        }
    }
}