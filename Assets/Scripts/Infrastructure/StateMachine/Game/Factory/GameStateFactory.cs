using System;
using System.Collections.Generic;
using Infrastructure.StateMachine.Game.States;
using Infrastructure.StateMachine.Main.States.Core;
using Infrastructure.StateMachine.Main.States.Factory;
using Zenject;

namespace Infrastructure.StateMachine.Game.Factory
{
    public class GameStateFactory : StateFactory
    {
        public GameStateFactory(DiContainer container) : base(container) { }

        protected override Dictionary<Type, Func<IBaseState>> BuildStatesMap() =>
            new Dictionary<Type, Func<IBaseState>>
            {
                //chained
                [typeof(BootstrapState)] = _container.Resolve<BootstrapState>,
                [typeof(LoadDataState)] = _container.Resolve<LoadDataState>,
                [typeof(SetupApplicationState)] = _container.Resolve<SetupApplicationState>,
                [typeof(BootstrapAnalyticsState)] = _container.Resolve<BootstrapAnalyticsState>,
                [typeof(FinalizeBootstrapState)] = _container.Resolve<FinalizeBootstrapState>,
                [typeof(GameLoopState)] = _container.Resolve<GameLoopState>,

                //other
                [typeof(ReloadState)] = _container.Resolve<ReloadState>,
                [typeof(SaveDataState)] = _container.Resolve<SaveDataState>,
                [typeof(LoadSceneAsyncState)] = _container.Resolve<LoadSceneAsyncState>,
                [typeof(LoadSceneWithTransitionAsyncState)] = _container.Resolve<LoadSceneWithTransitionAsyncState>
            };
    }
}