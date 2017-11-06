using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor {

    void OnSceneGUI()
    {
        MeshCombiner mc = target as MeshCombiner;
        Vector3 pos = new Vector3();
        pos.Set(0f, 0f, 0f);
        if(Handles.Button(pos, Quaternion.LookRotation(Vector3.up), 1, 1, Handles.CylinderCap))
        {
            mc.CombineMeshes();
        }
    }
}
