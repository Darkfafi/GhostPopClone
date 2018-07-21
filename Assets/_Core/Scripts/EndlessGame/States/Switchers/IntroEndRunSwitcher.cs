using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Game.SwitchGameState<RunGameState>();
    }
}
