public class GlobalPreviousStateSwitcher : BaseGlobalStateSwitcher<EndlessGameWorld>
{
    protected override void OnActivate(IGame<EndlessGameWorld> game)
    {
        game.GetGameWorld().UserInput.KeyPressedEvent += OnKeyPressedEvent;
    }

    protected override void OnDeactivate()
    {
        Game.GetGameWorld().UserInput.KeyPressedEvent -= OnKeyPressedEvent;
    }

    private void OnKeyPressedEvent(string keyID)
    {
        if(keyID == UserInputNotifier.LEFT)
        {
            Game.SwitchToPreviousGameState();
        }
    }
}
