using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSPrototype
{
    /// <summary>
    /// Used to document what code changes I make to certain classes
    /// and functions inside Opsive's Ultimate Character Controller
    /// </summary>
    public class OpsiveUCCRewriteImplementation : MonoBehaviour
    {
        #region Common Properties
        //Transform rootAllyTransform
        //{
        //    get
        //    {
        //        if (_rootAllyTransform == null)
        //        {
        //            _rootAllyTransform = transform.root;
        //        }
        //        return _rootAllyTransform;
        //    }
        //}
        //Transform _rootAllyTransform = null;
        #endregion

        #region UsedCode
        /// <summary>
        /// RTSPrototype-OpsiveUCC-ShootableWeapon: Inside HitscanFire() method, 
        /// after FireDirection has been created, add this code (also insert rootAllyTransform property).
        /// </summary>
        //void OnRTSHitscanFire()
        //{
        //    var fireDirection = FireDirection();
        //    var _force = fireDirection * m_HitscanImpactForce;
        //    rootAllyTransform.SendMessage("CallOnTryHitscanFire", _force, SendMessageOptions.RequireReceiver);
        //}

        /// <summary>
        /// RTSPrototype-OpsiveUCC-MeleeWeapon: Inside UseItem() method, 
        /// after rest of code, add this line (also insert rootAllyTransform property).
        /// </summary>
        //public override void UseItem()
        //{
        //    //...
        //    //...
        //    rootAllyTransform.SendMessage("CallOnTryMeleeAttack", SendMessageOptions.RequireReceiver);
        //}
        #endregion

        #region SimpleBugFixes

        #endregion

        #region OldBugFixes
        //Appears To Be Fixed In UCC Update
        /// <summary>
        /// RTSPrototype-OpsiveUCC-CameraController: After Loop, Where
        /// Each ViewType's Attach Character Method is Called, Call this
        /// method for the m_ViewType field. m_ViewType isn't equal to 
        /// the indexed version in the array for whatever reason.
        /// </summary>
        //protected virtual void InitializeCharacter(GameObject character)
        //{
        //    // Notify the view types of the character that is being attached.
        //    for (int i = 0; i < m_ViewTypes.Length; ++i)
        //    {
        //        m_ViewTypes[i].AttachCharacter(m_Character);
        //    }

        //    if (m_Character != null)
        //    {
        //        //Simple Fix for ViewType Bug
        //        m_ViewType.AttachCharacter(m_Character);
        //        //.....
        //    }
        //}
        #endregion
    }
}