using UnityEngine;
using StateStuff;

public class Dribbling : State<AI_Player>
{
    private static Dribbling _instance;
    public bool right;
    public bool dir;
    public bool tothegoal;

    private float timeIntervalBeetweenGreatPassSpeech = 1.0f;
    private float timerForGreatPassSpeech = 0f;

    private float interval = 1.0f;
	private float time = 2.5f;

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
        //Debug.Log("Entering Dribbling State");
        if (_owner.isDribbler())
        {
            _owner.stateMachine.ChangeState(FirstState.Instance);
        }
        ////Debug.Log(dot);
        dir = false;
        _owner.ball.GetComponent<Rigidbody>().velocity *= 0.0f;
        _owner.GetComponent<Rigidbody>().velocity *= 0.0f;
        _owner.setTeamState(AI_Player.stateTeam.Attack);
        _owner.setNotReceiver(_owner.enemies);
        _owner.receiver = _owner.findClosestTeammate();
        if (_owner.receiver != null)
        {
            if(_owner.findMySupport(_owner.tag == "BlueTeam").GetComponent<AI_Player>() != null)
            {
                _owner.findMySupport(_owner.tag == "BlueTeam").GetComponent<AI_Player>().amITheSupportingPlayer = true;
                _owner.UpdateSupport(_owner.receiver);
            }
        }
        tothegoal = false;

    }

    public override void ExitState(AI_Player _owner)
    {
        //Debug.Log("Exiting Dribbling State");
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
		time += Time.deltaTime;
        timerForGreatPassSpeech += Time.deltaTime;

        _owner.setTeamState(AI_Player.stateTeam.Attack);
        _owner.leader = true;
        _owner.setNotReceiver(_owner.enemies);

        //If the player is dribbling to the goal follow the ball
        if (tothegoal)
        {
            _owner.ToTheBall(30f);

        }

        /*if (_owner.team == 1)
        {
            if (_owner.canIShoot())
            {
                if(!tothegoal)
                {
                    _owner.ToTheGoal();
                    tothegoal = true;
                    
                }
                else
                {

                    if (Vector3.Distance(_owner.enemyGoal.transform.position, _owner.ball.transform.position) > 185f)
                    {
                        _owner.kickDir = (_owner.enemyGoal.transform.position - _owner.ball.transform.position).normalized;
                        _owner.kickDir += _owner.AddNoiseOnAngle(0, 10);
                        _owner.kickDir = _owner.kickDir.normalized;
                        _owner.kickDir *= 30f;
                        _owner.amIReceiver = false;
                        _owner.stateMachine.ChangeState(KickBall.Instance);
                    }
                }
            }
        }*/
        //_owner.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        _owner.fov.FindVisibleInArray(_owner.enemies);
        
        bool control = false;
        if (_owner.team == 2)
        {
            if (Vector3.Distance(_owner.enemyGoal.transform.position, _owner.transform.position) > 300 || _owner.fov.visibleTargets.Count != 0)
            {
                control = true;
            }
        }
        else
        {
            if(_owner.fov.visibleTargets.Count != 0)
            {
                control = true;
            }
        }

        Vector3 to_ball = (_owner.ball.transform.position - _owner.transform.position).normalized;
        double dot = 0;
        if (control)
        {
            ////Debug.Log(_owner.teammates);
            if(_owner.team == 1)
            {
                _owner.receiver = _owner.findBestTeammateKick();
            }
            else
            {
                _owner.receiver = _owner.findBestTeammateControl();
            }

            if (_owner.receiver != null)
            {
                _owner.kickDir = (_owner.receiver.transform.position - _owner.ball.transform.position).normalized;

                dot = Vector3.Dot(_owner.kickDir, to_ball);
                ////Debug.Log(dot);

                if (dot < 0)
                {
                    Vector3 dribbleDir;

                    if (!dir)
                    {
                        float angle = _owner.AngleDir(_owner.transform.forward, _owner.kickDir, _owner.transform.up);
                        if (angle == 1f)
                        {
                            ////Debug.Log("right");
                            right = true;
                            Debug.DrawRay(_owner.transform.position, _owner.kickDir * 30f, Color.white);

                        }
                        else
                        {

                            ////Debug.Log("Left");
                            Debug.DrawRay(_owner.transform.position, _owner.kickDir * 30f, Color.black);
                            right = false;
                        }
                        dir = true;
                    }
                    if (right)
                    {
                        ////Debug.Log("Right!!!");
                        Debug.DrawRay(_owner.transform.position, _owner.kickDir * 10f, Color.blue);
                        dribbleDir = _owner.RotateY(to_ball, 90f);
                    }
                    else
                    {
                        ////Debug.Log("Left!!!");
                        dribbleDir = _owner.RotateY(to_ball, -90f);
                        Debug.DrawRay(_owner.transform.position, _owner.kickDir * 10f, Color.blue);
                    }
                    Debug.DrawRay(_owner.transform.position, dribbleDir * 10f, Color.red);
                    _owner.ball.GetComponent<Rigidbody>().velocity = new Vector3(dribbleDir.x * 50f, 0.0f, dribbleDir.z * 50f);
                    Vector3 v = new Vector3(dribbleDir.x * 1.5f, 0.0f, dribbleDir.z * 1.5f);


                }
                else
                {
                    dir = false;
                    AI_Player a = _owner.receiver.GetComponent("AI_Player") as AI_Player;
                    if(a != null)
                    {
                        // Play a "C'est une belle passe" speech 1 time out of 4
                        float randomDraw = Random.Range(0, 100);
                        if (randomDraw <= 25 && timerForGreatPassSpeech > timeIntervalBeetweenGreatPassSpeech)
                        {
                            _owner.greatPassSpeechEvent.start();
                            timerForGreatPassSpeech = 0;
                        } 

                        a.amIReceiver = true;
                        _owner.amIReceiver = false;
                        _owner.stateMachine.ChangeState(KickBall.Instance);

                    }
                }
            }
            else
            {
                //Debug.Log("null "+_owner.team);
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
                        ////Debug.Log("right");
                        right = true;
                    }
                    else
                    {

                        // //Debug.Log("Left");
                        right = false;
                    }
                    dir = true;
                }
                if (right)
                {
                    ////Debug.Log("Right!!!");
                    Debug.DrawRay(_owner.transform.position, _owner.kickDir * 10f, Color.blue);
                    dribbleDir = _owner.RotateY(to_ball, 90f);
                }
                else
                {
                    ////Debug.Log("Left!!!");
                    dribbleDir = _owner.RotateY(to_ball, -90f);
                    Debug.DrawRay(_owner.transform.position, _owner.kickDir * 10f, Color.blue);
                }
                Debug.DrawRay(_owner.transform.position, dribbleDir * 10f, Color.red);
                Vector3 v = new Vector3(dribbleDir.x * 1.5f, 0.0f, dribbleDir.z * 1.5f);
                _owner.ball.GetComponent<Rigidbody>().velocity = new Vector3(dribbleDir.x * 50f, 0.0f, dribbleDir.z * 50f);
            }
            else
            {
                dir = false;
                float dToGoal = Vector3.Distance(_owner.enemyGoal.transform.position, _owner.ball.transform.position);
                float distBlue;
                if(_owner.team == 1)
                {
                    distBlue = 175f;
                }
                else
                {
                    distBlue = 150f;
                }
                if (dToGoal < distBlue && !tothegoal)
                {
                    _owner.kickDir += _owner.AddNoiseOnAngle(0, 5);
                    _owner.kickDir -= _owner.AddNoiseOnAngle(0, 2);
                    _owner.kickDir *= 1.1f;
                    Debug.DrawRay(_owner.transform.position, _owner.kickDir, Color.black);
                    _owner.amIReceiver = false;

                    //Audio play
                    
                    if (time > interval) {
                        if(GameObject.Find("World").GetComponent<WorldController>().crowdExcitationRatio <= 0.95f)
                            GameObject.Find("World").GetComponent<WorldController>().crowdExcitationRatio += 0.05f;
                        FMOD.Studio.EventInstance shot = FMODUnity.RuntimeManager.CreateInstance (_owner.le_tir);
						shot.start ();
						shot.release ();

						float rand = Random.Range (0, 100);
						if (rand < 50) {
							shot = FMODUnity.RuntimeManager.CreateInstance (_owner.near_miss);
							shot.start ();
							shot.release ();
						}

						time = 0.0f;
					}
                    _owner.stateMachine.ChangeState(KickBall.Instance);
                }
                else
                {
                    if (!tothegoal)
                    {
                        _owner.ToTheGoal();
                        tothegoal = true;
                    }
                    Vector3 v = new Vector3(_owner.kickDir.x*50, 0.0f, _owner.kickDir.z*50);
                    _owner.ball.GetComponent<Rigidbody>().velocity = v;
                    _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.ball.transform.position + v, 1f);
                    Debug.DrawRay(_owner.transform.position, _owner.kickDir * 10f, Color.red);
                    
                    //Debug.Log("Je dribble");
                }

            }

        }



    }


}
