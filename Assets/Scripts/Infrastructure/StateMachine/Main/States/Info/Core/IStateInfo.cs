using System;

namespace Infrastructure.StateMachine.Main.States.Info.Core
{
    public interface IStateInfo
    {
        public Type StateType { get; }

        public void Enter();

        public void Tick();

        public void Exit();
    }
}