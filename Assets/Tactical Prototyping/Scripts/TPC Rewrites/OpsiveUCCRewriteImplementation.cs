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

        #endregion

        #region UsedCode

        #endregion

        #region SimpleBugFixes
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