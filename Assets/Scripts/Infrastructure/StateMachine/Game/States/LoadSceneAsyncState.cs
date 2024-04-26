using System;
using Infrastructure.SceneManagement.Core;
using Infrastructure.Services.Log.Core;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.Core;
using Infrastructure.StateMachine.Main.States.Core;

namespace Infrastructure.StateMachine.Game.States
{
    public class LoadSceneAsyncState : IPayloadedState<LoadSceneAsyncState.Payload>, IGameState
    {
        private readonly IStateMachine<IGameState> _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly ILogService _logService;

        public LoadSceneAsyncState(IStateMachine<IGameState> gameStateMachine, ISceneLoader sceneLoader, ILogService logService)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _logService = logService;
        }

        public void Enter(Payload payload)
        {
            _logService.Log($"LoadSceneAsyncState: {payload.SceneName}");
            _sceneLoader.LoadAsync(payload.SceneName, () => OnLoadedScene(payload));
        }

        private void OnLoadedScene(Payload payload)
        {
            payload.OnComplete?.Invoke();
            _gameStateMachine.Enter<GameLoopState>();
        }

        public class Payload
        {
            public string SceneName;
            public Action OnComplete;
        }
    }
}