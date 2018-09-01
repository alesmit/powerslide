using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    void LateUpdate() {
        transform.rotation *= Quaternion.AngleAxis(transform.rotation.eulerAngles.z * -1, Vector3.forward);
    }

}
