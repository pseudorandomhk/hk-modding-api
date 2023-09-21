using MonoMod;
using System.Collections;
using UnityEngine;

#pragma warning disable 1591, 0649

namespace Modding.Patches
{
    [MonoModPatch("global::CameraController")]
    public class CameraController : global::CameraController
    {
        [MonoModIgnore]
        [MonoModPublic]
        private void PositionToHero(bool forceDirect) { }

        public void SnapTo(float x, float y)
        {
            this.camTarget.transform.position = new Vector3(x, y, this.camTarget.transform.position.z);
            base.transform.position = new Vector3(x, y, base.transform.position.z);
        }
    }
}