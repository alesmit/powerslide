using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartPhysics : MonoBehaviour
{
    private Rigidbody rb;
    private KartInput ki;
    private GroundCheck groundCheck;

    public Text speedDisplay;
    public Text speedometer;

    [Tooltip("Center of mass of the object.")]
    public Transform centerOfMass;

    [Header("Powerslide")]

    [Tooltip("Minimum speed required for powerslides to work.")]
    public float powMinSpeed = 500f;

    [Tooltip("How much X velocity increases/decreases during powerslides.")]
    public float powXVelVariation;

    [Tooltip("How much steer force increases during powerslides.")]
    public float powSteerForceFactor;

    [Tooltip("Boost to apply in powerlides.")]
    public float powBoostForce;

    [Header("Slope")]

    [Tooltip("How fast speed changes according to the slope angle.")]
    public float slopeFactor;

    [Tooltip("How much slopes can impact on the top speed.")]
    public float slopeImpactOnTopSpeed;

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

    private float Speed { get; set; }

    [Header("READ ONLY. Please do not touch values here.")]

    public SpeedTrend speedTrend;

    public PowerslideDirection powerslideDirection;

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

    void Update()
    {
        ApplyJump();
        UpdatePowerslideDirection();
        ApplyPowerslideBoost();
    }

    private void LateUpdate()
    {
        UpdateUI();
    }

    private void ApplyJump()
    {
        if (ki.IsJumping && groundCheck.isGrounded && powerslideDirection == PowerslideDirection.None) {
            rb.AddForce(Vector3.up * jumpForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    /*
     * Make the kart move forward/backward
     */
    private void ApplySpeed()
    {
        if (groundCheck.isGrounded && powerslideDirection == PowerslideDirection.None)
        {

            // top speed may change depending some factors:

            // on the slope
            float slope = Utils.AvoidNearZero(Mathf.Round(transform.forward.normalized.y * 1000) / 1000, .005f);
            float speedChangeDueToSlope = slope * slopeFactor * -1;

            // on the steering
            float speedChangeDueToSteering = Mathf.Abs(ki.SteerValue) * speedSteerDecrementFactor * -1;

            // create a new var maxSpeed to apply those changes
            float maxSpeed = topSpeed + speedChangeDueToSteering + speedChangeDueToSlope * slopeImpactOnTopSpeed;

            if (ki.IsAccelerating)
            {
                float normalizedSpeedIncrement = accelerationPower / topSpeed;
                float speedIncrement = accelerationCurve.Evaluate(normalizedSpeedIncrement) * topSpeed;

                if (Speed < maxSpeed)
                {
                    speedTrend = SpeedTrend.Increasing;
                    Speed += speedIncrement * Time.deltaTime;
                }
                else
                {
                    speedTrend = SpeedTrend.Decreasing;
                    Speed -= speedIncrement * Time.deltaTime;
                }

            }

            if (ki.IsGoingReverse && Speed > topReverseSpeed)
            {
                speedTrend = SpeedTrend.Decreasing;
                Speed -= accelerationPower * Time.deltaTime;
            }

            if (!ki.IsAccelerating && !ki.IsGoingReverse)
            {

                // force Speed to be 0 when it's very close to it to avoid kart slowly moving forward/backward
                Speed = Utils.AvoidNearZero(Speed, zeroSpeedThreshold);

                if (Speed > 0)
                {
                    speedTrend = SpeedTrend.Decreasing;
                    Speed -= decelerationPower * Time.deltaTime;
                }
                else if (Speed < 0)
                {
                    speedTrend = SpeedTrend.Increasing;
                    Speed += decelerationPower * Time.deltaTime;
                }

            }

            // speed always increases or decreases depending on the slope
            Speed += speedChangeDueToSlope;

        }

        // apply speed
        Vector3 velocity = transform.forward * Speed * Time.deltaTime;

        // alter velocity Z and X when the kart is powersliding
        if (powerslideDirection != PowerslideDirection.None)
        {
            velocity = GetPowerslideVelocity(velocity);
        }

        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

    }

    private void UpdatePowerslideDirection()
    {
        if (ki.IsPowersliding && groundCheck.isGrounded && ki.SteerValue != 0 && Speed > powMinSpeed)
        {
            if (powerslideDirection == PowerslideDirection.None)
            {
                powerslideDirection = ki.SteerValue > 0
                    ? PowerslideDirection.Right
                    : PowerslideDirection.Left;
            }
        }
        else
        {
            powerslideDirection = PowerslideDirection.None;
        }
    }

    private void ApplyPowerslideBoost()
    {
        if (ki.IsJumping && powerslideDirection != PowerslideDirection.None)
        {
            rb.AddRelativeForce(transform.forward * powBoostForce * Time.fixedDeltaTime, ForceMode.Impulse);
            Debug.Log("Boost!");
        }
    }

    private Vector3 GetPowerslideVelocity(Vector3 baseVelocity)
    {
        var localVelocity = transform.InverseTransformDirection(baseVelocity);

        switch (powerslideDirection)
        {
            case PowerslideDirection.Right:
            localVelocity.x -= powXVelVariation;
            break;

            case PowerslideDirection.Left:
            localVelocity.x += powXVelVariation;
            break;

            default:
            case PowerslideDirection.None:
            localVelocity.x = 0;
            break;
        }

        return transform.TransformDirection(localVelocity);
    }

    private void UpdateUI()
    {
        // show speed
        speedDisplay.text = Mathf.RoundToInt(Speed).ToString();

        // rotate speedometer
        var normalizedSpeed = Speed / topSpeed * .95f;
        var angle = Mathf.Rad2Deg * Mathf.Asin(-normalizedSpeed);
        speedometer.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /*
     * Make the kart steer
     */
    private void ApplySteer()
    {
        var steerForce = steeringSpeed * ki.SteerValue;
        var direction = ki.IsGoingReverse && !ki.IsAccelerating ? Vector3.down : Vector3.up;

        if (powerslideDirection != PowerslideDirection.None)
        {
            steerForce *= powSteerForceFactor;
        }

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
