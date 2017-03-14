using UnityEngine;
using System.Collections;

public class ChooseSpawner : MonoBehaviour {

	[SerializeField]
	private int seed;

	private GameObject[] spawners;
	// Use this for initialization
	void Start () {
		spawners = GameObject.FindGameObjectsWithTag ("Spawner");
		if (spawners == null) {
			Debug.LogError ("Spawners couldn't be found in ChooseSpawner");
		}

		Random.InitState (seed * (int)System.DateTime.Now.Ticks);
		int index = Random.Range (0, spawners.Length);
		Debug.Log ("Spawner chosen was " + spawners [index].name);
		spawners [index].GetComponent<SpawnParticle> ().enabled = true;
	}

}
