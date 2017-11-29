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
	}
		
}