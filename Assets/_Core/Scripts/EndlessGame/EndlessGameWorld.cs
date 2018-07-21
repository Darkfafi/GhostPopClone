using UnityEngine;
using GameStateSystem;

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
