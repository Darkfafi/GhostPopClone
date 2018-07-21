using System;
using UnityEngine;

public class UserInputNotifier : MonoBehaviour
{
    public const string LEFT = "LEFT";
    public const string RIGHT = "RIGHT";
    public const string ACTION = "ACTION";

    public event Action<string> KeyPressedEvent;

	protected void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            if(KeyPressedEvent != null)
            {
                KeyPressedEvent(LEFT);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (KeyPressedEvent != null)
            {
                KeyPressedEvent(RIGHT);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (KeyPressedEvent != null)
            {
                KeyPressedEvent(ACTION);
            }
        }
    }
}
