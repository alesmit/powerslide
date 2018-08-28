using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform target;

	Vector3 offset;

	// Use this for initialization
	void Start () {
		//offset.z = transform.position.z - target.position.z;
		offset = new Vector3(transform.position.x, transform.position.y - target.position.y, transform.position.z - target.position.z);
		
	}

	void LateUpdate () {

		transform.position = target.position + offset;
	}
}
