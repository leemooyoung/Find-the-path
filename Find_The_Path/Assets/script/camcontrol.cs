using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camcontrol : MonoBehaviour {

    const float PI = 3.1415926535f;
    const float AngularVelocity = 360.0f / 60.0f;

    void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(-10, 0, 0);
        transform.Rotate(0, -Time.deltaTime * AngularVelocity, 0);
        //transform.Rotate(0, 0, 0.1f);
        transform.Rotate(10, 0, 0);
    }
}
