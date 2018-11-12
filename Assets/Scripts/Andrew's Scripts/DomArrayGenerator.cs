using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class DomArrayGenerator : MonoBehaviour {

    public GameObject domObject;
    public TextAsset icecubeTextAsset;
    private const float BELOW_ICE = -1950.0f;
	private bool first = true;

    //should keep track of all created objects here in a sort of "dom" matrix..
    //this could allow for very efficient DOM lookups later...
    public const int NUM_STRINGS = 86;
    public const int NUM_DOMS_PER_STRING = 64;

	public GameObject[,] DOMArray = new GameObject[NUM_STRINGS,NUM_DOMS_PER_STRING];

    //line updating variables...
    private bool firstDraw = true;

	void Start () {
		Vector3[] pos = new Vector3[2];

        string[] datasets = icecubeTextAsset.text.Split('\n');

        /*float avgX = 0.0f;
        float avgZ = 0.0f;
        float count = 0.0f;*/
        foreach (string dataset in datasets) {
            // Skip useless data points (and blank last line from String.Split)
            if (dataset == "") continue;
            string[] data = dataset.Split (
                new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            // Ignore malformatted data points
            if (data.Length < 5) continue;

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
            GameObject tableDom = (GameObject)Instantiate(domObject);
			//SetLayersRecursively(tableDom, LayerMask.NameToLayer("TableTop"));

			Vector3 domPos = new Vector3 (xFloat, BELOW_ICE + zFloat, yFloat);

			//table
			tableDom.transform.position = domPos;
			tableDom.transform.SetParent (transform);
            //Debug.Log("Setting scale");
            tableDom.transform.localScale.Set(10.0f, 10.0f, 10.0f);
			tableDom.GetComponent<DOMController>().stringNum = domUnitNum;
			tableDom.GetComponent<DOMController>().domNum = domNum;
            //tableDom.SetActive(false);

            if (domNum <= 60)
            {

				Vector3 startPos = new Vector3 (xFloat, BELOW_ICE + zFloat, yFloat); 
				
                //avgX += xFloat;
                //avgZ += yFloat;
                if(domNum == 0)
                {
                    pos[0] = startPos;
                }


				pos [1] = pos [0];
				first = false;
			}
            else 
            {
                first = true;
            }
				
			DOMArray[domUnitNum, domNum] = tableDom;

		}   //end while()
	}//end Start()

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
