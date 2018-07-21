using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField]
    private EndlessGameWorld _gameWorld;

    private Game<EndlessGameWorld, IntroGameState> _game;

    protected void Awake()
    {
        _game = new Game<EndlessGameWorld, IntroGameState>(_gameWorld, 
            new GameStateConstructor<EndlessGameWorld>().AddSwitcher<IntroEndRunSwitcher>()
        );

        _game.AddGlobalStateSwitcher<GlobalPreviousStateSwitcher>();
    }

    protected void OnDestroy()
    {
        _game.CleanGame();
    }
}
