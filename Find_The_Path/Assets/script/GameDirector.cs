using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum D { UP, DOWN, LEFT, RIGHT, STOP }

public struct returnable
{
    public Vector2 final;
    public Vector2 middle;
    public Vector2 portal;
    public int state; //통과 : 0      충돌 : 1      포탈 : 2      시간초과 : 3        포탈체크 : 4

    public D d;

    public returnable(Vector2 _final, Vector2 _middle, int _state, D _d)
    {
        final = _final;
        middle = _middle;
        portal = new Vector2(0, 0);
        state = _state;
        d = _d;
    }

    public returnable(Vector2 _final, Vector2 _middle, Vector2 _portal, int _state, D _d)
    {
        final = _final;
        middle = _middle;
        portal = _portal;
        state = _state;
        d = _d;
    }
}

public class GameDirector : MonoBehaviour
{
    const float timelimit = 15.0f;
    const float resttime = 1.0f;
    
    int t = 0;
    public int s = 0;
    int state = 0;
    int isLaser = 0;
    float time = timelimit;
    float RestTime = resttime;
    float[] distance;
    bool[,] items;
    Vector2[] pgoal;
    Vector3[] pstart;
    Map board;
    GameObject[] player;
    PlayerController[] pcontrol;
    GameObject pausekey;
    GameObject[,] buttons;
    GameObject timer;
    public GameObject LaserPrefab;

    void SetButton(bool on)
    {
        bool[] WhichA;
        if (isLaser == 0)
        {
            WhichA = board.Wa(t);
            for (int i = 0; i < 4; i++)
            {
                buttons[t, i].GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
        else
        {
            WhichA = new bool[] { true, true, true, true };
            for (int i = 0; i < 4; i++)
            {
                buttons[t, i].GetComponent<Image>().color = new Color(0.95f, 0, 0);
            }
        }

        int j = 0;
        while (j < 4)
        {
            buttons[t, j].SetActive(on && WhichA[j]);
            j++;
        }
        while (j < 6)
        {
            buttons[t, j].SetActive(on && items[t, j - 4]);
            j++;
        }
    }
    
    void Start()
    {
        Debug.Log("start 시작");
        int[,] mapinfo = new int[42, 2]
        {
            {0,3},{0,13},{1,4},{1,8},{1,10},{2,1},{2,5},
            {2,11},{3,2},{3,6},{3,12},{4,9},{4,13},{5,4},
            {5,8},{5,14},{6,3},{6,11},{7,2},{7,12},{8,7},
            {9,0},{9,4},{9,8},{9,12},{10,11},{11,2},{11,12},
            {12,3},{12,7},{12,13},{13,6},{14,1},{14,5},{14,9},
            {14,13},/*포탈*/{1,12},{14,11},{8,1},{8,9},{5,0},{13,4}
        };
        board = new Map(mapinfo);

        pgoal = new Vector2[] { new Vector3(3.5f, 3.5f), new Vector3(-3.5f, 3.5f) };                  //p1, p2의 골인 지점 설정

        player = new GameObject[2];
        player[0] = GameObject.Find("p1");                          //p1, p2 찾기
        player[1] = GameObject.Find("p2");

        pcontrol = new PlayerController[2];
        pcontrol[0] = player[0].GetComponent<PlayerController>();
        pcontrol[1] = player[1].GetComponent<PlayerController>();

        pcontrol[0].SetGoal(new Vector2(0, 0));
        pcontrol[1].SetGoal(new Vector2(14, 0));

        buttons = new GameObject[2, 6];

        buttons[0, 0] = GameObject.Find("play1up");
        buttons[0, 1] = GameObject.Find("play1down");
        buttons[0, 2] = GameObject.Find("play1left");
        buttons[0, 3] = GameObject.Find("play1right");
        buttons[0, 4] = GameObject.Find("double1");
        buttons[0, 5] = GameObject.Find("laser1");
        buttons[1, 0] = GameObject.Find("play2up");
        buttons[1, 1] = GameObject.Find("play2down");
        buttons[1, 2] = GameObject.Find("play2left");
        buttons[1, 3] = GameObject.Find("play2right");
        buttons[1, 4] = GameObject.Find("double2");
        buttons[1, 5] = GameObject.Find("laser2");

        distance = new float[2];

        items = new bool[2, 2] { { true, true }, { true, true } };
        //p1 더블, 레이저 p2 더블, 레이저

        timer = GameObject.Find("Timer");
        
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                buttons[i, j].SetActive(false);
            }
        }
        Debug.Log(items[1,0]);
        Debug.Log("start 끝");
    }

    void Update()
    {
        //Debug.Log("t = "+ t);
        distance[t] = (new Vector2(player[t].transform.position.x, player[t].transform.position.z) - pgoal[t]).magnitude;      //플레이어의 현재 위치와 골인 지점 사이의 거리

        if (t == 0)                                                  //p1 턴일때
        {
            if (distance[t] <= 0.2f)                                //목표 거리가 0.2f 이하면
            {
                SceneManager.LoadScene("win1");                     //p1 승리 씬 호출
            }
        }
        else                                                        //p2 턴에는
        {
            if (distance[t] <= 0.2f)
            {
                SceneManager.LoadScene("win2");                     //p2 승리 씬 호출
            }
        }

        switch (state)
        {
            case 0:                                                 //이동 횟수가 남아있을 때
                if ((time <= 0)||(s==3))                            //시간이 다 되었을 때 또는 이동을 다 했을때
                {
                    if (s != 3) pcontrol[t].SetGoal(board.Gohome());//이동 덜 했는데 시간 다되면 태초.
                    state = 1;                                      //시간이 0 이하가 되면 그 턴을 끝냄.
                    time = 0.0f;                                    
                    SetButton(false);                               //방향키 비활성화
                    break;
                }
                if (pcontrol[t].ismoving == 0)                      //플레이어가 멈춰있다면
                {
                    SetButton(true);                                //방향키 활성화
                }
                time -= Time.deltaTime;
                break;
            case 1:                                                 //이동 횟수가 남지 않았을 때
                RestTime -= Time.deltaTime;
                if (RestTime <= 0)
                {
                    state = 2;
                    RestTime = resttime;
                }
                break;
            case 2: //턴 종료시
                t = (t + 1) % 2;
                time = timelimit;                                       //시간, s 초깃값으로
                state = 0;
                s = 0;
                isLaser = 0;
                board.turn();
                break;
        }
        
        //남은 시간을 UI에 표시하는 코드를 여기에 작성
        timer.GetComponent<Text>().text = "남은 시간 : " + time.ToString("F2");
    }

    public void arrowtable(int d)
    {
        if(isLaser == 1)
        {
            GameObject Laser = Instantiate(LaserPrefab) as GameObject;
            Laser.GetComponent<LaserController>().setpath(board.Line(d));
            Laser.transform.position = pcontrol[t].transform.position + new Vector3(0, 0.5f, 0) ;
            if (d >= 2) Laser.transform.Rotate(0, 0, 90); 
            
            isLaser = 0;
        }
        else
        {
            Debug.Log("d : " + d);
            SetButton(false); //눌리는 순간 비활성화
            returnable r = board.move(t, (D)d);
            if (r.state == 1) s = 2;
            if (r.state == 4) { /*포탈 보이기*/ }
            pcontrol[t].SetGoal(r);
            s++;
        }
    }

    public void DoubleButton()
    {
        if ((s == 0)&& items[t, 0])                                 //s == 0일때만 사용 가능
        {
            s = -3;                                                 //이동 횟수 3번 추가
            time += timelimit;                                      //시간 20초 추가
            items[t, 0] = false;
        }
    }

    public void LazorButton()
    {
        // 원래 방향키 색을 바꾸고,/ 어떤 변수를 바꿔서 방향키로 레이저방향을 조절할 수 있게 한다.
        // 레이저방향이 결정되면, 맵에서 현재위치와 벽의위치를 계산한다.
        // 레이저 프리펩을 생성하고, 방향을 결정해서 계산한 위치까지 움직이게한다.
        // 레이저 프리fab 삭제. 방향키 색 원래대로.
        
        if(items[t, 1] == true) isLaser = 1;
        items[t, 1] = false;

    }
}

class Map
{
    int t = 0;
    private enum Tile { portalA1, portalA2, portalB1, portalB2, portalC1, portalC2, empty, wall, portalCheck };
    private Tile[,] board; //검은 칸은 x좌표 y좌표 합이 4의 배수, 하얀 칸은 아님
    private Vector2[] p; //플레이어 좌표
    private Vector2[] q;
    public Vector2[] playerPos { get { return p; } }
    D[] pastD; //플레이어가 이동했던 방향
    private readonly Vector2[] start; //태초 좌표eddededed
    private Vector2[,] portals; //포탈 목록과 정보

    public Map() //랜덤생성
    {
        Debug.Log("랜덤 맵 생성하는 생성자 작 ☆ 동");

        p = new Vector2[] { new Vector2(0, 0), new Vector2(15, 0) };
        start = new Vector2[] { new Vector2(0, 0), new Vector2(15, 0) };
        List<Vector2> edge = new List<Vector2>();

        board = new Tile[15, 15];
        {/* board 초기화
         * 거울을 27개, 포탈을 3쌍 배치해야한다. 
         * 거울과 포탈을 놓을 수 있는 공간, 즉 칸과 칸의 틈은 촐 112개
         * 포탈에 들어갈 때 검은 칸에서 들어간다면 흰색 칸으로 나온다.
         * 중복 없이 27 + 3*2 개를 뽑아야 한다
         * 뽑고나서 도착 불가능한 맵이 나오면 다시(포탈로 도착가능할 때는 일단 보류)
         * 1) 112개 좌표가 있는 배열에서 뽑을까?(112에서 하나 뽑음-111에서 하나 뽑음-...)
         * 2) 뽑음-벽이 아니면 벽으로 만듦, 벽이면 다시 뽑음-만든 벽이 27개가 될때까지 반복 으로 할까?
         * 3) 먼저 길을 만들고 벽을 배치
         */
        }

        int n = 0;
        for (int i = 0; i <= 7; i++) //edge 초기화
        {
            for (int j = 0; j <= 6; j++)
            {
                edge.Add(new Vector2(j * 2 + 1, i * 2));
                n++;
            }
        }
        for (int i = 0; i <= 7; i++)
        {
            for (int j = 0; j <= 6; j++)
            {
                edge.Add(new Vector2(i * 2, j * 2 + 1));
                n++;
            }
        }

        List<Vector2> road = new List<Vector2>();
        Random r = new Random();
        int LeftMove = 20;

        while (LeftMove > 0)
        {

        }

    }

    public Map(int[,] mapinfo) //주어진 맵 대로 생성, 입력된 배열의 0 ~ 35번 원소는 벽의 좌표, 36 ~ 41번 원소는 포탈의 좌표
    {
        Debug.Log("주어진 배열대로 맵을 만드는 생성자 작 ☆ 동");

        pastD = new D[2];
        p = new Vector2[] { new Vector2(0, 0), new Vector2(14, 0) };
        start = new Vector2[] { new Vector2(0, 0), new Vector2(14, 0) };
        board = new Tile[15, 15]; //board 초기화
        for (int k = 0; k < 15; k++)
        {
            for (int j = 0; j < 15; j++)
            {
                board[k, j] = Tile.empty;
            }
        }
        portals = new Vector2[6, 3];
        /* portals  [0]                 [1]                 [2] ...
         * [0]      portalA1 좌표       portalA2좌표        portalB1좌표 ...
         * [1]      그 옆 흰칸 좌표     그 옆 흰칸 좌표     그 옆 흰칸 좌표 ...
         * [2]      그 옆 검은칸 좌표   그 옆 검은칸 좌표   그 옆 검은칸 좌표 ...
         */

        int i = 0;
        while (i < 36)
        {
            board[mapinfo[i, 0], mapinfo[i, 1]] = Tile.wall; //board에 거울을 표시하는 코드
            i++;
        }
        while (i < 42)
        {
            board[mapinfo[i, 0], mapinfo[i, 1]] = (Tile)(i - 36);
            //배열의 37번째 원소부터는 포탈로, 순서대로 A1, A2, B1, ...이다. board에 포탈을 표시하는 코드
            portals[i - 36, 0] = new Vector2(mapinfo[i, 0], mapinfo[i, 1]);
            //포탈의 정보를 담을 portals배열에 포탈의 좌표를 입력한다.
            i++;
        }
        board[mapinfo[i, 0], mapinfo[i, 1]] = Tile.portalCheck;

        setPortal();
    }

    void setPortal() //portals배열 내용 채우기
    {
        for (int i = 0; i < 6; i++)
        {
            int px = (int)portals[i, 0].x;
            int py = (int)portals[i, 0].y;
            int ix = (px % 2) * (-(px + py) % 4 + 2);
            int iy = (py % 2) * (-(px + py) % 4 + 2);
            portals[i, 2] = portals[i, 0] + new Vector2(-ix, -iy); //포탈 옆의 검은 칸 좌표
            portals[i, 1] = portals[i, 0] + new Vector2(ix, iy); //포탈 옆의 흰색 칸 좌표

            /* portals[i, 2] = portals[i, 0] + new Vector2(-(px % 2) * (-(px + py) % 4 + 2), -(py % 2) * (-(px + py) % 4 + 2)); //포탈 옆의 검은 칸 좌표
             * portals[i, 1] = portals[i, 0] + new Vector2((px % 2) * (-(px + py) % 4 + 2), (py % 2) * (-(px + py) % 4 + 2)); //포탈 옆의 흰색 칸 좌표
            */
        }
    }

    public returnable Gohome()
    {
        returnable r = new returnable(start[t], p[t], 3, D.UP);
        p[t] = start[t];
        return r;
    }

    public void turn()
    {
        t = (t + 1) % 2;
        pastD[0] = pastD[1] = D.STOP;
    }

    public bool[] Wa(int t) // Which Arrow 
    {
        bool[] table = new bool[] { false, false, false, false };
        if (p[t].y < 14) table[0] = true;
        if (p[t].y > 0) table[1] = true;
        if (p[t].x > 0) table[2] = true;
        if (p[t].x < 14) table[3] = true;
        
        if(pastD[t] != D.STOP) table[( 3*(int)pastD[t]+1 )%4] = false;
        return table;
    }
    
    public int[] Line( int d ) //보드의 일부
    {
        int[] r = new int[18];
        if (d < 2)
        {
            for (int i = 0; i < 15; i++)
            {
                if ((int)board[(int)p[t].x, i] == 6)
                {
                    r[i] = 0;
                }
                else
                {
                    r[i] = 1;
                }
            }
        }
        else
        {
            for (int i = 0; i < 15; i++)
            {
                if ((int)board[i,(int)p[t].y] == 6)
                {
                    r[i] = 0;
                }
                else
                {
                    r[i] = 1;
                }
            }
        }
        r[15] = d;
        r[16] = (int)p[t].x;
        r[17] = (int)p[t].y;
        return r;
    }
    
    public returnable move(int t, D d) //플레이어 번호, 방향
    {
        
        Vector2 direction = Vector2.zero;
        pastD[t] = d;
        switch (d)
        {
            case D.UP:
                direction = Vector2.up;
                break;
            case D.DOWN:
                direction = Vector2.down;
                break;
            case D.LEFT:
                direction = Vector2.left;
                break;
            case D.RIGHT:
                direction = Vector2.right;
                break;
        } //direction 초기화

        p[t] = p[t] + direction;

        /*
        if (p[t].x < 0 || p[t].x > 14 || p[t].y < 0 || p[t].y > 14) //맵 바깥으로 나가려 할 때
        {
            p[t] = p[t] - direction;
            Debug.Log("벗어날 수 있을거라 생각했나?");
            return p[t];
            //return 이동하지 않기로 약속된 어떤 값(s를 증가시키지 않는)
        }*/

        int c = ((int)((p[t] - direction).x + (p[t] - direction).y) % 4) / 2 + 1; //이전에 있던 타일이 검은색이면 c = 1 ,흰색이면 2
        int tile = (int)board[(int)p[t].x, (int)p[t].y];
        returnable r;

        switch ((Tile)tile)
        {
            case Tile.empty:
                p[t] = p[t] + direction;
                if (board[(int)p[t].x, (int)p[t].y] == Tile.portalCheck)
                    r = new returnable(p[t], p[t], 4, d);
                else
                    r = new returnable(p[t], p[t], 0, d);

                return r;

            case Tile.wall:
                r = new returnable(start[t], p[t], 1, d);
                p[t] = start[t];
                
                return r;
            
            default:
                int portalOut = ((tile + 1) % 2) * (tile + 1) + (tile % 2) * (tile - 1);
                /* 위 식을 f(tile)이라 하면
                 * f(0) = 1, f(portalA1) = portalA2
                 * f(1) = 0, f(portalA2) = portalA1
                 * f(2) = 3, f(portalB1) = portalB2
                 * f(3) = 2, f(portalB2) = portalB1
                 * f(4) = 5, f(portalC1) = portalC2
                 * f(5) = 4, f(portalC2) = portalc1
                 */
                r = new returnable(portals[portalOut, c], p[t], 2, d);
                p[t] = portals[portalOut, c];

                return r;
        }
    }
}

