using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour {

	private Rigidbody rb;
	private KartInput ki;
	public Transform centerOfMass;
	public float speed;
	public float steerAngle;
	public float angleTolerance = .1f;

	void Start () {

		rb = GetComponent<Rigidbody>();
		ki = GetComponent<KartInput>();

		rb.centerOfMass = centerOfMass.localPosition;

	}

	void FixedUpdate () {

		Vector3 velocity;

		if (ki.ThrottleValue > 0) {
			velocity = transform.forward * speed * ki.ThrottleValue;
			velocity.y = rb.velocity.y;
			rb.velocity = velocity;
		}

		// steering
		var steerForce = steerAngle * ki.SteerValue;
		rb.transform.Rotate(Vector3.up * steerForce * Time.deltaTime);

		// slopes
		Debug.Log((Mathf.PI / 180) * Mathf.Abs(transform.rotation.eulerAngles.x));

	}
}
