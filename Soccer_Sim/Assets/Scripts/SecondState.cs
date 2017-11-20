using UnityEngine;
using StateStuff;

public class SecondState : State<AI_Player>
{
    private static SecondState _instance;

    private SecondState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static SecondState Instance
    {
        get
        {
            if (_instance == null)
            {
                new SecondState();
            }

            return _instance;
        }
    }

    public override void EnterState(AI_Player _owner)
    {
        Debug.Log("Entering Second State");
    }

    public override void ExitState(AI_Player _owner)
    {
        Debug.Log("Exiting Second State");
    }

    public override void UpdateState(AI_Player _owner)
    {
        if (!_owner.switchState)
        {
            _owner.stateMachine.ChangeState(FirstState.Instance);
        }
    }
}
