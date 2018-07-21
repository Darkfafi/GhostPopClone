using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessGameWorld : MonoBehaviour, IGameWorld
{
    public UserInputNotifier UserInput
    {
        get
        {
            return _userInput;
        }
    }

    [SerializeField]
    private UserInputNotifier _userInput;
}
