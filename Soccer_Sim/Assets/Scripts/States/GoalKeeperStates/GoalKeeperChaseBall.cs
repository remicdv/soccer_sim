using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;

public class GoalKeeperChaseBall : State<GoalKeeperScript> 
{

	private static GoalKeeperChaseBall _instance;

	private GoalKeeperChaseBall()
	{
		if (_instance != null)
		{
			return;
		}

		_instance = this;
	}

	public static GoalKeeperChaseBall Instance
	{
		get
		{
			if (_instance == null)
			{
				new GoalKeeperChaseBall();
			}

			return _instance;
		}
	}

	public override void EnterState(GoalKeeperScript _owner)
	{
		Debug.Log("Entering GoalKeeper Follow Ball State");

	}

	public override void ExitState(GoalKeeperScript _owner)
	{
		Debug.Log("Exiting GoalKeeper Follow Ball State");
	}

	public override void UpdateState(GoalKeeperScript _owner)
	{
		_owner.ToTheBall (30.0f);
        if(Vector3.Distance(GameObject.Find("Ball").transform.position, _owner.transform.position) > 50f ||
            Vector3.Distance(_owner.transform.position, _owner.homeGoal.transform.position) < 30f)
        {
            _owner.stateMachine.ChangeState(GoalKeeperFollowBall.Instance);
        }
        if (Vector3.Distance(_owner.ball.transform.position, _owner.transform.position) < 10f)
        {
            Vector3 kickDir = (_owner.enemyGoal.transform.position - _owner.ball.transform.position).normalized;
            //kickDir = _owner.AddNoiseOnAngle(0, 15f);
            _owner.ball.GetComponent<Rigidbody>().velocity = new Vector3(kickDir.x * 150f, 0.0f, kickDir.z * 150f);
            _owner.stateMachine.ChangeState(GoalKeeperFollowBall.Instance);
        }
    }
		
}