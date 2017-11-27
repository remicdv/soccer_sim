using UnityEngine;
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
        Debug.Log("Entering Chasing ball State");
        _owner.leader = true;

    }

    public override void ExitState(AI_Player _owner)
    {
        Debug.Log("Exiting Chasing ball State");
        _owner.leader = false;
    }

    public override void UpdateState(AI_Player _owner)
    {
        
        
        //Debug.DrawRay(_owner.transform.position, _owner.transform.forward- _owner.transform.position, Color.red);
        if (!_owner.haveToChase())
        {
            _owner.stateMachine.ChangeState(FirstState.Instance);
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
