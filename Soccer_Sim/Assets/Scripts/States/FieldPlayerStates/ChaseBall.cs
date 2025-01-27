﻿using UnityEngine;
using StateStuff;

public class ChaseBall : State<AI_Player>
{
    private static ChaseBall _instance;

    private ChaseBall()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static ChaseBall Instance
    {
        get
        {
            if (_instance == null)
            {
                new ChaseBall();
            }

            return _instance;
        }
    }

    public override void EnterState(AI_Player _owner)
    {
        //Debug.Log("Entering Chasing ball State");
        _owner.leader = true;
        if (_owner.amIReceiver)
        {
            _owner.setTeamState(AI_Player.stateTeam.Attack);
        }

        if (_owner.findClosestTeammate() != null)
        {
            AI_Player a = _owner.findClosestTeammate().GetComponent("AI_Player") as AI_Player;
            if(a != null)
            {
                _owner.findClosestTeammate().GetComponent<AI_Player>().amITheSupportingPlayer = true;
                _owner.UpdateSupport(_owner.findClosestTeammate());
            }
        }

    }

    public override void ExitState(AI_Player _owner)
    {
        //Debug.Log("Exiting Chasing ball State");
        _owner.setTeamState(AI_Player.stateTeam.Defense);
    }

    public override void UpdateState(AI_Player _owner)
    {
        bool t = false;
        if(_owner.team == 1)
        {
            if(_owner.ball.transform.position.x > 40)
            {
                t = true;
            }
        }
        else
        {
            if (_owner.ball.transform.position.x < -40)
            {
                t = true;
            }
        }
        if (!_owner.amIReceiver && !t)
        {
            _owner.setTeamState(AI_Player.stateTeam.Defense);
        }
        else
        {
            _owner.setTeamState(AI_Player.stateTeam.Attack);
        }

        //Debug.DrawRay(_owner.transform.position, _owner.transform.forward- _owner.transform.position, Color.red);
        if (!_owner.haveToChase())
        {
            _owner.stateMachine.ChangeState(FirstState.Instance);
            _owner.leader = false;
        }
        if (!_owner.isOnBall)
        {
            
            //_owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.ball.transform.position, 1f);
            /*Vector3 to_ball = (_owner.ball.transform.position - _owner.transform.position).normalized * 30f;
            _owner.myRigidbody.MovePosition(_owner.transform.position + to_ball * Time.deltaTime);
            var newRotation = Quaternion.LookRotation(_owner.ball.transform.position - _owner.transform.position, Vector3.up);
            newRotation.x = 0f;
            newRotation.z = 0f;
            _owner.transform.rotation = Quaternion.Slerp(_owner.transform.rotation, newRotation, Time.deltaTime * 8);*/
            _owner.ToTheBall(30f);
        }
        else
        {
            _owner.stateMachine.ChangeState(Dribbling.Instance);
           
        }
    }
}
