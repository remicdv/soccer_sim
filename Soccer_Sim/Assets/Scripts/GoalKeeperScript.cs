using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;

public class GoalKeeperScript : MonoBehaviour {

	public GameObject ball;
	GameObject upGoalLimit;
	GameObject botGoalLimit;
	Vector3 upGoalPosition;
	Vector3 botGoalPosition;
	bool ballIsBottom;
	bool ballIsTop;
    public bool onBall = false;
	public FieldOfView fov;
	Vector3 beginningPos;
    public GameObject enemyGoal;
    public GameObject homeGoal;

    public StateMachine<GoalKeeperScript> stateMachine { get; set; }

	// Use this for initialization
	void Start () 
	{

		ball = GameObject.Find ("Ball");
		upGoalLimit = GameObject.Find ("UpGoalLimit");
		botGoalLimit = GameObject.Find ("BotGoalLimit");
		upGoalPosition = upGoalLimit.transform.position;
		botGoalPosition = botGoalLimit.transform.position;
		beginningPos = transform.position;

		ballIsBottom = false;
		ballIsTop = false;

		fov = GetComponent<FieldOfView> ();

		stateMachine = new StateMachine<GoalKeeperScript>(this);
		stateMachine.ChangeState(GoalKeeperFollowBall.Instance);
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		if (transform.position.z > upGoalPosition.z)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, upGoalPosition.z);
			GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
		}
		else if (transform.position.z < botGoalPosition.z)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, botGoalPosition.z);
			GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
		}


		stateMachine.Update ();

	}

	public void FollowBall()
	{
		if (ball.transform.position.z >= upGoalPosition.z)
		{
			ballIsTop = true;
		}
		else if (ball.transform.position.z <= botGoalPosition.z)
		{
			ballIsBottom = true;
		}

		if (transform.position.z <= upGoalPosition.z && transform.position.z >= botGoalPosition.z)
		{
			transform.position = Vector3.MoveTowards (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (beginningPos.x, beginningPos.y, ball.transform.position.z), 1.0f);
			ballIsBottom = false;
			ballIsTop = false;
		}
		else if (ballIsBottom && ball.transform.position.z >= botGoalPosition.z)
		{
			transform.position = Vector3.MoveTowards (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (beginningPos.x, beginningPos.y, ball.transform.position.z), 1.0f);
			ballIsBottom = false;
		}
		else if (ballIsTop && ball.transform.position.z <= upGoalPosition.z)
		{
			transform.position = Vector3.MoveTowards (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (beginningPos.x, beginningPos.y, ball.transform.position.z), 1.0f);
			ballIsTop = false;
		}
	}

	public void ToTheBall(float speed)
	{
		Vector3 to_ball = (ball.transform.position - transform.position).normalized * speed;
		GetComponent<Rigidbody> ().MovePosition (transform.position + to_ball * Time.deltaTime);

		var newRotation = Quaternion.LookRotation(ball.transform.position - transform.position, Vector3.up);
		newRotation.x = 0f;
		newRotation.z = 0f;
		transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 8);
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Ball")
        {
            onBall = true;
        }

    }
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.name == "Ball")
        {
            onBall = true;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name == "Ball")
        {
            onBall = false;
        }
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
}
