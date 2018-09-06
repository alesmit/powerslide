using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    public static float AvoidNearZero(float value, float tolerance = .1f) {

        if ((value > 0 && value < tolerance) || (value < 0 && value > -tolerance)) {
            return 0;
        }

        return value;

    }

}
