using Infrastructure.Services.Log.Core;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.Core;
using Infrastructure.StateMachine.Main.States.Core;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace Infrastructure.StateMachine.Game.States
{
    public class SetupApplicationState : IState, IGameState
    {
        private readonly IStateMachine<IGameState> _gameStateMachine;
        private readonly ILogService _logService;

        public SetupApplicationState(IStateMachine<IGameState> gameStateMachine, ILogService logService)
        {
            _gameStateMachine = gameStateMachine;
            _logService = logService;
        }

        public void Enter()
        {
            _logService.Log("SetupApplicationState");
            DisableSleepTimeout();
            _gameStateMachine.Enter<BootstrapAnalyticsState>();
        }

        private void DisableSleepTimeout() => Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}