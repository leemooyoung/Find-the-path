using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(0.02f, 8.0f, 0.2f);        //처음 위치
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y > 3.0f)
        {
            transform.Translate(0, -0.2f, 0);
        }
        else if(transform.position.y > 0.2f)
        {
            transform.Translate(-0.2f, -0.1f, 0.2f); // (-3.5,0.06,3.5)
            transform.Rotate(-1.0f, 0, 2.0f);
        }
	}
}
