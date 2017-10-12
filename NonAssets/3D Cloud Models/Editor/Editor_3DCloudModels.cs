using UnityEngine;
using UnityEditor;
using System.Collections;

public class Editor_3DCloudModels : EditorWindow {
		[MenuItem ("Window/3D Cloud Models %&q")]
	
	static void Init()
	{
		Editor_3DCloudModels window =  (Editor_3DCloudModels)EditorWindow.GetWindow (typeof (Editor_3DCloudModels));
		window.title = "3D Cloud Models";
		window.maxSize=new Vector2(250,220);
		window.minSize = window.maxSize;
		window.Show();		
	}
	
	//Unneeded for now.
	//List<Material> MatList = new List<Material>();

	Color AmbientColor;
	Color SunColor;
	float Contrast =1, Bias = 0.5f, Growth = 1.0f;
	GameObject LookAtObject;
	float CloudThickness; //z scale
	float CloudThicknessMax; //relative to 2x the x scale.
	bool isCloud=false;

	private void OnInspectorUpdate()
	{
		Repaint();
        
    }
	void OnGUI()
	{
		GameObject[] selectionobjects = Selection.gameObjects;	
		isCloud = CheckforCloud(selectionobjects);
		if(isCloud)
			GetMaterialValues(selectionobjects);
			
			if(GUILayout.Button("Help"))
			{
				EditorUtility.DisplayDialog("How to Use 3D Cloud Models",
					"1. Drag and drop some 3D Cloud Model prefabs into the scene.\n\n" +
					"2. Select the clouds you want to modify. You can adjust the sun and ambient colors using the color swatches.\n\n" +
					"3. Adjust their contrast and density using the sliders.\n\n" +
					"4. You can have them look at an object or the scene camera by clicking on the corresponding button.\n\n" +
					"WARNING: This tool overrides any values you set in the material.\n\nIf you want to make your own cloud textures, the cloud formation goes into the red channel, hard alpha goes into the green channel, soft alpha goes into the blue.","OK");
			}

			GUILayout.Label("Current Selection:");
			AmbientColor = EditorGUILayout.ColorField("Ambient Color: ",AmbientColor);
			SunColor = EditorGUILayout.ColorField("Sun Color: ",SunColor);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Cloud Contrast: ");
			Contrast = GUILayout.HorizontalSlider(Contrast,0.1f,3,null);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Clouds Density: ");
			Bias = GUILayout.HorizontalSlider(Bias,0.25f,0.95f,null);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Cloud Growth: ");
			Growth = GUILayout.HorizontalSlider(Growth,1f,-1f,null);
			EditorGUILayout.EndHorizontal();	
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Cloud Thickness: ");
			CloudThickness = GUILayout.HorizontalSlider(CloudThickness,0.1f,1,null);
			EditorGUILayout.EndHorizontal();
				
			if(selectionobjects.Length!=0 && isCloud)
			{
			for(int x=0;x<selectionobjects.Length;x++)
				{
				float z = (selectionobjects[x].transform.localScale.x * 2) * CloudThickness;
				selectionobjects[x].transform.localScale = new Vector3(selectionobjects[x].transform.localScale.x,selectionobjects[x].transform.localScale.y,z);
				}
			}
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Object to LookAt:");
			LookAtObject = (GameObject)EditorGUILayout.ObjectField(LookAtObject,typeof(GameObject),true,null);
			EditorGUILayout.EndHorizontal();

			if(GUILayout.Button("Look at Object"))
			{
				
				if(selectionobjects.Length==0)
						EditorUtility.DisplayDialog("No Selection is Made","Please select objects (clouds) you want to modify.","OK");
				else if (LookAtObject == null)
						EditorUtility.DisplayDialog("Look At Object is Invalid","Please pick an object to look at in the 'Object to LookAt' box.","OK");
				else if (isCloud)
				for(int x=0;x<selectionobjects.Length;x++)
				{
						selectionobjects[x].transform.LookAt(LookAtObject.transform);	
				}
			}
			
			if(GUILayout.Button("Look at Scene Cam"))
			{			
				if(selectionobjects.Length==0)
						EditorUtility.DisplayDialog("No Selection is Made","Please select objects (clouds) you want to modify.","OK");
			else if(isCloud)
					{
					for(int x=0;x<selectionobjects.Length;x++)
					{					
						selectionobjects[x].transform.LookAt(SceneView.lastActiveSceneView.camera.transform);
					}
				}
			}	
			if(selectionobjects.Length>0 && isCloud)
				UpdateMaterialColors(selectionobjects);
			
		

	}

	
	void UpdateMaterialColors(GameObject[] selection)
	{		
		for(int x=0;x<selection.Length;x++)
			{
			string shaderName = selection[x].GetComponent<Renderer>().sharedMaterial.shader.name;
			if(shaderName.Contains("3DCloudModels")||shaderName.Contains("MetaCloud"))
				{
				selection[x].GetComponent<Renderer>().sharedMaterial.SetColor("_AmbientColor",AmbientColor);
				selection[x].GetComponent<Renderer>().sharedMaterial.SetColor("_SunColor",SunColor);
				selection[x].GetComponent<Renderer>().sharedMaterial.SetFloat("_CloudContrast",Contrast);
				selection[x].GetComponent<Renderer>().sharedMaterial.SetFloat("_Bias",Bias);
				selection[x].GetComponent<Renderer>().sharedMaterial.SetFloat("_CloudGrowth",Growth);
				}			
			}
	}
	
	
	
	void GetMaterialValues(GameObject[] selection)
	{
		if(selection.Length>0)
		{
		SunColor = selection[0].GetComponent<Renderer>().sharedMaterial.GetColor("_SunColor");
		AmbientColor = selection[0].GetComponent<Renderer>().sharedMaterial.GetColor("_AmbientColor");
		Contrast = selection[0].GetComponent<Renderer>().sharedMaterial.GetFloat("_CloudContrast");
		Bias = selection[0].GetComponent<Renderer>().sharedMaterial.GetFloat("_Bias");
		Growth = selection[0].GetComponent<Renderer>().sharedMaterial.GetFloat("_CloudGrowth");
					}
		
	}
	
	private bool CheckforCloud(GameObject[] selection)
	{
		bool isCloud=true;
		string shaderName="";
		foreach (GameObject g in selection)
		{
			if(g.GetComponent<MeshRenderer>())			
				 shaderName= g.GetComponent<Renderer>().sharedMaterial.shader.name;
	
			if(shaderName.Contains("3DCloudModel")||shaderName.Contains("MetaCloud"))			
				isCloud=true;
			else
			{
				isCloud=false;
				break;
			}
		}

		if(selection.Length==0)
			isCloud=false;
		return isCloud;
	}
	
}
