using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour {

	private Rigidbody rb;

	public float speed;

	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	void Update () {
		if (Input.GetButton("AButton")) {
			rb.AddForce(Vector3.forward * speed * Time.deltaTime, ForceMode.Acceleration);
		}		
	}
}
