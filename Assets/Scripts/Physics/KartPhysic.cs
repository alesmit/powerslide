using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPhysic : MonoBehaviour {

	private Rigidbody rb;
	private KartInput ki;
    private float speedNearZeroTolerance = .1f;

    [Tooltip("Speed factor.")]
    public float speedFactor = 10f;

    [Tooltip("Center of mass of the object.")]
	public Transform centerOfMass;

    [Tooltip("Maximum speed the object can reach.")]
    public float topSpeed;

    [Tooltip("How fast will object reach the top speed.")]
    public float acceleration;

    [Tooltip("How fast will object reach the 0 speed.")]
    public float deceleration;

    [Tooltip("Steer handling. The more the value, the more the object steering is fast.")]
	public float steerAngle;

    private float Speed { get; set; }

    void Awake() {
        Speed = 0;
    }

	void Start() {

		rb = GetComponent<Rigidbody>();
		ki = GetComponent<KartInput>();

		rb.centerOfMass = centerOfMass.localPosition;

	}

	void FixedUpdate() {

        /*
         * Logic for setting the kart speed dynamically,
         * based on its acceleration
         */

        var acc = acceleration * speedFactor * Time.deltaTime;
        var dec = deceleration * speedFactor * Time.deltaTime;

		if (ki.IsAccelerating() && Speed < topSpeed) {
            Speed += acc;

        } else if (!ki.IsAccelerating()) {

            ForceSpeedZero();

            if (Speed > 0) {
                Speed -= dec;
            } else if (Speed < 0) {
                Speed += dec;
            }

        }

        // move the kart forward according to Speed value
        ApplySpeed();

		// steering
		var steerForce = steerAngle * ki.SteerValue;
		rb.transform.Rotate(Vector3.up * steerForce * Time.deltaTime);

	}

    private void ApplySpeed() {
        Vector3 velocity = transform.forward * Speed * Time.deltaTime;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    /*
     * Forces Speed to 0 when it's very close to it
     * to avoid kart flickering on the horizontal axis
     */
    private void ForceSpeedZero() {
        if (
            (Speed > 0 && Speed < speedNearZeroTolerance) ||
            (Speed < 0 && Speed > -speedNearZeroTolerance)
        ) {
            Speed = 0;
        }
    }

}
