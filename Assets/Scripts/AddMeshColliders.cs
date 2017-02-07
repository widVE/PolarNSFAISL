using UnityEngine;
using System.Collections;

public class AddMeshColliders : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Get all the child transforms recursively
		Transform[] childTransforms = GetComponentsInChildren<Transform> ();

		// Attach mesh colliders to them
		// Clearly nothing can go wrong
		foreach (Transform child in childTransforms) {
			if (child.gameObject.GetComponent<MeshCollider>() == null) {
				child.gameObject.AddComponent<MeshCollider> ();
			}

		}
	}
}
