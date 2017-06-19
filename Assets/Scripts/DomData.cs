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

	public struct DomStruct
	{
		public GameObject puzzleDom;
		public GameObject tableDom;
	};

	public DomStruct[,] DOMArray = new DomStruct[NUM_STRINGS,NUM_DOMS_PER_STRING];

    //line updating variables...
    private bool firstDraw = true;

	void Start () {

		StreamReader reader = new StreamReader (icecubeFile);

		LineRenderer lineRen = null;
		Vector3[] pos = new Vector3[2];

		string line;
        /*float avgX = 0.0f;
        float avgZ = 0.0f;
        float count = 0.0f;*/
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
            GameObject tableDom = (GameObject)Instantiate(domObject); //GameObject.CreatePrimitive(PrimitiveType.Sphere);
			SetLayersRecursively(tableDom, LayerMask.NameToLayer("TableTop"));
			GameObject puzzleDom = (GameObject)Instantiate(domObject);
			SetLayersRecursively(puzzleDom, LayerMask.NameToLayer("Puzzle"));

			Vector3 domPos = new Vector3 (xFloat, BELOW_ICE + zFloat, yFloat);

			//table
			tableDom.transform.position = domPos;
			tableDom.transform.SetParent (transform);
			tableDom.GetComponent<DOMController>().stringNum = domUnitNum;
			tableDom.GetComponent<DOMController>().domNum = domNum;

			//puzzle
			puzzleDom.transform.position = domPos + new Vector3(3000, 0, 0);
			puzzleDom.transform.SetParent (transform);
			puzzleDom.GetComponent<DOMController>().stringNum = domUnitNum;
			puzzleDom.GetComponent<DOMController>().domNum = domNum;



            if (domNum <= 60)
            {

				Vector3 startPos = new Vector3 (xFloat, BELOW_ICE + zFloat, yFloat); 
				
                //avgX += xFloat;
                //avgZ += yFloat;
                if(domNum == 0)
                {
                    pos[0] = startPos;
                }

				if (first == false) {
					/*lineRen = dom.AddComponent<LineRenderer> ();
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
					lineRen.SetPositions (pos2);*/
				}
                else 
                {
                   /* lineRen = dom.AddComponent<LineRenderer>();
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
                    lineRen.SetPositions(pos2);*/
                }

               /* if (domNum == 59)
                {
                    //how to make line widths different for oculus vs. regular user?
                    pos[1] = new Vector3(xFloat, BELOW_ICE + zFloat, yFloat);
                    lineRen = dom.AddComponent<LineRenderer>();
                    lineRen.SetWidth(.023f, .023f);
                    lineRen.SetColors(Color.black, Color.black);
                    lineRen.material = new Material(Shader.Find("Standard"));//Particles/Additive"));
                    lineRen.material.color = Color.black;
                    lineRen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    lineRen.receiveShadows = false;

                    //Vector3[] pos2 = new Vector3[2];
                    //pos2[0] = new Vector3(xFloat, 0.0f, yFloat);
                    //pos2[1] = pos[0];

                    //pos2[1].y += 0.64f;
                    //pos2[1].y -= 0.62f;
                    lineRen.SetPositions(pos);
                }*/
				pos [1] = pos [0];
				first = false;
			}
            else 
            {
                first = true;
            }

			DomStruct pair = new DomStruct();
			pair.tableDom = tableDom;
			pair.puzzleDom = puzzleDom;
            DOMArray[domUnitNum, domNum] = pair;

		}   //end while()

		reader.Close ();
	}//end Start()

	void Update () {
		
        /*if (firstDraw || UnityEngine.Camera.main.velocity.magnitude > 0.0f)
        {
            firstDraw = false;
            for (int i = 0; i < transform.childCount; ++i)
            {
                DOMController d = transform.GetChild(i).gameObject.GetComponent<DOMController>();
                if(d.lineRen != null)
                {
                    float w = Vector3.Distance(UnityEngine.Camera.main.transform.position, d.transform.position) * 0.001f;
                    d.lineRen.startWidth = w;
                    d.lineRen.endWidth = w;
                }
            }
        }*/
	}//end Update()

	private void SetLayersRecursively(GameObject obj, int layer) {
		if (obj == null) {
			return;
		}

		obj.layer = layer;
		foreach (Transform child in obj.transform) {
			if (child == null) {
				continue;
			}
			SetLayersRecursively (child.gameObject, layer);
		}
	}
}//end Class
