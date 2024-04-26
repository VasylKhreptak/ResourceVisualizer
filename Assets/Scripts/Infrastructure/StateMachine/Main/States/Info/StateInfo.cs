using System;
using Infrastructure.StateMachine.Main.States.Core;
using Infrastructure.StateMachine.Main.States.Info.Core;
using Zenject;

namespace Infrastructure.StateMachine.Main.States.Info
{
    public class StateInfo<TState, TBaseState> : IStateInfo where TState : class, IState, TBaseState
    {
        private readonly StateMachine<TBaseState> _stateMachine;
        private readonly TState _state;
        private readonly ITickable _tickable;
        private readonly IExitable _exitable;

        public StateInfo(StateMachine<TBaseState> stateMachine, TState state)
        {
            _stateMachine = stateMachine;
            _state = state;
            _tickable = state as ITickable;
            _exitable = state as IExitable;
            StateType = typeof(TState);
        }

        public Type StateType { get; }

        public virtual void Enter() => _stateMachine.Enter<TState>();

        public void Tick() => _tickable?.Tick();

        public void Exit() => _exitable?.Exit();
    }
}