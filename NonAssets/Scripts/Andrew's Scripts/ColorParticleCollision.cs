using UnityEngine;
using System.Collections;



public class ColorParticleCollision : MonoBehaviour {

	[SerializeField]
	private Vector3 defaultScale;

	private int minIndex = 10;
	private float mindistance = 100f;
	private Color[] colors = new Color[7];
	private Color defaultColor;
	[SerializeField]
	private GameObject sphere;
	//[SerializeField]
	//private bool historicColor = false;
	//private int minIndex = 20;
	private ColorEventManager colorMan;
    private MeshRenderer domGlobe = null;
    private MeshRenderer domGlobe2 = null;
	private GameObject externalParts;
	//private ParticleTrail trail;

	void Start() {

        externalParts = transform.Find("s1DOM_ExternalParts").gameObject;
        if (externalParts != null)
        {
            domGlobe = externalParts.GetComponent<MeshRenderer>();
            //r.materials[0].shader = Shader.Find("Particles/Additive");
            //need to change this on all LOD levels as well too...
            GameObject shell = externalParts.transform.Find("Outer").gameObject;
            if (shell != null)
            {
                domGlobe2 = shell.GetComponent<MeshRenderer>();
                //r2.materials[2].shader = Shader.Find("Particles/Additive");
            }
		} else {
			Debug.Log ("External Parts Null");
		}

		// Set up the pretty colors - lower index = closer to dom
		colors[0] = Color.red;
		colors[1] = new Color(1.0f, 0.5f, 0.0f, 1.0f);
		colors[2] = Color.yellow;
		colors[3] = Color.green;
		colors[4] = Color.blue;
		colors[5] = Color.magenta;
		colors[6] = new Color(0.5f, 0.0f, 1.0f, 1.0f);

		//sphere = this.transform.Find ("Sphere");
		defaultColor = sphere.GetComponent<MeshRenderer> ().material.color;

		colorMan = this.transform.parent.GetComponent<ColorEventManager>();
		//trail = this.transform.parent.GetComponent<ParticleTrail> ();
        if (colorMan == null)
        {
            Debug.LogError("Couldn't get the ColorEventManager script from the object");
        }
        

	}

	// Scale the DOM based on the closest distance the particle has been to this DOM
	void OnTriggerStay(Collider other) {
        //Debug.Log("In trigger stay!");
		if (other.gameObject.tag != ("Particle")) {
			return;
		}
		float currDistance = Vector3.Distance (other.transform.position, this.transform.position);
		if (currDistance < mindistance) {
			mindistance = currDistance;
			this.transform.localScale = getScale(currDistance);
		}
	}

	void OnTriggerEnter(Collider other) {
        //Debug.Log("In trigger enter!");
        if (colorMan)
        {
            
            if (other.gameObject.tag.Equals("Particle"))
            {
                // Add the point to the trail list
				//trail.addPoint(this.transform.position);
               
			}

            colorMan.addChangedDom(this);

            //Debug.Log ("Collided with particle!");
            // Collided with the particle, get the time interval from initial impact
            int index = 0;

            SpawnParticle p = other.transform.parent.GetComponent<SpawnParticle>();
            if (p != null)
            {
                float interval = p.getTravelInterval();

                // Turn that interval into a meaningful color....somehow....
                // I'll estimate that it takes ~2 seconds for a particle to traverse the dom array
                // so each 1/3 second will be associated with a different color

                index = ClampToIndex(interval);
                if (index > 6)
                {
                    index = 6;
                }
                if (index < minIndex)
                {
                    minIndex = index;
                }
                else
                {
                    return;
                }

                // Apply the color
                sphere.GetComponent<MeshRenderer>().material.color = colors[index];
            }

            if (domGlobe != null)
            {
                domGlobe.materials[0].shader = Shader.Find("Particles/Additive");
                domGlobe.materials[0].SetColor("_TintColor", colors[index]);
            }

            if (domGlobe2 != null)
            {
                domGlobe2.materials[2].shader = Shader.Find("Particles/Additive");
                domGlobe2.materials[2].SetColor("_TintColor", colors[index]);
            }

        }
	}
			

	private int ClampToIndex(float interval) {
		return (int) Mathf.Round (interval * 6f);
	} 

	public void reset() {
		sphere.GetComponent<MeshRenderer> ().material.color = defaultColor;
        if (domGlobe != null)
        {
            domGlobe.materials[0].shader = Shader.Find("Standard");
        }

        if (domGlobe2 != null)
        {
            domGlobe2.materials[2].shader = Shader.Find("Standard");
        }

		this.transform.localScale = defaultScale;
		mindistance = 100f;
		minIndex = 10;
	}

	private Vector3 getScale(float currDistance) {
		float coeff = 5 - (currDistance);

		return (new Vector3(0.1f, 0.1f, 0.1f) * coeff);
	}










	/// <summary>
	/// Below is the old implementation of color - based on distance
	/// </summary>
	/// <param name="other">Other.</param>
	/*void OnTriggerStay(Collider other) {
		if (!other.gameObject.tag.Equals("Particle")) {
			//Debug.LogError ("DOM collided with something not a particle!");
			return;
		}
		//Debug.Log ("Collided with particle!");
		// Collided with the particle, get the distance and scale the dom for now
		float distance = Vector3.Distance(this.transform.position, other.transform.position);
		if (distance != prevdistance) {
			prevdistance = distance;
			int index = ClampToIndex (distance);
            if (index < 0 || index > 6)
            {
                Debug.LogError("Index arithmetic failed, index was: " + index);
            }
            else
            {
				if (historicColor) {
					// check if the new index is lower than the original to update
					if(minIndex > index) {
						sphere.GetComponent<MeshRenderer>().material.color = colors[index];
						minIndex = index;
					}

				} else {
					// Else always update to the new value
					sphere.GetComponent<MeshRenderer>().material.color = colors[index];
				}
                
            }
		}

	}

	void OnTriggerExit(Collider other) {
		if (!other.gameObject.tag.Equals("Particle")) {
			//Debug.LogError ("DOM exited collider of something not a particle!");
			return;
		}
		if (!historicColor) {
			sphere.GetComponent<MeshRenderer> ().material.color = defaultColor;
		}

		//Debug.Log ("Particle exited nicely");
	}

	private int ClampToIndex(float distance) {
		return (int) Mathf.Round (distance/2f);
	} */
}
