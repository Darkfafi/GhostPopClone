using UnityEngine;

public class RunGameState : BaseGameState<EndlessGameWorld>
{
    private int _myNumber;

    protected override void OnActivated()
    {
        Debug.Log("BB: " + _myNumber);
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
