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

    public enum stateTeam { Defense, Attack }
    public enum rolePlayer { DG, DD, DCG, DCD, MDF, MCG, MCD, AD, AG, BU}

    public rolePlayer myRole;
    public stateTeam myTeamState;

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
    public GameObject myLover;

    public bool iWantTheBall;

    public GameObject ball;
    public GameObject receiver;
    public bool amIReceiver;
    public FieldOfView fov;
    public Rigidbody myRigidbody;
    public Vector3 startPos;
    public bool leader;
    public bool coroutine = false;
    FMOD.Studio.EventInstance run = new FMOD.Studio.EventInstance();
    public int homeRegion;
    public int homeRegionAttack;
    public bool amITheSupportingPlayer;


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
        iWantTheBall = false;
        amITheSupportingPlayer = false;
        if (team == 1)
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.blue);
            teammates = GameObject.FindGameObjectsWithTag("BlueTeam");
            enemies = GameObject.FindGameObjectsWithTag("RedTeam");
            homeRegionAttack = homeRegion + 5;
        }
        else
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.red);
            teammates = GameObject.FindGameObjectsWithTag("RedTeam");
            enemies = GameObject.FindGameObjectsWithTag("BlueTeam");
            homeRegionAttack = homeRegion - 5;
        }
        myLover = getMyLover();

        receiver = (GameObject)teammates.GetValue(1);


    }

    private void FixedUpdate()
    {
        if (Time.time > gameTimer + 1)
        {
            gameTimer = Time.time;
            seconds++;
        }

        if (seconds == 5)
        {
            seconds = 0;
            switchState = !switchState;
        }

        /* if (leader && (GetComponent<Rigidbody>().velocity.x > 0 || GetComponent<Rigidbody>().velocity.y > 0))
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
         }*/

        transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
        stateMachine.Update();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Ball")
        {
            isOnBall = true;
            foreach(GameObject g in teammates)
            {
                if(g.GetComponent<AI_Player>() != null)
                {
                    g.GetComponent<AI_Player>().amIReceiver = false;
                }
            }
        }


//        if ((col.gameObject.tag == "BlueTeam" || col.gameObject.tag == "RedTeam") && fov.findBall(ball))
//        {
//            FMOD.Studio.EventInstance colision = FMODUnity.RuntimeManager.CreateInstance(colisionSound);
//            colision.start();
//            colision.release();
//        }

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

    public void ToTheGoal()
    {
        var newRotation = Quaternion.LookRotation(enemyGoal.transform.position - transform.position, Vector3.up);
        newRotation.x = 0f;
        newRotation.z = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 1);
        Vector3 v = (enemyGoal.transform.position - transform.position).normalized;
        v = transform.position + v * 3;
        ball.transform.position = new Vector3(v.x, ball.transform.position.y, v.z);
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

    public GameObject getEnemyByRole(rolePlayer r)
    {
        GameObject leBon = null;
        foreach(GameObject g in enemies)
        {
            AI_Player a = g.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                if (a.myRole == r)
                {
                    Debug.Break();
                    leBon = g;
                    break;
                }
            }
        }
        return leBon;
    }

    public GameObject getMyLover()
    {
        GameObject lover = null;
        switch (myRole)
        {
            case rolePlayer.AD:
                lover = getEnemyByRole(rolePlayer.DCG);
                break;
            case rolePlayer.AG:
                lover = getEnemyByRole(rolePlayer.DD);
                break;
            case rolePlayer.BU:
                lover = getEnemyByRole(rolePlayer.DCD);
                break;
            case rolePlayer.MDF:
                lover = getEnemyByRole(rolePlayer.MCD);
                break;
            case rolePlayer.MCD:
                lover = getEnemyByRole(rolePlayer.DG);
                break;
            case rolePlayer.MCG:
                lover = getEnemyByRole(rolePlayer.MDF);
                break;
            case rolePlayer.DD:
                lover = getEnemyByRole(rolePlayer.AG);
                break;
            case rolePlayer.DG:
                lover = getEnemyByRole(rolePlayer.AD);
                break;
            case rolePlayer.DCG:
                lover = getEnemyByRole(rolePlayer.BU);
                break;
            case rolePlayer.DCD:
                lover = getEnemyByRole(rolePlayer.MCG);
                break;
        }
        return lover;
    }

    public void setTeamState(stateTeam s)
    {
        myTeamState = s;
        foreach(GameObject g in teammates)
        {
            AI_Player a = g.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                a.myTeamState = s;
            }
        }
    }
    
    public void setNotReceiver(GameObject[] tabs)
    {
        foreach(GameObject g in tabs)
        {
            AI_Player a = g.GetComponent("AI_Player") as AI_Player;
            if( a != null)
            {
                a.amIReceiver = false;
            }
        }
    }

    public List<GameObject> getPotentialReceiver()
    {
        List<GameObject> potRe = new List<GameObject>();
        foreach(GameObject g in teammates)
        {
            AI_Player a = g.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                if (a.iWantTheBall == true)
                {
                    potRe.Add(g);
                }

            }
        }
        return potRe;
    }

    public GameObject findReicever(List<GameObject> l)
    {
        float dist = 1000f;
        GameObject rec = null;
        foreach(GameObject g in l)
        {
            if (g.transform != transform)
            {
                if (Vector3.Distance(transform.position, g.transform.position) < dist)
                {
                    dist = Vector3.Distance(transform.position, g.transform.position);
                    rec = g;
                }
            }
        }
        return rec;
    }

    public void updateScore()
    {
        foreach(Vector3 key in GameConstants.centers)
        {
            GameConstants.centersDic[key] = 0f;
        }

        float dist = 0;
        Vector3 theKey = new Vector3();
        foreach (Vector3 key in GameConstants.centers)
        {
            RaycastHit hit;

            //Can Score
            Vector3 to_goal = (enemyGoal.transform.position - key).normalized;
            if (Physics.Raycast(key, to_goal, out hit))
            {
                if(hit.collider.gameObject.tag != enemies[0].tag)
                {
                    GameConstants.centersDic[key] += 1f;
                }
            }
            if(!Physics.Raycast(key, to_goal))
            {
                GameConstants.centersDic[key] += 1f;
            }

            //Can pass
            Vector3 to_ball = (ball.transform.position - key).normalized;
            if (Physics.Raycast(key, to_goal, out hit))
            {
                if (hit.collider.gameObject.tag != enemies[0].tag)
                {
                    GameConstants.centersDic[key] += 2f;
                }
            }
            if (!Physics.Raycast(key, to_goal))
            {
                GameConstants.centersDic[key] += 2f;
            }

            //Dist
            if(Vector3.Distance(key, ball.transform.position) > dist && Vector3.Distance(key, ball.transform.position) < 150)
            {
                dist = Vector3.Distance(key, ball.transform.position);
                theKey = key;
            }
        }
        GameConstants.centersDic[theKey] += 2f;
    }

    public Vector3 getBestSupportingSpot()
    {
        updateScore();
        float score = 0;
        Vector3 theKey = new Vector3();
        foreach (Vector3 key in GameConstants.centersDic.Keys)
        {
            if(gameObject.tag == "RedTeam")
            {
                if(key.x < GameObject.Find("FieldCenter").transform.position.x)
                {
                    if (GameConstants.centersDic[key] > score)
                    {
                        score = GameConstants.centersDic[key];
                        theKey = key;
                    }
                }
            }
            if (gameObject.tag == "BlueTeam")
            {
                if (key.x > GameObject.Find("FieldCenter").transform.position.x)
                {
                    if (GameConstants.centersDic[key] > score)
                    {
                        score = GameConstants.centersDic[key];
                        theKey = key;
                    }
                }
            }

        }
        return theKey;
    }

    public void UpdateSupport(GameObject g)
    {
        foreach(GameObject go in teammates)
        {
            if(g != go)
            {
                if(go.GetComponent<AI_Player>() != null)
                {
                    go.GetComponent<AI_Player>().amITheSupportingPlayer = false;
                }
            }
        }
    }
    
}
