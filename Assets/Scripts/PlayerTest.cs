using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour {

	private Rigidbody rb;
	public Transform centerOfMass;
	public float speed;
	public float steerSpeed;
	bool isTurningRight;
	bool isTurningLeft;
	bool isAccelerating;
	public float turningFactor = 1f;
	float horizontalAxisValue = 0;
	float throttleValue = 0;
	public float maxThrottleValue = 100f;
	public float throttleFactor = 10;

	void Start () {
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = centerOfMass.localPosition;
	}

	void FixedUpdate () {

		// acceleration
		rb.MovePosition(transform.position + transform.forward * speed * throttleValue * Time.deltaTime);
		

		// steering
		rb.transform.Rotate(Vector3.up * horizontalAxisValue * steerSpeed * Time.deltaTime);

	}
	
	void Update () {

		isTurningRight = Input.GetButton("DPadR");
		isTurningLeft = Input.GetButton("DPadL");
		isAccelerating = Input.GetButton("AButton");

		if (isTurningLeft && horizontalAxisValue > -1) {
			horizontalAxisValue -= Time.deltaTime * turningFactor;
		} 
		
		else if (isTurningRight && horizontalAxisValue < 1) {
			horizontalAxisValue += Time.deltaTime * turningFactor;
		} 
		
		else if (!isTurningLeft && !isTurningRight) {
			var factor = horizontalAxisValue > 0 ? -1 : 1;
			horizontalAxisValue += Time.deltaTime * turningFactor * factor;
		}

		if (isAccelerating && throttleValue < maxThrottleValue) {
			throttleValue += Time.deltaTime * throttleFactor;

		} else if (!isAccelerating && throttleValue > 0) {
			throttleValue -= Time.deltaTime * throttleFactor;
		}

		Debug.Log("throttle: " + throttleValue);

	}
}
