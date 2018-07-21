using System;

public abstract class BaseGlobalStateSwitcher<T> where T : class, IGameWorld
{
    public IGame<T> Game { get; private set; }

    public void Activate(IGame<T> game)
    {
        if (Game != null)
            return;

        Game = game;
        OnActivate(Game);
    }

    public void Deactivate()
    {
        if (Game == null)
            return;

        OnDeactivate();

        Game = null;
    }

    protected abstract void OnActivate(IGame<T> game);
    protected abstract void OnDeactivate();
}

public abstract class BaseStateSwitcher<T, U> : BaseStateSwitcher<T> where T : class, IGameWorld where U : BaseGameState<T>
{
    public new U GameState { get; private set; }

    public void Activate(IGame<T> game, U stateInstance)
    {
        Activate(game, (BaseGameState<T>)stateInstance);
    }

    public void Deactivate(U stateInstance)
    {
        Deactivate((BaseGameState<T>)stateInstance);
    }

    protected override void OnActivate(IGame<T> game, BaseGameState<T> gameStateInstance)
    {
        OnActivate(game, (U)gameStateInstance);
    }

    protected override void OnDeactivate(BaseGameState<T> gameStateInstance)
    {
        OnDeactivate((U)gameStateInstance);
    }

    protected abstract void OnActivate(IGame<T> game, U gameStateInstance);
    protected abstract void OnDeactivate(U gameStateInstance);
}

public abstract class BaseStateSwitcher<T> where T : class, IGameWorld
{
    public IGame<T> Game { get; private set; }
    public BaseGameState<T> GameState { get; private set; }

    public void Activate(IGame<T> game, BaseGameState<T> stateInstance)
    {
        if (Game != null)
            return;

        Game = game;
        OnActivate(Game, stateInstance);
    }

    public void Deactivate(BaseGameState<T> stateInstance)
    {
        if (Game == null)
            return;

        OnDeactivate(stateInstance);

        Game = null;
    }

    protected abstract void OnActivate(IGame<T> game, BaseGameState<T> gameStateInstance);
    protected abstract void OnDeactivate(BaseGameState<T> stateInstance);
}