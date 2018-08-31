using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity : MonoBehaviour {

	[Tooltip("Gravity factor.")]
    public float gravityScale = 1.0f;

    [Tooltip("Global gravity value.")]
    public float globalGravity = -9.81f;

    private Rigidbody rb;

    private void OnEnable() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
	}

    private void FixedUpdate() {
		Vector3 gravity = globalGravity * gravityScale * Vector3.up;
		rb.AddForce(gravity, ForceMode.Acceleration);
	}

}
