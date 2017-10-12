using UnityEngine;
using System.Collections;

public class MovementSFX : MonoBehaviour {

	Vector3 holder;
	public float amount = 3;
	public float timeScale = 5;
	public float rotateSpeed;
	public bool ShouldRotate;
	public bool RandomRotation;
    public bool randomTimeScale;
	public Vector3 rotationDirection;
	// Use this for initialization
	void Start () 
	{
		if(RandomRotation)
		{
			rotationDirection = Random.insideUnitSphere;
		}
        if(randomTimeScale)
        {
            timeScale += Random.Range(-2, 2);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		holder = transform.position;
		holder.y += Mathf.Sin (Time.time * timeScale) * amount;
		transform.position = holder;
		if(ShouldRotate)
		{
			transform.Rotate(rotateSpeed *rotationDirection);
		}
	}
}
