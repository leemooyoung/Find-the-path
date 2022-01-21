using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {
    int[] road;
    int direction;
    int state = 0;
    float velocity = 6.0f;
    Vector3 Goal;
    Vector2 playerPos;
    Vector2 position;
    Vector2 v1;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if(state < 2)
        {
            Goal = new Vector3(position.x / 2 - 3.5f, transform.position.y, position.y / 2 - 3.5f);

            if ((Goal - transform.position).magnitude < 2 * velocity * Time.deltaTime)
            {
                int i;
                int j;
                if (direction < 2)
                {
                    i = (int)(position + v1).y;

                    if (0 <= i && i <= 14) j = road[i];
                    else j = 1;
                }
                else
                {
                    i = (int)(position + v1).x;

                    if (0 <= i && i <= 14) j = road[i];
                    else j = 1;
                }

                if (j == 1)
                {
                    v1 = -v1;
                    state++;
                }
                else
                {
                    position = position + v1;
                }
            }
            else
            {
                transform.Translate(v1.x * velocity * Time.deltaTime, 0, v1.y * velocity * Time.deltaTime, Space.World);
            }
        }
        else
        {
            Destroy(gameObject, 0.3f);
        }
    }

    public void setpath(int[] r)
    {
        road = new int[15];
        for(int i = 0;i < 15; i++)
        {
            road[i] = r[i];
        }
        direction = r[15];
        switch (direction)
        {
            case 0:
                v1 = new Vector2(0, 1);
                break;
            case 1:
                v1 = new Vector2(0, -1);
                break;
            case 2:
                v1 = new Vector2(-1, 0);
                break;
            case 3:
                v1 = new Vector2(1, 0);
                break;
        }
        playerPos = position = new Vector2(r[16], r[17]);
    }
}
