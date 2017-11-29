using UnityEngine;
using StateStuff;

public class Dribbling : State<AI_Player>
{
    private static Dribbling _instance;
    public bool right;
    public bool dir;
    public int timer;

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
        //Debug.Log(dot);
        dir = false;
        _owner.leader = true;
        _owner.ball.GetComponent<Rigidbody>().velocity *= 0.0f;
        _owner.GetComponent<Rigidbody>().velocity *= 0.0f;
        _owner.setTeamState(AI_Player.stateTeam.Attack);
        _owner.setNotReceiver(_owner.enemies);
        timer = 0;


    }

    public override void ExitState(AI_Player _owner)
    {
        Debug.Log("Exiting Dribbling State");
        _owner.leader = false;
        if (_owner.tag == "BlueTeam")
        {
            _owner.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
        else
        {
            _owner.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }

        _owner.setTeamState(AI_Player.stateTeam.Defense);

    }

    public override void UpdateState(AI_Player _owner)
    {
        timer++;
        _owner.ToTheBall(30f);
        _owner.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        _owner.fov.FindVisibleInArray(_owner.enemies);
        Vector3 to_ball = (_owner.ball.transform.position - _owner.transform.position).normalized;
        double dot = 0;
        if (_owner.fov.visibleTargets.Count != 0)
        {
            //Debug.Log(_owner.teammates);
            _owner.receiver = _owner.findClosestTeammate();

            if (_owner.receiver != null)
            {
                _owner.kickDir = (_owner.receiver.transform.position - _owner.ball.transform.position).normalized;

                dot = Vector3.Dot(_owner.kickDir, to_ball);
                //Debug.Log(dot);

                if (dot < 0)
                {
                    Vector3 dribbleDir;

                    if (!dir)
                    {
                        float angle = _owner.AngleDir(_owner.transform.forward, _owner.kickDir, _owner.transform.up);
                        if (angle == 1f)
                        {
                            //Debug.Log("right");
                            right = true;
                            Debug.DrawRay(_owner.transform.position, _owner.kickDir * 30f, Color.white);

                        }
                        else
                        {

                            //Debug.Log("Left");
                            Debug.DrawRay(_owner.transform.position, _owner.kickDir * 30f, Color.black);
                            right = false;
                        }
                        dir = true;
                    }
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
                    Debug.DrawRay(_owner.transform.position, dribbleDir * 10f, Color.red);
                    //_owner.ball.GetComponent<Rigidbody>().velocity = new Vector3(dribbleDir.x * 50f, 0.0f, dribbleDir.z * 50f);
                    Vector3 v = new Vector3(dribbleDir.x * 2, 0.0f, dribbleDir.z * 2);
                    _owner.ball.transform.position = _owner.ball.transform.position + v;


                }
                else
                {
                    dir = false;
                    AI_Player a = _owner.receiver.GetComponent("AI_Player") as AI_Player;
                    a.amIReceiver = true;
                    _owner.amIReceiver = false;
                    _owner.stateMachine.ChangeState(KickBall.Instance);
                }
            }
        }
        else
        {
            _owner.kickDir = (_owner.enemyGoal.transform.position - _owner.ball.transform.position).normalized;
            dot = Vector3.Dot(_owner.kickDir, to_ball);
            if (dot < 0)
            {
                Vector3 dribbleDir;

                if (!dir)
                {
                    float angle = _owner.AngleDir(_owner.transform.forward, _owner.kickDir, _owner.transform.up);
                    if (angle == 1f)
                    {
                        //Debug.Log("right");
                        right = true;
                    }
                    else
                    {

                        // Debug.Log("Left");
                        right = false;
                    }
                    dir = true;
                }
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
                Debug.DrawRay(_owner.transform.position, dribbleDir * 10f, Color.red);
                Vector3 v = new Vector3(dribbleDir.x * 2, 0.0f, dribbleDir.z * 2);
                _owner.ball.transform.position = _owner.ball.transform.position + v;
            }
            else
            {
                dir = false;
                float dToGoal = Vector3.Distance(_owner.enemyGoal.transform.position, _owner.ball.transform.position);

                if (dToGoal < 80f)
                {
                    _owner.kickDir += _owner.AddNoiseOnAngle(0, 10);
                    //_owner.kickDir *= 20f;
                    Debug.DrawRay(_owner.transform.position, _owner.kickDir, Color.black);
                    _owner.amIReceiver = false;
                    _owner.stateMachine.ChangeState(KickBall.Instance);
                    Debug.Log("Distance au but " + dToGoal);
                }
                else
                {
                    //Vector3 v = new Vector3(_owner.kickDir.x * 1f, 0.0f, _owner.kickDir.z * 1f);
                    //_owner.ball.transform.position = _owner.ball.transform.position + v;
                    //_owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.ball.transform.position + v, 1f);
                    //Debug.DrawRay(_owner.transform.position, _owner.kickDir * 10f, Color.red);
                    if(timer > 100)
                    {

                        _owner.receiver = _owner.findReicever(_owner.getPotentialReceiver());
                        _owner.kickDir = (_owner.receiver.transform.position - _owner.ball.transform.position).normalized;
                        Debug.Log("Receiver =====", _owner.receiver);
                        _owner.stateMachine.ChangeState(KickBall.Instance);
                    }
                    _owner.GetComponent<Rigidbody>().velocity *= 0;
                    _owner.ball.GetComponent<Rigidbody>().velocity *= 0;
                    Debug.Log("Je dribble");
                }

            }

        }



    }


}
