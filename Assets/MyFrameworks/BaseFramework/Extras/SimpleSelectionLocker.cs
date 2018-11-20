using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    /// <summary>
    /// Used To Lock A Selection To A Given GameObject. 
    /// Recommend Using Another Inspector Window That's 
    /// Locked For Convienence.
    /// </summary>
    public class SimpleSelectionLocker : MonoBehaviour
    {
        [Tooltip("Will Only Lock Selection If Inspector Window " +
            "For This MonoBehaviour Is Locked. Recommend Using Another" +
            "Inspector Window That's Locked For Convienence.")]
        [Header("GameObject To Lock")]
        public GameObject mySelection;
    }
}