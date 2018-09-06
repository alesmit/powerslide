using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPhysics : MonoBehaviour
{
    private Rigidbody rb;
    private KartInput ki;
    private GroundCheck groundCheck;

    [Tooltip("Center of mass of the object.")]
    public Transform centerOfMass;

    [Tooltip("Slope factor: how fast speed changes according to the slope angle.")]
    public float slopeFactor;

    [Tooltip("When the speed has to be forced to 0.")]
    public float zeroSpeedThreshold;

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

    [Tooltip("Steering speed. The more the value, the more the object steering is fast.")]
    public float steeringSpeed;

    [Tooltip("How fast the object's X rotation resets when is not grounded.")]
    public float flightXRotationRoughness;

    [Tooltip("How fast the object's Z rotation resets when is not grounded.")]
    public float flightZRotationRoughness;

    private float Speed { get; set; }

    void Awake()
    {
        Speed = 0;
    }

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

    /*
     * Make the kart move forward/backward
     */
    private void ApplySpeed()
    {
        if (groundCheck.isGrounded)
        {
            // define how much the speed should increment/decrement
            //var speedIncrement = accelerationCurve.Evaluate(accelerationPower / topSpeed);
            var speedIncrement = accelerationPower;
            var speedDecrement = decelerationPower;

            /*

            // get a value between 0 and 1 which is the slope of the kart
            float slope = Utils.AvoidNearZero(Mathf.Round(transform.forward.normalized.y * 1000) / 1000);
            float slopeVariation = slope * slopeFactor * -1;

            // apply slope variation to the top speed
            float maxSpeed = topSpeed + slopeVariation;

            // apply slope variation to the speed increment/decrement
            speedIncrement += speedIncrement * slopeVariation;
            speedDecrement += speedIncrement * slopeVariation;

            */

            if (ki.IsAccelerating() && Speed < topSpeed /*maxSpeed*/)
            {
                Speed += speedIncrement * Time.deltaTime;
                Debug.Log(string.Format("Speed, Max, Increment: {0}, {1}, {2}", Speed, topSpeed, speedIncrement));
            }

            if (ki.IsGoingReverse() && Speed > topReverseSpeed)
            {
                Speed -= speedIncrement * Time.deltaTime;
            }

            if (!ki.IsAccelerating() && !ki.IsGoingReverse() /*&& slope == 0*/)
            {

                // forces Speed to be 0 when it's very close to it to avoid kart slowly moving forward/backward
                Speed = Utils.AvoidNearZero(Speed, zeroSpeedThreshold);

                if (Speed > 0)
                {
                    Speed -= speedDecrement * Time.deltaTime;
                }
                else if (Speed < 0)
                {
                    Speed += speedDecrement * Time.deltaTime;
                }

            }

        }

        // apply speed
        Vector3 velocity = transform.forward * Speed * Time.deltaTime;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

    }

    /*
     * Make the kart steer
     */
    private void ApplySteer()
    {
        var steerForce = steeringSpeed * ki.SteerValue;
        var direction = ki.IsGoingReverse() && !ki.IsAccelerating() ? Vector3.down : Vector3.up;
        rb.transform.Rotate(direction * steerForce * Time.deltaTime);
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
