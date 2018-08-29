using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePad {

    /*
     * TODO @sevenyz add methods to get axis values (float) for:
     * LeftStickX, LeftStickY, RightStickX, RightStickY
     */

    public bool GetButtonRight() {
        return Input.GetButton("DPadR");
    }

    public bool GetButtonLeft() {
        return Input.GetButton("DPadL");
    }

    public bool GetButtonDown() {
        return Input.GetButton("DPadDown");
    }

    public bool GetButtonUp() {
        return Input.GetButton("DPadUp");
    }

    public bool GetButtonA() {
        return Input.GetButton("AButton");
    }

    public bool GetButtonY() {
        return Input.GetButton("YButton");
    }

    public bool GetButtonX() {
        return Input.GetButton("XButton");
    }

    public bool GetButtonB() {
        return Input.GetButton("BButton");
    }

    public bool GetButtonStart() {
        return Input.GetButton("StartButton");
    }

    public bool GetButtonBack() {
        return Input.GetButton("BackButton");
    }

    public bool GetButtonLeftBumper() {
        return Input.GetButton("LeftBumper");
    }

    public bool GetButtonRightBumper() {
        return Input.GetButton("RightBumper");
    }

    public bool GetButtonLeftTrigger() {
        return Input.GetButton("LeftTrigger");
    }

    public bool GetButtonRightTrigger() {
        return Input.GetButton("RightTrigger");
    }

    public bool GetButtonXbox() {
        return Input.GetButton("XboxButton");
    }

}