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

    [Tooltip("Steer handling. The more the value, the more the object steering is fast.")]
	public float steerAngle;

    [Tooltip("How fast the object's X rotation resets when is not grounded.")]
    public float flightXRotationRoughness = 1f;

    [Tooltip("How fast the object's Z rotation resets when is not grounded.")]
    public float flightZRotationRoughness = .3f;

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

        ResetRotationWhenNotGrounded();
        ApplySpeed();
        ApplySteer();

	}

    /*
     * Make the kart move forward/backward
     */
    private void ApplySpeed() {

        // set speed dynamically

        var acc = acceleration * speedFactor * Time.deltaTime;

        var xOppositionForce = transform.rotation.x * -1;
        acc += xOppositionForce * 100000 * Time.deltaTime;


        var dec = deceleration * speedFactor * Time.deltaTime;

        if (groundCheck.isGrounded) {

            if (ki.IsAccelerating() && Speed < topSpeed) {
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



        Debug.Log(Speed);

        // apply speed
        Vector3 velocity = transform.forward * Speed * Time.deltaTime;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

    }

    /*
     * Make the kart steer
     */
    private void ApplySteer() {
        var steerForce = steerAngle * ki.SteerValue;
        var direction = ki.IsGoingReverse() ? Vector3.down : Vector3.up;
        rb.transform.Rotate(direction * steerForce * Time.deltaTime);
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

    /*
     * Reset Z and X rotation when the kart is not grounded
     * Note: Y rotation is not resetted (even when flying the kart should be able to steer)
     */
    private void ResetRotationWhenNotGrounded() {
        if (!groundCheck.isGrounded) {

            // reset Z rotation
            Quaternion zRotation = transform.rotation * Quaternion.AngleAxis(transform.rotation.eulerAngles.z * -1, Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, zRotation, 10 * flightZRotationRoughness * Time.deltaTime);

            // reset X rotation
            Quaternion xRotation = transform.rotation * Quaternion.AngleAxis(transform.rotation.eulerAngles.x * -1, Vector3.right);
            transform.rotation = Quaternion.Lerp(transform.rotation, xRotation, 10 * flightXRotationRoughness * Time.deltaTime);

        }
    }

}
