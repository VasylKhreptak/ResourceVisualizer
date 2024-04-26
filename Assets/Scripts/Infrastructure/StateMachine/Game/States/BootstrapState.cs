using Infrastructure.LoadingScreen.Core;
using Infrastructure.SceneManagement.Core;
using Infrastructure.Services.Log.Core;
using Infrastructure.Services.StaticData.Core;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.Core;
using Infrastructure.StateMachine.Main.States.Core;

namespace Infrastructure.StateMachine.Game.States
{
    public class BootstrapState : IState, IGameState
    {
        private readonly IStateMachine<IGameState> _stateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly IStaticDataService _staticDataService;
        private readonly ILoadingScreen _loadingScreen;
        private readonly ILogService _logService;

        public BootstrapState(IStateMachine<IGameState> stateMachine, ISceneLoader sceneLoader, IStaticDataService staticDataService,
            ILoadingScreen loadingScreen, ILogService logService)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _staticDataService = staticDataService;
            _loadingScreen = loadingScreen;
            _logService = logService;
        }

        public void Enter()
        {
            _logService.Log("BootstrapState");
            _loadingScreen.Show();
            _sceneLoader.LoadAsync(_staticDataService.Config.BootstrapScene, OnLoadedScene);
        }

        private void OnLoadedScene() => _stateMachine.Enter<LoadDataState>();
    }
}