using UnityEngine;
using GameStateSystem;

public class RunGameState : BaseGameState<EndlessGameWorld>
{
    private int _myNumber;

    protected override void OnActivated(StateParameters parameters)
    {
        MyDataParameter data;
        parameters.TryGetStateParameter(out data, new MyDataParameter(Random.Range(1337, 2674)));

        Debug.Log("BB: " + _myNumber + " << " + data.MyValue);
    }

    protected override void OnDeactivated()
    {
        Debug.Log("CB: " + _myNumber);

    }

    protected override void OnDeinitialize()
    {
        Debug.Log("DB: " + _myNumber);

    }

    protected override void OnInitialized()
    {
        _myNumber = this.GetHashCode();
        Debug.Log("AB: " + _myNumber);
    }
}
