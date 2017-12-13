using UnityEngine;
using StateStuff;

public class FirstState : State<AI_Player>
{
    private static FirstState _instance;
    public bool test;
    public Vector3 pos;
    private FirstState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static FirstState Instance
    {
        get
        {
            if (_instance == null)
            {
                new FirstState();
            }

            return _instance;
        }
    }

    public override void EnterState(AI_Player _owner)
    {
        Debug.Log("Entering First State");
        if (_owner.GetLeader() != null)
        {
            test = true;
            //pos = _owner.getBestSupportingSpot();
            pos = _owner.getGoodPos(_owner.GetLeader());
        }
    }

    public override void ExitState(AI_Player _owner)
    {
        Debug.Log("Exiting First State");
        _owner.amITheSupportingPlayer = false;
        if (_owner.tag == "BlueTeam")
        {
            _owner.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
        else
        {
            _owner.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
    }

    public override void UpdateState(AI_Player _owner)
    {
        _owner.RotateTheBall();
        Vector3 v = new Vector3(_owner.transform.position.x, _owner.transform.position.y + 5f, _owner.transform.position.z);
        Debug.DrawRay(_owner.transform.position, v - _owner.transform.position, Color.white);

        //_owner.transform.position = Vector3.MoveTowards (_owner.transform.position, GameConstants.centers [_owner.homeRegion], 1.0f);

        if (_owner.myTeamState == AI_Player.stateTeam.Defense)
        {
            _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, GameConstants.centers[_owner.homeRegion], 1.0f);
            _owner.amITheSupportingPlayer = false;
        }
        else
        {

            if (_owner.amITheSupportingPlayer)
            {
                /*if (!test)
                {*/
                //pos = _owner.getBestSupportingSpot();
                pos = _owner.getGoodPos(_owner.GetLeader());
                //}
               /* if (_owner.tag == "BlueTeam")
                {
                    _owner.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
                }
                else
                {
                    _owner.GetComponent<Renderer>().material.SetColor("_Color", Color.magenta);
                }*/
                _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, pos, 1f);
                Debug.DrawLine(pos, new Vector3(pos.x, pos.y + 25, pos.z), Color.blue);

            }
            else
            {
                if (_owner.tag == "BlueTeam")
                {
                    _owner.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                }
                else
                {
                    _owner.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                }
                _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, GameConstants.centers[_owner.homeRegionAttack], 1.0f);
            }
        }

        if (_owner.haveToChase())
        {
            _owner.stateMachine.ChangeState(ChaseBall.Instance);
        }

        if(_owner.myTeamState == AI_Player.stateTeam.Defense)
        {
           /* if (_owner.GetLeader() != null)
            {
                if ((_owner.GetLeader().transform.position.x > _owner.enemyGoal.transform.position.x && _owner.GetLeader().transform.position.x < _owner.ball.transform.position.x)
                    || (_owner.GetLeader().transform.position.x < _owner.enemyGoal.transform.position.x && _owner.GetLeader().transform.position.x > _owner.ball.transform.position.x))
                {
                    _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.startPos, 1f);
                }
            }*/
            
            if (_owner.myLover != null)
            {
                
               /* if ((_owner.myLover.transform.position.x > _owner.enemyGoal.transform.position.x && _owner.myLover.transform.position.x < _owner.ball.transform.position.x)
                    || (_owner.myLover.transform.position.x < _owner.enemyGoal.transform.position.x && _owner.myLover.transform.position.x > _owner.ball.transform.position.x))
                {
                    _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, new Vector3(_owner.ball.transform.position.x, _owner.myLover.transform.position.y, _owner.myLover.transform.position.z), 1f);
                }
                else
                {
                    _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.myLover.transform.position, 1f);
                }*/
                
            }
            
        }
        else
         {
            
            Vector3 v_flock;
             //ATTAQUE
            /* switch (_owner.GetLeader().myRole)
             {
                 case AI_Player.rolePlayer.BU:
                     v_flock = _owner.GetLeader().transform.forward.normalized;
                     if (_owner.myRole == AI_Player.rolePlayer.AG || _owner.myRole == AI_Player.rolePlayer.AD)
                     {
                        _owner.iWantTheBall = true;
                        /*if(_owner.team == 1)
                        {
                            v_flock += Vector3.right;
                        }
                        else
                        {
                            v_flock += Vector3.left;
                        }
                        // v_flock = _owner.transform.position + v_flock;
                        v_flock = new Vector3(v_flock.x, 0, v_flock.z);
                         Debug.DrawRay(_owner.transform.position, v_flock*10,Color.black);
                        Debug.Log("v_flock = " + v_flock);
                         _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.transform.position+v_flock, 1.5f);
                     }
                     break;
                 case AI_Player.rolePlayer.AG:
                     v_flock = _owner.GetLeader().transform.forward.normalized;
                     if (_owner.myRole == AI_Player.rolePlayer.BU || _owner.myRole == AI_Player.rolePlayer.AD)
                     {
                        _owner.iWantTheBall = true;
                        if (_owner.team == 1)
                         {
                             v_flock += Vector3.back;
                         }
                         else
                         {
                             v_flock += Vector3.forward;
                        }
                        v_flock = new Vector3(v_flock.x, 0, v_flock.z);
                        Debug.DrawRay(_owner.transform.position, v_flock*10, Color.black);
                         _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.transform.position + v_flock, 1.5f);
                     }
                     break;

                 case AI_Player.rolePlayer.AD:
                     v_flock = _owner.GetLeader().transform.forward.normalized;
                     if (_owner.myRole == AI_Player.rolePlayer.BU || _owner.myRole == AI_Player.rolePlayer.AG)
                     {
                        _owner.iWantTheBall = true;
                        if (_owner.team == 1)
                         {
                             v_flock += Vector3.forward;
                         }
                         else
                         {
                             v_flock += Vector3.back;
                        }
                        v_flock = new Vector3(v_flock.x, 0, v_flock.z);
                        Debug.DrawRay(_owner.transform.position, v_flock*10, Color.black);
                         _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.transform.position + v_flock, 1.5f);
                     }
                     break;

                    case AI_Player.rolePlayer.MCD:
                        v_flock = _owner.GetLeader().transform.position.normalized;
                        if (_owner.myRole == AI_Player.rolePlayer.MDF || _owner.myRole == AI_Player.rolePlayer.MCG)
                        {
                            if (_owner.team == 1)
                            {
                                v_flock += Vector3.back;
                            }
                            else
                            {
                                v_flock += Vector3.forward;
                            }
                            Debug.DrawRay(_owner.transform.position, v_flock * 10f, Color.black);
                            _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.transform.position + v_flock, 1f);
                        }
                        break;
                    case AI_Player.rolePlayer.MCG:
                        v_flock = _owner.GetLeader().transform.position.normalized;
                        if (_owner.myRole == AI_Player.rolePlayer.MDF || _owner.myRole == AI_Player.rolePlayer.MCD)
                        {
                            if (_owner.team == 1)
                            {
                                v_flock += Vector3.back;
                            }
                            else
                            {
                                v_flock += Vector3.forward;
                            }
                            Debug.DrawRay(_owner.transform.position, v_flock * 10f, Color.black);
                            _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.transform.position + v_flock, 1f);
                        }
                        break;

                    case AI_Player.rolePlayer.MDF:
                        v_flock = _owner.GetLeader().transform.position.normalized;
                        if (_owner.myRole == AI_Player.rolePlayer.MDF || _owner.myRole == AI_Player.rolePlayer.MCD)
                        {
                            if (_owner.team == 1)
                            {
                                v_flock += Vector3.right;
                            }
                            else
                            {
                                v_flock += Vector3.left;
                            }
                            Debug.DrawRay(_owner.transform.position, v_flock * 10f, Color.black);
                            _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.transform.position + v_flock, 1f);
                        }
                        break;
                 }*/


        }
    }
}
