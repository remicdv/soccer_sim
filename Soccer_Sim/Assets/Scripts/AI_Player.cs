using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;

public class AI_Player : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string kickSound;

    [FMODUnity.EventRef]
    public string colisionSound;

    [FMODUnity.EventRef]
    public string footStep;

    public bool switchState = false;
    public int team;
    public float gameTimer;
    public bool isOnBall = false;
    public int seconds = 0;
    public float kickForce = 10f;
    public Vector3 kickDir;
    public GameObject homeGoal;
    public GameObject enemyGoal;
    public Vector3 target;
    public GameObject[] teammates;
    public GameObject[] enemies;
    public GameObject ball;
    public GameObject receiver;
    public bool amIReceiver;
    public FieldOfView fov;
    public Rigidbody myRigidbody;
    public Vector3 startPos;
    public bool leader;
    public bool coroutine = false;
    FMOD.Studio.EventInstance run = new FMOD.Studio.EventInstance();


    public StateMachine<AI_Player> stateMachine { get; set; }

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        fov = gameObject.GetComponent("FieldOfView") as FieldOfView;
        ball = GameObject.Find("Ball");
        stateMachine = new StateMachine<AI_Player>(this);
        stateMachine.ChangeState(FirstState.Instance);
        amIReceiver = false;
        gameTimer = Time.time;
        startPos = transform.position;
        leader = false;
        if (team == 1)
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.blue);
            teammates = GameObject.FindGameObjectsWithTag("BlueTeam");
            enemies = GameObject.FindGameObjectsWithTag("RedTeam");
        }
        else
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.red);
            teammates = GameObject.FindGameObjectsWithTag("RedTeam");
            enemies = GameObject.FindGameObjectsWithTag("BlueTeam");
        }

        receiver = (GameObject)teammates.GetValue(1);


    }

    private void Update()
    {
        if (Time.time > gameTimer + 1)
        {
            gameTimer = Time.time;
            seconds++;
            //Debug.Log(seconds);
        }

        if (seconds == 5)
        {
            seconds = 0;
            switchState = !switchState;
        }
        
        if (leader && (GetComponent<Rigidbody>().velocity.x > 0 || GetComponent<Rigidbody>().velocity.y > 0))
        {
            FMOD.Studio.PLAYBACK_STATE playstate;
            run.getPlaybackState(out playstate);
            if (playstate == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                Debug.Log("creation");
                run = FMODUnity.RuntimeManager.CreateInstance(footStep);
                run.start();
            }
        }
        else
        {
            run.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        
        stateMachine.Update();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Ball")
        {
            isOnBall = true;
        }


        if ((col.gameObject.tag == "BlueTeam" || col.gameObject.tag == "RedTeam") && fov.findBall(ball))
        {
            FMOD.Studio.EventInstance colision = FMODUnity.RuntimeManager.CreateInstance(colisionSound);
            colision.start();
            colision.release();
        }

    }
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.name == "Ball")
        {
            isOnBall = true;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name == "Ball")
        {
            isOnBall = false;
        }
    }

    public GameObject findClosestTeammate()
    {
        GameObject closest = null;
        float dist = 1000f;
        foreach (GameObject g in teammates)
        {
            //Debug.Log(Vector3.Distance(g.transform.position, transform.position));

            if (g.transform != transform)
            {
                AI_Player a = g.GetComponent("AI_Player") as AI_Player;
                if(a != null)
                {
                    a.fov.FindVisibleInArray(a.enemies);
                    if (Vector3.Distance(g.transform.position, transform.position) < dist)
                    {
                        if (a.fov.visibleTargets.Count == 0)
                        {
                            dist = Vector3.Distance(g.transform.position, transform.position);
                            closest = g;
                        }
                        /*else
                        {
                            foreach(Transform t in a.fov.visibleTargets)
                            {
                                if((t.position.x > homeGoal.transform.position.x && t.position.x < transform.position.x)
                                    || (t.position.x < homeGoal.transform.position.x && t.position.x > transform.position.x))
                                {
                                    dist = Vector3.Distance(g.transform.position, transform.position);
                                    closest = g;
                                }
                            }
                        }*/
                    }
                }
                
            }
        }
        return closest;
    }

    public bool haveToChase()
    {
        float d1 = Vector3.Distance(transform.position, ball.transform.position);
        bool chase = true;
        if (!amIReceiver)
        {
            foreach (GameObject g in teammates)
            {
                if (g != gameObject)
                {
                    AI_Player t = g.GetComponent("AI_Player") as AI_Player;
                    if (t != null)
                    {
                        if (t.amIReceiver == true)
                        {
                            chase = false;
                            break;
                        }
                    }
                    float d2 = Vector3.Distance(g.transform.position, ball.transform.position);
                    if (d2 < d1)
                    {
                        chase = false;
                        break;
                    }
                }

            }
        }

        return chase;
    }

    public Vector3 RotateY(Vector3 v, float angle)
    {
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);

        float tx = v.x;
        float tz = v.z;
        v.x = (cos * tx) + (sin * tz);
        v.z = (cos * tz) - (sin * tx);
        return v;
    }

    public Vector3 AddNoiseOnAngle(float min, float max)
    {
        // Find random angle between min & max inclusive
        float xNoise = Random.Range(min, max);
        float yNoise = Random.Range(min, max);
        float zNoise = Random.Range(min, max);

        // Convert Angle to Vector3
        Vector3 noise = new Vector3(
          Mathf.Sin(2 * Mathf.PI * xNoise / 360),
          Mathf.Sin(2 * Mathf.PI * yNoise / 360),
          Mathf.Sin(2 * Mathf.PI * zNoise / 360)
        );
        return noise;
    }

    public void ToTheBall(float speed)
    {
        Vector3 to_ball = (ball.transform.position - transform.position).normalized * speed;
        myRigidbody.MovePosition(transform.position + to_ball * Time.deltaTime);
        var newRotation = Quaternion.LookRotation(ball.transform.position - transform.position, Vector3.up);
        newRotation.x = 0f;
        newRotation.z = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 8);
    }

    public AI_Player GetLeader()
    {
        AI_Player l = null;
        foreach (GameObject g in teammates)
        {
            AI_Player a = g.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                if (a.leader)
                {
                    l = a;
                    break;
                }
            }
        }

        return l;
    }

    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }
}
