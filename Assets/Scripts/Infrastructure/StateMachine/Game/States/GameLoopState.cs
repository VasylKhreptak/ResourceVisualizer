using Infrastructure.Services.Log.Core;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.States.Core;

namespace Infrastructure.StateMachine.Game.States
{
    public class GameLoopState : IState, IGameState
    {
        private readonly ILogService _logService;

        public GameLoopState(ILogService logService)
        {
            _logService = logService;
        }

        public void Enter() => _logService.Log("GameLoopState");
    }
}