using UnityEngine;
using StateStuff;

public class BeginState : State<AI_Player>
{
    private static BeginState _instance;
    public bool test;
    public Vector3 pos;
    private BeginState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static BeginState Instance
    {
        get
        {
            if (_instance == null)
            {
                new BeginState();
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
        _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, GameConstants.centers[_owner.homeRegion], 1.0f);
        Vector3 v1 = new Vector3(_owner.transform.position.x, 0.0f, _owner.transform.position.z);
        Vector3 v2 = new Vector3(GameConstants.centers[_owner.homeRegion].x, 0.0f, GameConstants.centers[_owner.homeRegion].z);

        if (Vector3.Distance(v1, v2) < 1)
        {
            if(!_owner.finish)
                _owner.ready = true;
        }

    }
}
