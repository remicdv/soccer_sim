using UnityEngine;
using StateStuff;

public class FirstState : State<AI_Player>
{
    private static FirstState _instance;

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
    }

    public override void ExitState(AI_Player _owner)
    {
        Debug.Log("Exiting First State");
    }

    public override void UpdateState(AI_Player _owner)
    {
        Vector3 v = new Vector3(_owner.transform.position.x, _owner.transform.position.y + 5f, _owner.transform.position.z);
        Debug.DrawRay(_owner.transform.position, v - _owner.transform.position, Color.white);
        if (_owner.haveToChase())
        {
            _owner.stateMachine.ChangeState(ChaseBall.Instance);
        }
    }
}
