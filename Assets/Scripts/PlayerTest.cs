using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour {

	private Rigidbody rb;

	public float speed;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Throttle")) {
			Debug.Log("cazzo");

			rb.AddForce(Vector3.forward * speed * Time.deltaTime, ForceMode.Acceleration);
		}
		
	}
}
