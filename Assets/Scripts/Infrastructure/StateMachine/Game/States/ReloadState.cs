using System;
using Infrastructure.Services.Log.Core;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.Core;
using Infrastructure.StateMachine.Main.States.Core;
using Infrastructure.Transition.Core;

namespace Infrastructure.StateMachine.Game.States
{
    public class ReloadState : IGameState, IState
    {
        private readonly IStateMachine<IGameState> _stateMachine;
        private readonly ILogService _logService;
        private readonly ITransitionScreen _transitionScreen;

        public ReloadState(IStateMachine<IGameState> stateMachine, ILogService logService, ITransitionScreen transitionScreen)
        {
            _stateMachine = stateMachine;
            _logService = logService;
            _transitionScreen = transitionScreen;
        }

        public void Enter()
        {
            _logService.Log("ReloadState");

            _transitionScreen.Show(() =>
            {
                _stateMachine.Enter<SaveDataState, Action>(() =>
                {
                    _stateMachine.Enter<BootstrapState>();
                    _transitionScreen.HideImmediately();
                });
            });
        }
    }
}