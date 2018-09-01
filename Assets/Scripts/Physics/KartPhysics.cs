using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPhysics : MonoBehaviour {

	private Rigidbody rb;
	private KartInput ki;
    private Gravity gravity;
    private GroundCheck groundCheck;

    private float speedNearZeroTolerance = 1f;

    [Tooltip("Speed factor.")]
    public float speedFactor = 10f;

    [Tooltip("Jumping factor.")]
    public float jumpingFactor = 1000f;

    [Tooltip("Center of mass of the object.")]
	public Transform centerOfMass;

    [Tooltip("Maximum speed the object can reach.")]
    public float topSpeed;

    [Tooltip("Maximum speed the object can move backwards.")]
    public float topReverseSpeed;

    [Tooltip("How fast will object reach the top speed.")]
    public float acceleration;

    [Tooltip("How fast will object reach the 0 speed after releasing the acceleration button.")]
    public float deceleration;

    [Tooltip("How fast will the object stop after pressing the brake button.")]
    public float brakingForce;

    [Tooltip("How much vertical force apply on the object to make it jump.")]
    public float jumpingForce;

    [Tooltip("Steer handling. The more the value, the more the object steering is fast.")]
	public float steerAngle;

    [Tooltip("How much smooth rotate the object on the Z axis when is not grounded.")]
    public float zRotationSmoothness = .1f;

    private float Speed { get; set; }

    void Awake() {
        Speed = 0;
    }

	void Start() {

		rb = GetComponent<Rigidbody>();
		ki = GetComponent<KartInput>();
        gravity = GetComponent<Gravity>();
        groundCheck = GetComponentInChildren<GroundCheck>();

		rb.centerOfMass = centerOfMass.localPosition;

	}

	void FixedUpdate() {

        // Dynamically set the kart speed
        var acc = acceleration * speedFactor * Time.deltaTime;
        var dec = deceleration * speedFactor * Time.deltaTime;
        var brk = brakingForce * speedFactor * Time.deltaTime;

        if (groundCheck.isGrounded) {

            if (ki.IsBraking()) {

                ForceSpeedZero();

                if (Speed > 0) {
                    Speed -= brk;
                } else if (Speed < 0) {
                    Speed += brk;
                }

            } else if (ki.IsAccelerating() && Speed < topSpeed) {
                Speed += acc;

            } else if (ki.IsGoingReverse() && Speed > topReverseSpeed) {
                Speed -= acc;

            } else if (!ki.IsAccelerating() && !ki.IsGoingReverse()) {

                ForceSpeedZero();

                if (Speed > 0) {
                    Speed -= dec;
                } else if (Speed < 0) {
                    Speed += dec;
                }

            }

        }

        // move the kart forward according to Speed value
        ApplySpeed();

		// steering
		if (Speed != 0) {
            var steerForce = steerAngle * ki.SteerValue;
            var direction = ki.IsGoingReverse() ? Vector3.down : Vector3.up;
            rb.transform.Rotate(direction * steerForce * Time.deltaTime);
        }

        // block Z axis rotation when the kart is not grounded
        if (!groundCheck.isGrounded) {
            var targetRotation = transform.rotation * Quaternion.AngleAxis(transform.rotation.eulerAngles.z * -1, Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10 * zRotationSmoothness * Time.deltaTime);
        }

	}

    private void ApplySpeed() {
        Vector3 velocity = transform.forward * Speed * Time.deltaTime;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    /*
     * Forces Speed to 0 when it's very close to it
     * to avoid kart slowly moving forward/backward
     */
    private void ForceSpeedZero() {
        if (
            (Speed > 0 && Speed < speedNearZeroTolerance * speedFactor) ||
            (Speed < 0 && Speed > -speedNearZeroTolerance * speedFactor)
        ) {
            Speed = 0;
        }
    }

}
