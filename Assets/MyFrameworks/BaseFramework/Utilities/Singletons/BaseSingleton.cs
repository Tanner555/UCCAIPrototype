using System.Collections;
using UnityEngine;

namespace BaseFramework
{
    #region BaseSingleton
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class BaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _thisInstance;

        private static object _lock = new object();

        protected static bool bHasInstance = false;
        protected static bool bIsBeingDestroyed = false;

        public static T thisInstance
        {
            get
            {
                lock (_lock)
                {
                    if (_thisInstance == null)
                    {
                        var _foundObject = (T)FindObjectOfType(typeof(T));
                        if(_foundObject == null)
                        {
                            return _thisInstance;
                        }
                        //Only Set _thisInstance If Found Object Exists
                        //This Fixes Unsubbing Issues When New Level Is Being Loaded
                        _thisInstance = _foundObject;
                        bHasInstance = true;
                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Please Delete The Duplicate "+typeof(T)+" from the scene.");
                        }

                    }

                    return _thisInstance;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            bHasInstance = false;
        }
    }
    #endregion

    #region DontDestroyOnLoadBaseSingleton
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class DontDestroyOnLoadBaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _thisInstance;

        private static object _lock = new object();

        protected static bool bHasInstance = false;

        public static T thisInstance
        {
            get
            {
                lock (_lock)
                {
                    if (_thisInstance == null)
                    {
                        _thisInstance = (T)FindObjectOfType(typeof(T));
                        bHasInstance = true;
                        DontDestroyOnLoad(_thisInstance);
                    }

                    return _thisInstance;
                }
            }
        }
    }
    #endregion

    #region Commented Code
    //If Statement After "if ( FindObjectsOfType(typeof(T)).Length > 1 )" Ends
    //if (_thisInstance == null)
    //{

    //    GameObject singleton = new GameObject();
    //    _thisInstance = singleton.AddComponent<T>();
    //    singleton.name = "(singleton) " + typeof(T).ToString();

    //    DontDestroyOnLoad(singleton);

    //    Debug.Log("[Singleton] An instance of " + typeof(T) +
    //        " is needed in the scene, so '" + singleton +
    //        "' was created with DontDestroyOnLoad.");
    //}
    //else
    //{
    //    Debug.Log("[Singleton] Using instance already created: " +
    //        _thisInstance.gameObject.name);
    //}
    #endregion
}