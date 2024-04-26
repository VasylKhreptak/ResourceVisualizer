using System;
using Infrastructure.StateMachine.Game.States;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.Core;
using UniRx;
using Zenject;

namespace Infrastructure.Data.SaveLoad
{
    public class ApplicationPauseDataSaver : IInitializable, IDisposable
    {
        private readonly IStateMachine<IGameState> _gameStateMachine;

        public ApplicationPauseDataSaver(IStateMachine<IGameState> gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        private IDisposable _subscription;

        public void Initialize() => StartObserving();

        public void Dispose()
        {
            StopObserving();
            SaveData();
        }

        private void StartObserving() => _subscription = Observable.EveryApplicationPause().Where(x => x).Subscribe(_ => OnApplicationPaused());

        private void StopObserving() => _subscription?.Dispose();

        private void OnApplicationPaused() => SaveData();

        private void SaveData() => _gameStateMachine.Enter<SaveDataState, Action>(null);
    }
}