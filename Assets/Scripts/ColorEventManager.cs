using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorEventManager : MonoBehaviour {

	public int numActiveParticles = 0;
	private List<SpawnParticle> spawners = new List<SpawnParticle>();
	private List<ColorParticleCollision> changedDoms = new List<ColorParticleCollision>();

	void Start() {
		GameObject[] spawnObjs = GameObject.FindGameObjectsWithTag ("Spawner");
		foreach (GameObject curr in spawnObjs) {
			spawners.Add (curr.GetComponent<SpawnParticle> ());
		}
	}
		
	private void resetIntervals() {

		foreach (SpawnParticle curr in spawners) {
			curr.resetTravelInterval ();
		}
	}

	public void addChangedDom(ColorParticleCollision toAdd) {
		changedDoms.Add (toAdd);
	}

	private void resetColors() {
		foreach (ColorParticleCollision curr in changedDoms) {
			curr.reset ();
		}
		changedDoms.Clear ();
	}

	public void resetGame() {
		if (numActiveParticles == 0) {
			resetIntervals ();
			resetColors ();

			// TODO: Make spawners throw particles at random times,
			// not all at once as done here
			//foreach (SpawnParticle curr in spawners) {
			//	curr.startThrowing ();
			//}
		}
	}
}
