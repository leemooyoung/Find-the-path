using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    const int velocity = 2;

    returnable info;    // *setgoal 함수 호출시의 r을 그대로 받는 멤버변수
    readonly Vector3 zeropoint = new Vector3(-3.5f, 0, -3.5f);
    Vector3 final;
    Vector3 middle;
    Vector3 direction;

    public int ismoving;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (ismoving == 1)
        {
            float t = velocity * Time.deltaTime;
            direction = middle - transform.position;
            if (direction.magnitude > t)
            {
                transform.Translate(direction.normalized * t, Space.World);
            }
            else
            {
                transform.position = final;
                ismoving = 0;
            }
        }
    }

    public void SetGoal(returnable wow)
    {
        ismoving = 1;
        info = wow;
        middle = new Vector3(info.middle.x, 0, info.middle.y)/2 + zeropoint;
        final = new Vector3(info.final.x, 0, info.final.y) / 2 + zeropoint;
    }

    public void SetGoal(Vector2 wow)
    {
        ismoving = 1;
        middle = final = new Vector3(wow.x, 0, wow.y) / 2 + zeropoint;
    }
}