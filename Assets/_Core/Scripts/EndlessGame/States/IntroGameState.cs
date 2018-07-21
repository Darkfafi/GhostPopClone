using System;
using UnityEngine;
using GameStateSystem;

public class IntroGameState : BaseGameState<EndlessGameWorld>
{
    public event Action EndOfIntroEvent;

    private int _myNumber;

    protected override void OnActivated(StateParameters parameters)
    {
        Debug.Log("B: " + _myNumber);
        GameWorld.UserInput.KeyPressedEvent += OnKeyPressedEvent;
    }

    protected override void OnDeactivated()
    {
        Debug.Log("C: " + _myNumber);
        GameWorld.UserInput.KeyPressedEvent -= OnKeyPressedEvent;

    }

    private void OnKeyPressedEvent(string keyID)
    {
        if(keyID == UserInputNotifier.ACTION)
        {
            if(EndOfIntroEvent != null)
            {
                EndOfIntroEvent();
            }
        }
    }

    protected override void OnDeinitialize()
    {
        Debug.Log("D: " + _myNumber);

    }

    protected override void OnInitialized()
    {
        _myNumber = (int)(UnityEngine.Random.value * 100);
        Debug.Log("A: " + _myNumber);
    }
}


public class MyDataParameter : IStateParameter
{
    public int MyValue;

    public MyDataParameter(int v)
    {
        MyValue = v;
    }
}