using GameStateSystem;

public class IntroEndRunSwitcher : BaseStateSwitcher<EndlessGameWorld, IntroGameState>
{
    protected override void OnActivate(IGame<EndlessGameWorld> game, IntroGameState gameStateInstance)
    {
        gameStateInstance.EndOfIntroEvent += OnEndOfIntroEvent;
    }

    protected override void OnDeactivate(IntroGameState gameStateInstance)
    {
        gameStateInstance.EndOfIntroEvent -= OnEndOfIntroEvent;
    }

    private void OnEndOfIntroEvent()
    {
        Game.SwitchGameState<RunGameState>(GameStateConstructor<EndlessGameWorld>.Create().SetStateParameters(new MyDataParameter(676)));
    }
}
