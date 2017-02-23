using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class DomData : MonoBehaviour {

    public GameObject domObject;
    string icecubeFile = "Assets\\IceCubeData\\geometry\\Icecube_Geometry_Data.txt";
    private const float BELOW_ICE = -1950.0f;
	private bool first = true;
    
    //should keep track of all created objects here in a sort of "dom" matrix..
    //this could allow for very efficient DOM lookups later...
    private const int NUM_STRINGS = 86;
    private const int NUM_DOMS_PER_STRING = 64;
    public GameObject[,] DOMArray = new GameObject[NUM_STRINGS,NUM_DOMS_PER_STRING];

	void Start () {

		StreamReader reader = new StreamReader (icecubeFile);

		LineRenderer lineRen = null;
		Vector3[] pos = new Vector3[2];

		string line;
        float avgX = 0.0f;
        float avgZ = 0.0f;
        float count = 0.0f;
		while ((line = reader.ReadLine ()) != null) {

			string[] data = line.Split (new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

			//grab each column of data
			string stringIndex = data [0];
			string domIndex = data [1];
			string xVal = data [2];
			string yVal = data [3];
			string zVal = data [4];

			//parsing each piece of data
            int domUnitNum = Convert.ToInt32(stringIndex)-1;
            int domNum = Convert.ToInt32(domIndex)-1;
			float xFloat = float.Parse (xVal);
			float yFloat = float.Parse (yVal);
			float zFloat = float.Parse (zVal);

			//create DOMS
            GameObject dom = (GameObject)Instantiate(domObject); //GameObject.CreatePrimitive(PrimitiveType.Sphere);
			Vector3 domPos = new Vector3 (xFloat, BELOW_ICE + zFloat, yFloat);
			dom.transform.position = domPos;
			dom.transform.SetParent (transform);

            if (domNum <= 60)
            {
                count+=1.0f;
				Vector3 startPos = new Vector3 (xFloat, BELOW_ICE + zFloat, yFloat); 
				pos [0] = startPos;
                avgX += xFloat;
                avgZ += yFloat;
				if (first == false) {

					lineRen = dom.AddComponent<LineRenderer> ();
					lineRen.SetWidth (.023f, .023f);
					lineRen.SetColors (Color.black, Color.black);
					lineRen.material = new Material (Shader.Find ("Standard"));//Particles/Additive"));
					lineRen.material.color = Color.black;
					lineRen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					lineRen.receiveShadows = false;

					Vector3[] pos2 = new Vector3[2];
					pos2 [0] = pos [0];
					pos2 [1] = pos [1];

					pos2 [0].y += 0.64f;
					pos2 [1].y -= 0.62f;
					lineRen.SetPositions (pos2);
			
				}
                else 
                {
                    lineRen = dom.AddComponent<LineRenderer>();
                    lineRen.SetWidth(.023f, .023f);
                    lineRen.SetColors(Color.black, Color.black);
                    lineRen.material = new Material(Shader.Find("Standard"));//Particles/Additive"));
                    lineRen.material.color = Color.black;
                    lineRen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    lineRen.receiveShadows = false;

                    Vector3[] pos2 = new Vector3[2];
                    pos2[0] = new Vector3(xFloat, 0.0f, yFloat);
                    pos2[1] = pos[0];

                    pos2[1].y += 0.64f;
                    //pos2[1].y -= 0.62f;
                    lineRen.SetPositions(pos2);
			
                }

				pos [1] = pos [0];
				first = false;
			}
            else 
            {
                first = true;
            }

            DOMArray[domUnitNum, domNum] = dom;
		}//end while()

        //Debug.Log("Average X: " + avgX / count);
        //Debug.Log("Average Z: " + avgZ / count);
		reader.Close ();
	}//end Start()

	void Update () {
		//TODO ?
	}//end Update()
}//end Class
