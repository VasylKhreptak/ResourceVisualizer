using Infrastructure.StateMachine.Game.Factory;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main;

namespace Infrastructure.StateMachine.Game
{
    public class GameStateMachine : StateMachine<IGameState>
    {
        private GameStateMachine(GameStateFactory stateFactory) : base(stateFactory) { }
    }
}