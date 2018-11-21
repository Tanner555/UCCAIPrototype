using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSPrototype
{
    public class RTSThirdPersonViewType : Opsive.UltimateCharacterController.ThirdPersonController.Camera.ViewTypes.Combat
    {
        bool bHasBeenActivated = false;

        public override void ChangeViewType(bool activate, float pitch, float yaw, Quaternion characterRotation)
        {
            if (activate && bHasBeenActivated == false)
            {
                base.ChangeViewType(activate, pitch, yaw, characterRotation);
            }
            else if(activate == false)
            {
                base.ChangeViewType(activate, pitch, yaw, characterRotation);
            }

            bHasBeenActivated = true;
        }
    }
}