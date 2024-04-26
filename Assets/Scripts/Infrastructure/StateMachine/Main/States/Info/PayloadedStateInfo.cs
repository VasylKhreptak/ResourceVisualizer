using System;
using Infrastructure.StateMachine.Main.States.Core;
using Infrastructure.StateMachine.Main.States.Info.Core;
using Zenject;

namespace Infrastructure.StateMachine.Main.States.Info
{
    public class PayloadedStateInfo<TState, TBaseState, TPayload> : IStateInfo
        where TState : class, IPayloadedState<TPayload>, TBaseState
    {
        private readonly StateMachine<TBaseState> _stateMachine;
        private readonly TPayload _payload;
        private readonly ITickable _tickable;
        private readonly IExitable _exitable;

        public PayloadedStateInfo(StateMachine<TBaseState> stateMachine, TState state, TPayload payload)
        {
            _stateMachine = stateMachine;
            _payload = payload;

            _tickable = state as ITickable;
            _exitable = state as IExitable;

            StateType = typeof(TState);
        }

        public Type StateType { get; }

        public void Enter() => _stateMachine.Enter<TState, TPayload>(_payload);

        public void Tick() => _tickable?.Tick();

        public void Exit() => _exitable?.Exit();
    }
}