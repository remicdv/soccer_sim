using UnityEngine;
using StateStuff;

public class KickBall : State<AI_Player>
{
    private static KickBall _instance;

    private KickBall()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static KickBall Instance
    {
        get
        {
            if (_instance == null)
            {
                new KickBall();
            }

            return _instance;
        }
    }

    public override void EnterState(AI_Player _owner)
    {
        Debug.Log("Entering Kick Ball State");
    }

    public override void ExitState(AI_Player _owner)
    {
        Debug.Log("Exiting Kick Ball State");
    }

    public override void UpdateState(AI_Player _owner)
    {
        if (_owner.isOnBall)
        {
            Debug.Log(_owner.receiver.transform.position - _owner.transform.position);

            _owner.ball.GetComponent<Rigidbody>().velocity = _owner.kickDir * 200f *Time.deltaTime;

        }
        else
        {
            _owner.stateMachine.ChangeState(FirstState.Instance);
        }
    }
}
