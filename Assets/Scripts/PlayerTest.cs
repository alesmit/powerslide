using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour {

	private Rigidbody rb;

	public float speed;
	public float steerSpeed;

	bool isTurningRight;
	bool isTurningLeft;
	bool isAccelerating;

	void Start () {
		rb = GetComponent<Rigidbody>();	
	}

	void FixedUpdate () {
		if (isAccelerating) {
			rb.AddRelativeForce(Vector3.forward * speed * Time.deltaTime, ForceMode.Acceleration);
		}

		if (isTurningRight) {
			rb.AddRelativeTorque(Vector3.up * steerSpeed * Time.deltaTime);
		}

		if (isTurningLeft) {
			rb.AddRelativeTorque(-Vector3.up * steerSpeed * Time.deltaTime);
		}
	}
	
	void Update () {
		isTurningRight = Input.GetButton("DPadR");
		isTurningLeft = Input.GetButton("DPadL");
		isAccelerating = Input.GetButton("AButton");
	}
}
