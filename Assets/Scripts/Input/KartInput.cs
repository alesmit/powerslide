using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartInput : MonoBehaviour {

    private float maxSteerValue = 1;
    private float maxThrottleValue = 1;

    [Tooltip("The higher is, the more time it's needed to reach the max steer value.")]
    public float steerFactor;
    
    [Tooltip("The higher is, the more time it's needed to reach the max throttle value.")]
    public float throttleFactor;

    public float SteerValue { get; private set; }
    public float ThrottleValue { get; private set; }

    private float steerNearZeroTolerance = .1f;

    private GamePad gamePad;

    void Awake() {

        // init gamepad class
        gamePad = new GamePad();

        // set initial values
        SteerValue = 0;
        ThrottleValue = 0;

    }

    void Update() {

        /*
         * Assign SteerValue:
         * Smoothly increase the SteerValue up to its max value as long as a D-Pad button is down
         * Smoothly decrease it when a D-Pad button is no longer being pressed
         */

        var steer = Time.deltaTime * steerFactor;

        if (IsTurningLeft() && SteerValue > maxSteerValue * -1) {
			SteerValue -= steer;

		} else if (IsTurningRight() && SteerValue < maxSteerValue) {
			SteerValue += steer;

		} else if (!IsTurningLeft() && !IsTurningRight()) {

            ForceSteerZero();
			
            if (SteerValue > 0) {
                SteerValue -= steer;

            } else if (SteerValue < 0) {
                SteerValue += steer;

            }

		}

        /*
         * Assign ThrottleValue:
         * Smoothly increase the ThrottleValue up to its max value as long as the acceleration button is down
         * Smoothly decrease it to zero when the gamer stops accelerating
         */

        var throttle = Time.deltaTime * throttleFactor;

        if (IsAccelerating() && ThrottleValue < maxThrottleValue) {
			ThrottleValue += throttle;

		} else if (!IsAccelerating()) {

            if (ThrottleValue > 0) {
                ThrottleValue -= throttle;

            } else if (ThrottleValue < 0) {
                ThrottleValue = 0;
            }

		}

    }

    /*
     * Force SteerValue to 0 when is very close to it
     * to avoid kart flickering on the horizontal axis
     */
    private void ForceSteerZero() {
        if (
            (SteerValue > 0 && SteerValue < steerNearZeroTolerance) || 
            (SteerValue < 0 && SteerValue > -steerNearZeroTolerance)
        ) {
            SteerValue = 0;
        }
    }

    public bool IsTurningRight() {
        return gamePad.GetButtonRight();
    }

    public bool IsTurningLeft() {
        return gamePad.GetButtonLeft();
    }

    public bool IsAccelerating() {
        return gamePad.GetButtonA();
    }

    public bool IsGoingReverse() {
        return gamePad.GetButtonDown();
    }

    public bool IsBraking() {
        return gamePad.GetButtonX();
    }

}
