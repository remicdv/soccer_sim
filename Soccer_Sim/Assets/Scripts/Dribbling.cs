using UnityEngine;
using StateStuff;

public class Dribbling : State<AI_Player>
{
    private static Dribbling _instance;
    public bool right;

    private Dribbling()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static Dribbling Instance
    {
        get
        {
            if (_instance == null)
            {
                new Dribbling();
            }

            return _instance;
        }
    }

    public override void EnterState(AI_Player _owner)
    {
        Debug.Log("Entering Dribbling State");
        _owner.fov.FindVisibleInArray(_owner.enemies);
        if (_owner.fov.visibleTargets.Count != 0)
        {
            _owner.receiver = _owner.findClosestTeammate();
        }
        else
        {
            _owner.receiver = _owner.enemyGoal;
        }
        _owner.kickDir = _owner.receiver.transform.position - _owner.transform.position;
        Vector3 to_ball = _owner.ball.transform.position - _owner.transform.position;
        double dot = Vector3.Dot(_owner.kickDir.normalized, to_ball.normalized);
        //Debug.Log(dot);
        if (dot < -0.5f)
        {
            Debug.Log("right");
            right = true;
        }
        else
        {

            Debug.Log("Left");
            right = false;
        }
    }

    public override void ExitState(AI_Player _owner)
    {
        Debug.Log("Exiting Dribbling State");
    }

    public override void UpdateState(AI_Player _owner)
    {
        if(_owner.fov.visibleTargets.Count != 0)
        {
            //Debug.Log(_owner.teammates);
            _owner.receiver = _owner.findClosestTeammate();

            _owner.kickDir = (_owner.receiver.transform.position - _owner.transform.position).normalized;
            Vector3 to_ball = (_owner.ball.transform.position - _owner.transform.position).normalized;
            double dot = Vector3.Dot(_owner.kickDir, to_ball);
            //Debug.Log(dot);

            if (dot < 0)
            {
                Vector3 dribbleDir;

                if (right)
                {
                    //Debug.Log("Right!!!");
                    Debug.DrawRay(_owner.transform.position, _owner.kickDir * 10f, Color.blue);
                    dribbleDir = _owner.RotateY(to_ball, 90f);
                }
                else
                {
                    //Debug.Log("Left!!!");
                    dribbleDir = _owner.RotateY(to_ball, -90f);
                    Debug.DrawRay(_owner.transform.position, _owner.kickDir * 10f, Color.blue);
                }
                Debug.DrawRay(_owner.transform.position, dribbleDir*10f, Color.red);
                _owner.ball.GetComponent<Rigidbody>().velocity = dribbleDir * 50f;
            }
            else
            {
                AI_Player a = _owner.receiver.GetComponent("AI_Player") as AI_Player;
                a.amIReceiver = true;
                _owner.amIReceiver = false;
                _owner.stateMachine.ChangeState(KickBall.Instance);
            }
        }
        else
        {
            _owner.kickDir = (_owner.enemyGoal.transform.position - _owner.ball.transform.position).normalized;
            _owner.kickDir += _owner.AddNoiseOnAngle(0, 10);
            _owner.kickDir *= 20f;
            Debug.DrawRay(_owner.transform.position, _owner.kickDir, Color.black);
            _owner.amIReceiver = false;
            _owner.stateMachine.ChangeState(KickBall.Instance);
        }



    }
}
