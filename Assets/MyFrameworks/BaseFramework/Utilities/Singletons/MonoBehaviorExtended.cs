using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviorExtended : MonoBehaviour {
    //Not Implemented, Look at Commented Code
}

#region Commented Code
//static public class MethodExtensionForMonoBehaviourTransform
//{
//    /// <summary>
//    /// Gets or add a component. Usage example:
//    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
//    /// </summary>
//    static public T GetOrAddComponent<T>(this Component child) where T : Component
//    {
//        T result = child.GetComponent<T>();
//        if (result == null)
//        {
//            result = child.gameObject.AddComponent<T>();
//        }
//        return result;
//    }
//}
#endregion