using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePad
{
    public bool GetButtonRight()
    {
        return Input.GetButton("DPadR") || Input.GetKey(KeyCode.RightArrow);
    }

    public bool GetButtonLeft()
    {
        return Input.GetButton("DPadL") || Input.GetKey(KeyCode.LeftArrow);
    }

    public bool GetButtonDown()
    {
        return Input.GetButton("DPadDown");
    }

    public bool GetButtonUp()
    {
        return Input.GetButton("DPadUp");
    }

    public bool GetButtonA()
    {
        return Input.GetButton("AButton") || Input.GetKey(KeyCode.S);
    }

    public bool GetButtonY()
    {
        return Input.GetButton("YButton");
    }

    public bool GetButtonX()
    {
        return Input.GetButton("XButton");
    }

    public bool GetButtonB()
    {
        return Input.GetButton("BButton");
    }

    public bool GetButtonStart()
    {
        return Input.GetButton("StartButton");
    }

    public bool GetButtonBack()
    {
        return Input.GetButton("BackButton");
    }

    public bool GetButtonLeftBumper()
    {
        return Input.GetButtonDown("LeftBumper") || Input.GetKeyDown(KeyCode.A);
    }

    public bool GetButtonRightBumper()
    {
        return Input.GetButtonDown("RightBumper") || Input.GetKeyDown(KeyCode.D);
    }

    public bool GetButtonLeftTrigger()
    {
        return Input.GetButton("LeftTrigger");
    }

    public bool GetButtonRightTrigger()
    {
        return Input.GetButton("RightTrigger");
    }

    public bool GetButtonXbox()
    {
        return Input.GetButton("XboxButton");
    }

    public float GetLeftStickXAxis()
    {
        return Input.GetAxis("LeftStickX");
    }

    public float GetLeftStickYAxis()
    {
        return Input.GetAxis("LeftStickY");
    }

    public float GetRightStickXAxis()
    {
        return Input.GetAxis("RightStickX");
    }

    public float GetRightStickYAxis()
    {
        return Input.GetAxis("RightStickY");
    }

}
