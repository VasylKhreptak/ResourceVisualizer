using Infrastructure.Services.Log.Core;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.Core;
using Infrastructure.StateMachine.Main.States.Core;
using Infrastructure.Transition.Core;

namespace Infrastructure.StateMachine.Game.States
{
    public class LoadSceneWithTransitionAsyncState : IGameState, IPayloadedState<LoadSceneAsyncState.Payload>
    {
        private readonly IStateMachine<IGameState> _stateMachine;
        private readonly ITransitionScreen _transitionScreen;
        private readonly ILogService _logService;

        public LoadSceneWithTransitionAsyncState(IStateMachine<IGameState> stateMachine, ITransitionScreen transitionScreen,
            ILogService logService)
        {
            _stateMachine = stateMachine;
            _transitionScreen = transitionScreen;
            _logService = logService;
        }

        public void Enter(LoadSceneAsyncState.Payload payload)
        {
            _logService.Log($"LoadSceneWithTransitionAsyncState: {payload.SceneName}");
            _transitionScreen.Show(() =>
            {
                payload.OnComplete += () => _transitionScreen.Hide();

                _stateMachine.Enter<LoadSceneAsyncState, LoadSceneAsyncState.Payload>(payload);
            });
        }
    }
}