using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour {

	private Rigidbody rb;
	private KartInput ki;
	public Transform centerOfMass;
	public float speed;
	public float steerAngle;

	void Start () {

		rb = GetComponent<Rigidbody>();
		ki = GetComponent<KartInput>();

		rb.centerOfMass = centerOfMass.localPosition;

	}

	void FixedUpdate () {

		// acceleration
		var throttleForce = speed * ki.ThrottleValue;
		rb.MovePosition(transform.position + transform.forward * throttleForce * Time.deltaTime);

		// steering
		var steerForce = steerAngle * ki.SteerValue;
		rb.transform.Rotate(Vector3.up * steerForce * Time.deltaTime);

	}
}
