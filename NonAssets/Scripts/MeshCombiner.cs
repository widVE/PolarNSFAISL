#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MeshCombiner : MonoBehaviour {

	// Use this for initialization
	public void CombineMeshes () {

         ArrayList materials = new ArrayList();
         ArrayList combineInstanceArrays = new ArrayList();
         MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
 
         foreach (MeshFilter meshFilter in meshFilters)
         {
             MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
 
             if (!meshRenderer ||
                 !meshFilter.sharedMesh ||
                 meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
             {
                 continue;
             }
 
             for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
             {
                 int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
                 if (materialArrayIndex == -1)
                 {
                     materials.Add(meshRenderer.sharedMaterials[s]);
                     materialArrayIndex = materials.Count - 1;
                 }
                 combineInstanceArrays.Add(new ArrayList());
 
                 CombineInstance combineInstance = new CombineInstance();
                 combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                 combineInstance.subMeshIndex = s;
                 combineInstance.mesh = meshFilter.sharedMesh;
                 (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
             }
         }
 
         // Get / Create mesh filter & renderer
         MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
         if (meshFilterCombine == null)
         {
             meshFilterCombine = gameObject.AddComponent<MeshFilter>();
         }
         MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
         if (meshRendererCombine == null)
         {
             meshRendererCombine = gameObject.AddComponent<MeshRenderer>();
         }
 
         // Combine by material index into per-material meshes
         // also, Create CombineInstance array for next step
         Mesh[] meshes = new Mesh[materials.Count];
         CombineInstance[] combineInstances = new CombineInstance[materials.Count];
 
         for (int m = 0; m < materials.Count; m++)
         {
             CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
             meshes[m] = new Mesh();
             meshes[m].CombineMeshes(combineInstanceArray, true, true);
 
             combineInstances[m] = new CombineInstance();
             combineInstances[m].mesh = meshes[m];
             combineInstances[m].subMeshIndex = 0;
         }
 
         // Combine into one
         meshFilterCombine.sharedMesh = new Mesh();
         meshFilterCombine.sharedMesh.name = gameObject.name;
         meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);
 
         // Destroy other meshes
         foreach (Mesh oldMesh in meshes)
         {
             oldMesh.Clear();
             DestroyImmediate(oldMesh);
         }
 
         // Assign materials
         Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
         meshRendererCombine.materials = materialsArray;
 
         /*foreach (MeshFilter meshFilter in meshFilters)
         {
             DestroyImmediate(meshFilter.gameObject);
         }*/
     }
 
     private int Contains(ArrayList searchList, string searchName)
     {
         for (int i = 0; i < searchList.Count; i++)
         {
             if (((Material)searchList[i]).name == searchName)
             {
                 return i;
             }
         }
         return -1;
     }
}
#endif