using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;

namespace BaseFramework.Extras
{
#if UNITY_EDITOR
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class CombineMeshesUtility : MonoBehaviour
    {
        GameObject highlight;

        #region AddedProps
        MeshRenderer myRenderer
        {
            get
            {
                if(__myRenderer == null)
                    __myRenderer = GetComponent<MeshRenderer>();

                return __myRenderer;
            }
        }
        private MeshRenderer __myRenderer = null;

        MeshFilter myFilter
        {
            get
            {
                if (__myFilter == null)
                    __myFilter = GetComponent<MeshFilter>();

                return __myFilter;
            }
        }
        private MeshFilter __myFilter = null;

        #endregion

        void Start()
        {
            List<MeshFilter> meshFilters = GetComponentsInChildren<MeshFilter>().ToList();
            if (meshFilters.Contains(myFilter))
            {
                meshFilters.Remove(myFilter);
            }
 
            CombineInstance[] combine = new CombineInstance[meshFilters.Count];
            int i = 0;
            while (i < meshFilters.Count)
            {
                Debug.Log(meshFilters[i].gameObject.transform.name);
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                //meshFilters[i].gameObject.active = false;
                i++;
            }

            myFilter.mesh = new Mesh();
            myFilter.mesh.CombineMeshes(combine);
            transform.gameObject.SetActive(true);

            saveMesh();
        }

        void saveMesh()
        {
            Debug.Log("Saving Mesh?");
            Mesh m1 = myFilter.mesh;
            AssetDatabase.CreateAsset(m1, "Assets/Tactical Prototyping/Prefabs/Generated/" + transform.name + ".asset"); // saves to "assets/"
            AssetDatabase.SaveAssets();
        }
    }
#endif
}