using DebuggerOptions;
using Infrastructure.Coroutines.Runner;
using Infrastructure.Data.SaveLoad;
using Infrastructure.Observers.Screen;
using Infrastructure.SceneManagement;
using Infrastructure.Services.Framerate;
using Infrastructure.Services.ID;
using Infrastructure.Services.Log;
using Infrastructure.Services.PersistentData;
using Infrastructure.Services.SaveLoad;
using Infrastructure.Services.StaticData;
using Infrastructure.Services.StaticData.Core;
using Infrastructure.StateMachine.Game;
using Infrastructure.StateMachine.Game.Factory;
using Infrastructure.StateMachine.Game.States;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.Core;
using Infrastructure.Transition;
using UnityEngine;
using Zenject;

namespace Infrastructure.Zenject.Installers.ProjectContext.Bootstrap
{
    public class BootstrapInstaller : MonoInstaller, IInitializable
    {
        [Header("References")]
        [SerializeField] private GameObject _coroutineRunnerPrefab;
        [SerializeField] private GameObject _loadingCurtainPrefab;
        [SerializeField] private GameObject _transitionScreenPrefab;

        public override void InstallBindings()
        {
            BindMonoServices();
            BindSceneLoader();
            BindServices();
            BindScreenObserver();
            BindGameStateMachine();
            BindApplicationPauseDataSaver();
            InitializeDebugger();
            MakeInitializable();
        }

        public void Initialize() => BootstrapGame();

        private void BindMonoServices()
        {
            Container.BindInterfacesTo<CoroutineRunner>().FromComponentInNewPrefab(_coroutineRunnerPrefab).AsSingle();
            Container.BindInterfacesTo<LoadingScreen.LoadingScreen>().FromComponentInNewPrefab(_loadingCurtainPrefab).AsSingle();
            Container.BindInterfacesTo<TransitionScreen>().FromComponentInNewPrefab(_transitionScreenPrefab).AsSingle();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<IDService>().AsSingle();
            Container.BindInterfacesTo<LogService>().AsSingle();
            Container.BindInterfacesTo<StaticDataService>().AsSingle();
            Container.Resolve<IStaticDataService>().Load();
            Container.BindInterfacesTo<PersistentDataService>().AsSingle();
            Container.BindInterfacesTo<FramerateService>().AsSingle();
            Container.BindInterfacesTo<SaveLoadService>().AsSingle();
        }

        private void BindScreenObserver() => Container.BindInterfacesAndSelfTo<ScreenObserver>().AsSingle();

        private void BindSceneLoader() => Container.BindInterfacesTo<SceneLoader>().AsSingle();

        private void BindGameStateMachine()
        {
            BindGameStates();
            Container.Bind<GameStateFactory>().AsSingle();
            Container.BindInterfacesTo<GameStateMachine>().AsSingle();
        }

        private void BindGameStates()
        {
            //chained
            Container.Bind<BootstrapState>().AsSingle();
            Container.Bind<LoadDataState>().AsSingle();
            Container.Bind<SetupApplicationState>().AsSingle();
            Container.Bind<BootstrapAnalyticsState>().AsSingle();
            Container.Bind<FinalizeBootstrapState>().AsSingle();
            Container.Bind<GameLoopState>().AsSingle();

            //other
            Container.Bind<ReloadState>().AsSingle();
            Container.Bind<LoadSceneAsyncState>().AsSingle();
            Container.Bind<SaveDataState>().AsSingle();
            Container.Bind<LoadSceneWithTransitionAsyncState>().AsSingle();
        }

        private void BindApplicationPauseDataSaver() => Container.BindInterfacesTo<ApplicationPauseDataSaver>().AsSingle();

        private void BootstrapGame() => Container.Resolve<IStateMachine<IGameState>>().Enter<BootstrapState>();

        private void InitializeDebugger()
        {
            SRDebug.Init();
            SRDebug.Instance.AddOptionContainer(Container.Instantiate<GameOptions>());
        }

        private void MakeInitializable() => Container.Bind<IInitializable>().FromInstance(this).AsSingle();
    }
}