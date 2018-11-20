using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public static class BaseUtilities
    {
        #region Timer
        public class TimerUtility
        {
            float timerRate = 0.0f;
            float currentTimer = 0.0f;

            public TimerUtility(float rate)
            {
                timerRate = rate;
            }

            public void StartTimer()
            {
                currentTimer = Time.time + timerRate;
            }

            public bool IsTimerFinished()
            {
                return Time.time > currentTimer;
            }
        }
        #endregion

        #region Invoker
        public class InvokerUtility
        {
            private static MonoBehaviour invokeCaller = null;

            public static void InvokeInRealTime(ref MonoBehaviour caller, string methodName, float time)
            {
                invokeCaller = caller;
                caller.StartCoroutine(InvokeFromCoroutine(methodName, time));
            }

            static IEnumerator InvokeFromCoroutine(string methodName, float time)
            {
                yield return new WaitForSecondsRealtime(time);
                invokeCaller.Invoke(methodName, 0.0f);
            }
        }
        #endregion

        #region ExtensionMethods
        //public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        //{
        //    if (target == null)
        //        throw new ArgumentNullException(nameof(target));
        //    if (source == null)
        //        throw new ArgumentNullException(nameof(source));
        //    foreach (var element in source)
        //        target.Add(element);
        //}

        public static Dictionary<T, S> AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            //if (collection == null)
            //{
            //    throw new ArgumentNullException("Collection is null");
            //}

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                else
                {
                    // handle duplicate key issue here
                }
            }

            return source;
        }
        #endregion

        #region GetAllTransforms
        public static List<Transform> GetAllTransforms(Transform parent)
        {
            var transformList = new List<Transform>();
            BuildTransformList(transformList, parent);
            return transformList;
        }

        private static void BuildTransformList(ICollection<Transform> transforms, Transform parent)
        {
            if (parent == null) { return; }
            foreach (Transform t in parent)
            {
                transforms.Add(t);
                BuildTransformList(transforms, t);
            }
        }

        #endregion

        #region TextAssetToStringList
        public static List<string> TextAssetToList(TextAsset ta)
        {
            var listToReturn = new List<string>();
            var arrayString = ta.text.Split('\n');
            foreach (var line in arrayString)
            {
                listToReturn.Add(line);
            }
            return listToReturn;
        }

        public static string[] TextAssetToArray(TextAsset ta)
        {
            return TextAssetToList(ta).ToArray();
        }
        #endregion

        #region SaveSerializedObject
#if UNITY_EDITOR
        /// <summary>
        /// Only Useful If Saving Inside the Editor
        /// </summary>
        /// <param name="_object"></param>
        public static void SaveSerializedObject(UnityEngine.Object _object)
        {
            UnityEditor.EditorUtility.SetDirty(_object);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
        #endregion
    }
}