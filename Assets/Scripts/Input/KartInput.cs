using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartInput : MonoBehaviour
{
    private float maxSteerValue = 1;

    [Tooltip("The higher the value, the more time it's needed to reach the max steer value.")]
    public float steerFactor;

    [Tooltip("When the steering value has to be forced to 0.")]
    public float zeroSteerThreshold = .1f;

    public float SteerValue { get; private set; }

    public bool IsTurningRight { get; private set; }
    public bool IsTurningLeft { get; private set; }
    public bool IsAccelerating { get; private set; }
    public bool IsGoingReverse { get; private set; }
    public bool IsBraking { get; private set; }
    public bool IsJumping { get; private set; }

    private GamePad gamePad;

    void Awake()
    {
        // init gamepad class
        gamePad = new GamePad();

        // set initial values
        SteerValue = 0;
    }

    void Update()
    {
        IsTurningRight = gamePad.GetButtonRight();
        IsTurningLeft = gamePad.GetButtonLeft();
        IsAccelerating = gamePad.GetButtonA();
        IsGoingReverse = gamePad.GetButtonDown();
        IsBraking = gamePad.GetButtonX();
        IsJumping = gamePad.GetButtonLeftBumper() || gamePad.GetButtonRightBumper();

        SetSteerValue();

    }

    /*
     * Assign SteerValue:
     * Smoothly increases the SteerValue up to its max value as long as a D-Pad button is down
     * Smoothly decreases it when a D-Pad button is no longer being pressed
     */
    private void SetSteerValue()
    {
        var steer = Time.deltaTime * steerFactor;

        if (IsTurningLeft && SteerValue > -maxSteerValue)
        {
            SteerValue -= steer;
        }
        else if (IsTurningRight && SteerValue < maxSteerValue)
        {
            SteerValue += steer;
        }
        else if (!IsTurningLeft && !IsTurningRight)
        {
            SteerValue = Utils.AvoidNearZero(SteerValue, zeroSteerThreshold);

            if (SteerValue > 0)
            {
                SteerValue -= steer;
            }
            else if (SteerValue < 0)
            {
                SteerValue += steer;
            }
        }
    }

}
