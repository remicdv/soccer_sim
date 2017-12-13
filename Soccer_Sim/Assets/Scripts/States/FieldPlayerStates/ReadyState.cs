using UnityEngine;
using StateStuff;

public class ReadyState : State<AI_Player>
{
    private static ReadyState _instance;
    public bool test;
    public Vector3 pos;
    private ReadyState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static ReadyState Instance
    {
        get
        {
            if (_instance == null)
            {
                new ReadyState();
            }

            return _instance;
        }
    }

    public override void EnterState(AI_Player _owner)
    {

    }

    public override void ExitState(AI_Player _owner)
    {

    }

    public override void UpdateState(AI_Player _owner)
    {

    }
}
