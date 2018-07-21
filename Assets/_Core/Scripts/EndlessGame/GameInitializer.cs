using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField]
    private EndlessGameWorld _gameWorld;

    private Game<EndlessGameWorld, IntroGameState> _game;

    protected void Awake()
    {
        _game = new Game<EndlessGameWorld, IntroGameState>(_gameWorld);
    }

    protected void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _game.ResetGame();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _game.SwitchGameState<IntroGameState>();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _game.SwitchGameState<RunGameState>();
        }
    }

    protected void OnDestroy()
    {
        _game.CleanGame();
    }
}
