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

	public int team;

	[HideInInspector] public bool switchState = false;
    	[HideInInspector] public float gameTimer;
    	[HideInInspector] public bool isOnBall = false;
    	[HideInInspector] public int seconds = 0;
    	[HideInInspector] public float kickForce = 10f;
    	[HideInInspector] public Vector3 kickDir;
    	[HideInInspector] public GameObject homeGoal;
    	[HideInInspector] public GameObject enemyGoal;
    	[HideInInspector] public Vector3 target;
    	[HideInInspector] public GameObject[] teammates;
    	[HideInInspector] public GameObject[] enemies;
    	[HideInInspector] public GameObject ball;
    	[HideInInspector] public GameObject receiver;
    	[HideInInspector] public bool amIReceiver;
    	[HideInInspector] public FieldOfView fov;
    	[HideInInspector] public Rigidbody myRigidbody;
	public int homeRegion;


	[HideInInspector] public StateMachine<AI_Player> stateMachine { get; set; }

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        fov = gameObject.GetComponent("FieldOfView") as FieldOfView;
        ball = GameObject.Find("Ball");
        stateMachine = new StateMachine<AI_Player>(this);
        stateMachine.ChangeState(FirstState.Instance);
        amIReceiver = false;
        gameTimer = Time.time;
        if(team == 1)
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

        stateMachine.Update();
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.name == "Ball")
        {
            isOnBall = true;
            /*GameObject ball = GameObject.Find("Ball");

            Vector3 player_to_ball = ball.transform.position - transform.position;
            print(player_to_ball.normalized);

            if (Input.GetKey(KeyCode.A))
            {
                ball.GetComponent<Rigidbody>().velocity = player_to_ball * 10f;
            }*/
        }
    }

	void OnCollisionEnter(Collision col)
	{
		if ((col.gameObject.tag == "BlueTeam" || col.gameObject.tag == "RedTeam") && fov.findBall(ball))
		{
			FMOD.Studio.EventInstance colision = FMODUnity.RuntimeManager.CreateInstance (colisionSound);
			colision.start ();
			colision.release ();
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
                if (Vector3.Distance(g.transform.position, transform.position) < dist)
                {
                    dist = Vector3.Distance(g.transform.position, transform.position);
                    closest = g;
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
                    if (t!=null)
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

    public void ToTheBall()
    {
        Vector3 to_ball = (ball.transform.position - transform.position).normalized * 30f;
        myRigidbody.MovePosition(transform.position + to_ball * Time.deltaTime * 2.0f);
        var newRotation = Quaternion.LookRotation(ball.transform.position - transform.position, Vector3.up);
        newRotation.x = 0f;
        newRotation.z = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 8);
    }
}
