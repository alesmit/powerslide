using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartPhysics2 : MonoBehaviour
{
    private Rigidbody rb;
    private KartInput ki;
    private GroundCheck groundCheck;

    [Tooltip("Center of mass of the object.")]
    public Transform centerOfMass;

    [Header("Speed")]

    [Tooltip("Maximum speed the object reaches when going on a straight surface.")]
    public float topSpeed;

    [Tooltip("Maximum speed the object can move backwards.")]
    public float topReverseSpeed;

    [Tooltip("How fast will object reach the top speed.")]
    public float accelerationPower;

    [Tooltip("Defines how the maximum speed is reached.")]
    public AnimationCurve accelerationCurve;

    [Tooltip("How fast will object reach the 0 speed after releasing the acceleration button.")]
    public float decelerationPower;

    [Tooltip("Threshold to force the speed to 0.")]
    public float zeroSpeedThreshold;

    [Tooltip("How much the top speed decreases when steering.")]
    public float speedSteerDecrementFactor = 200f;

    [Header("Steering")]

    [Tooltip("The more the value, the more the object steering is fast.")]
    public float steeringSpeed;

    [Header("Jumping")]

    [Tooltip("Jump force.")]
    public float jumpForce;

    [Header("Not grounded behaviour")]

    [Tooltip("How fast the object's X rotation resets when is not grounded.")]
    public float flightXRotationRoughness;

    [Tooltip("How fast the object's Z rotation resets when is not grounded.")]
    public float flightZRotationRoughness;

    public float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ki = GetComponent<KartInput>();
        groundCheck = GetComponentInChildren<GroundCheck>();

        // set center of mass
        rb.centerOfMass = centerOfMass.localPosition;
    }

    void FixedUpdate()
    {
        ResetRotationWhenNotGrounded();
        ApplySteer();
        ApplySpeed();
    }

    void Update()
    {
        currentSpeed = rb.velocity.magnitude;
        ApplyJump();
    }

    private void ApplyJump()
    {
        if (ki.IsJumping && groundCheck.isGrounded) {
            rb.AddForce(Vector3.up * jumpForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    /*
     * Make the kart move forward/backward
     */
    private void ApplySpeed()
    {
        if (groundCheck.isGrounded)
        {

            rb.drag = 6;

            if (ki.IsAccelerating)
            {
                if (currentSpeed < topSpeed)
                {
                    rb.AddRelativeForce(Vector3.forward * accelerationPower * Time.deltaTime, ForceMode.Impulse);
                }
                else
                {
                    rb.AddRelativeForce(Vector3.forward * -accelerationPower * Time.deltaTime, ForceMode.Impulse);
                }
            }

            /*
            if (ki.IsGoingReverse && Speed > topReverseSpeed)
            {
                speedTrend = SpeedTrend.Decreasing;
                Speed -= accelerationPower * Time.deltaTime;
            }
            */

            if (!ki.IsAccelerating && !ki.IsGoingReverse)
            {

                float curSpeed = Utils.AvoidNearZero(currentSpeed, zeroSpeedThreshold);

                if (curSpeed > 0)
                {
                    rb.AddRelativeForce(Vector3.forward * decelerationPower * Time.deltaTime, ForceMode.Impulse);
                }
                else if (curSpeed < 0)
                {
                    rb.AddRelativeForce(Vector3.forward * -decelerationPower * Time.deltaTime, ForceMode.Impulse);
                }

            }

        } else {
            rb.drag = 0;
        }

    }

    /*
     * Make the kart steer
     */
    private void ApplySteer()
    {
        var steerForce = steeringSpeed * ki.SteerValue;
        var direction = ki.IsGoingReverse && !ki.IsAccelerating ? Vector3.down : Vector3.up;
        transform.Rotate(steerForce * direction * Time.deltaTime);
    }

    /*
     * Reset Z and X rotation when the kart is not grounded
     * Note: Y rotation is not resetted (even when flying the kart should be able to steer)
     */
    private void ResetRotationWhenNotGrounded()
    {
        if (!groundCheck.isGrounded)
        {
            // reset Z rotation
            Quaternion zRotation = transform.rotation * Quaternion.AngleAxis(transform.rotation.eulerAngles.z * -1, Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, zRotation, 10 * flightZRotationRoughness * Time.deltaTime);

            // reset X rotation
            Quaternion xRotation = transform.rotation * Quaternion.AngleAxis(transform.rotation.eulerAngles.x * -1, Vector3.right);
            transform.rotation = Quaternion.Lerp(transform.rotation, xRotation, 10 * flightXRotationRoughness * Time.deltaTime);
        }
    }

}
