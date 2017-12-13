using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;

public class GoalKeeperFollowBall : State<GoalKeeperScript> {

	private static GoalKeeperFollowBall _instance;

	private GoalKeeperFollowBall()
	{
		if (_instance != null)
		{
			return;
		}

		_instance = this;
	}

	public static GoalKeeperFollowBall Instance
	{
		get
		{
			if (_instance == null)
			{
				new GoalKeeperFollowBall();
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
		_owner.FollowBall ();

        if (_owner.fov.findBall(_owner.ball))
        {
            _owner.stateMachine.ChangeState(GoalKeeperChaseBall.Instance);
        }
    }
}
