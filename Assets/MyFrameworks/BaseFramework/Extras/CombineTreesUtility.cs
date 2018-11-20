using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class CombineTreesUtility : MonoBehaviour {
#if UNITY_EDITOR
    //Array with trees we are going to combine
    public GameObject[] treesArray;
    //The object that is going to hold the combined mesh
    public GameObject combinedObj;

    private string LeafsName = "leafs";
    private string TrunkName = "trunk";

    private Material LeafMaterial = null;
    private Material TrunkMaterial = null;

    void Start()
    {
        CombineTrees();
        //Prepare To Save Assets
        saveMesh();
        savePrefab();
        //Try Adding Materials
        TryAddingMaterials();
        //Save Assets
        AssetDatabase.SaveAssets();
    }

    void savePrefab()
    {
        Debug.Log("Saving Prefab?");
        PrefabUtility.CreatePrefab("Assets/Tactical Prototyping/Prefabs/Generated/" + combinedObj.transform.name + ".prefab", combinedObj);
    }

    void saveMesh()
    {
        Debug.Log("Saving Mesh?");
        AssetDatabase.CreateAsset(combinedObj.GetComponent<MeshFilter>().mesh, "Assets/Tactical Prototyping/Prefabs/Generated/" + combinedObj.transform.name + " mesh" + ".asset");
    }

    void TryAddingMaterials()
    {
        if(LeafMaterial != null && TrunkMaterial != null)
        {
            Debug.Log($"Attempt Adding Materials");
            var _renderer = combinedObj.GetComponent<MeshRenderer>();
            _renderer.materials = new Material[]
            {
                LeafMaterial, TrunkMaterial
            };
        }
    }

    //Similar to Unity's reference, but with different materials
    //http://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
    void CombineTrees()
    {
        //Lists that holds mesh data that belongs to each submesh
        List<CombineInstance> woodList = new List<CombineInstance>();
        List<CombineInstance> leafList = new List<CombineInstance>();

        //Loop through the array with trees
        for (int i = 0; i < treesArray.Length; i++)
        {
            Debug.Log($"Processing Tree {treesArray[i].name}");
            GameObject currentTree = treesArray[i];

            //Deactivate the tree 
            currentTree.SetActive(false);

            //Get all meshfilters from this tree, true to also find deactivated children
            MeshFilter[] meshFilters = currentTree.GetComponentsInChildren<MeshFilter>(true);

            //Loop through all children
            for (int j = 0; j < meshFilters.Length; j++)
            {
                Debug.Log($"Processing MeshFilter: {meshFilters[j].name}");
                MeshFilter meshFilter = meshFilters[j];

                CombineInstance combine = new CombineInstance();

                //Is it wood or leaf?
                MeshRenderer meshRender = meshFilter.GetComponent<MeshRenderer>();

                //Modify the material name, because Unity adds (Instance) to the end of the name
                string materialName = meshRender.material.name.Replace(" (Instance)", "");

                if (materialName.ToLower().Contains(LeafsName))
                {
                    Debug.Log($"Adding Leaf Model To Leaf List");
                    combine.mesh = meshFilter.mesh;
                    combine.transform = meshFilter.transform.localToWorldMatrix;

                    //Add it to the list of leaf mesh data
                    leafList.Add(combine);
                    LeafMaterial = meshRender.material;
                }
                else if (materialName.ToLower().Contains(TrunkName))
                {
                    Debug.Log($"Adding Trunk Model To Wood List");
                    combine.mesh = meshFilter.mesh;
                    combine.transform = meshFilter.transform.localToWorldMatrix;

                    //Add it to the list of wood mesh data
                    woodList.Add(combine);
                    TrunkMaterial = meshRender.material;
                }
            }
        }


        //First we need to combine the wood into one mesh and then the leaf into one mesh
        Mesh combinedWoodMesh = new Mesh();
        combinedWoodMesh.CombineMeshes(woodList.ToArray());

        Mesh combinedLeafMesh = new Mesh();
        combinedLeafMesh.CombineMeshes(leafList.ToArray());

        //Create the array that will form the combined mesh
        CombineInstance[] totalMesh = new CombineInstance[2];

        //Add the submeshes in the same order as the material is set in the combined mesh
        totalMesh[0].mesh = combinedLeafMesh;
        totalMesh[0].transform = combinedObj.transform.localToWorldMatrix;
        totalMesh[1].mesh = combinedWoodMesh;
        totalMesh[1].transform = combinedObj.transform.localToWorldMatrix;

        //Create the final combined mesh
        Mesh combinedAllMesh = new Mesh();

        //Make sure it's set to false to get 2 separate meshes
        combinedAllMesh.CombineMeshes(totalMesh, false);
        combinedObj.GetComponent<MeshFilter>().mesh = combinedAllMesh;
    }
#endif
}
