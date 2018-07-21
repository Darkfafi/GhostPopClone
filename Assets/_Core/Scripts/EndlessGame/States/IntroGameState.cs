using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroGameState : BaseGameState<EndlessGameWorld>
{
    private int _myNumber;

    protected override void OnActivated()
    {
        Debug.Log("B: " + _myNumber);
    }

    protected override void OnDeactivated(GameStateRawData rawData)
    {
        Debug.Log("C: " + _myNumber);

    }

    protected override void OnDeinitialize()
    {
        Debug.Log("D: " + _myNumber);

    }

    protected override void OnInitialized()
    {
        _myNumber = this.GetHashCode();
        Debug.Log("A: " + _myNumber);
    }
}
