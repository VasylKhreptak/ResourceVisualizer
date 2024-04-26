using System;
using System.Collections.Generic;
using Infrastructure.StateMachine.Main.States.Core;
using Infrastructure.StateMachine.Main.States.Factory.Core;
using Zenject;

namespace Infrastructure.StateMachine.Main.States.Factory
{
    public abstract class StateFactory : IStateFactory, IFactory<Type, IBaseState>
    {
        private readonly Dictionary<Type, Func<IBaseState>> _statesMap;

        protected readonly DiContainer _container;

        protected StateFactory(DiContainer container)
        {
            _container = container;
            _statesMap = BuildStatesMap();
        }

        protected abstract Dictionary<Type, Func<IBaseState>> BuildStatesMap();

        public T GetState<T>() where T : class, IBaseState => Create(typeof(T)) as T;

        public IBaseState Create(Type type)
        {
            if (_statesMap.TryGetValue(type, out Func<IBaseState> state))
                return state();

            throw new Exception($"State for {type.Name} can't be created");
        }
    }
}