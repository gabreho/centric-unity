using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

//	float timeCounter = 0.0f;

	public int rotationSpeed = 100;
//	public float Radius = 3.1f;
	// Use this for initialization
	void Start () {
	
	}

	public void ToggleRotation() {
		rotationSpeed = rotationSpeed * -1;
	}
	
	// Update is called once per frame
	void Update () {
	
//		timeCounter += Time.deltaTime;
//		float x = Mathf.Cos(timeCounter);
//		float y = Mathf.Sin(timeCounter);
//		float z = 0.0f;
//
//		transform.position = new Vector3(x,z,y);

		transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
	}
}
