using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;

public class GoalKeeperScript : MonoBehaviour {

	GameObject ball;
	GameObject upGoalLimit;
	GameObject botGoalLimit;
	Vector3 upGoalPosition;
	Vector3 botGoalPosition;
	bool ballIsBottom;
	bool ballIsTop;
	FieldOfView fov;
	Vector3 beginningPos;

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

		if(fov.findBall(ball))
		{
			stateMachine.ChangeState (GoalKeeperChaseBall.Instance);
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
}
