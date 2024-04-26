using System;
using Infrastructure.Services.Log.Core;
using Infrastructure.Services.PersistentData.Core;
using Infrastructure.Services.SaveLoad.Core;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.Core;
using Infrastructure.StateMachine.Main.States.Core;

namespace Infrastructure.StateMachine.Game.States
{
    public class SaveDataState : IGameState, IPayloadedState<Action>
    {
        private const string Key = "Data";

        private readonly IPersistentDataService _persistentDataService;
        private readonly IStateMachine<IGameState> _gameStateMachine;
        private readonly ILogService _logService;
        private readonly ISaveLoadService _saveLoadService;

        public SaveDataState(IPersistentDataService persistentDataService,
            IStateMachine<IGameState> gameStateMachine, ILogService logService, ISaveLoadService saveLoadService)
        {
            _persistentDataService = persistentDataService;
            _gameStateMachine = gameStateMachine;
            _logService = logService;
            _saveLoadService = saveLoadService;
        }

        public void Enter(Action onComplete)
        {
            _logService.Log("SaveDataState");
            _saveLoadService.Save(Key, _persistentDataService.Data);
            _gameStateMachine.Enter<GameLoopState>();
            onComplete?.Invoke();
        }
    }
}