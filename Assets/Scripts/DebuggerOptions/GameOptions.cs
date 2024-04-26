using System.ComponentModel;
using Infrastructure.StateMachine.Game.States;
using Infrastructure.StateMachine.Game.States.Core;
using Infrastructure.StateMachine.Main.Core;
using Infrastructure.Transition.Core;

namespace DebuggerOptions
{
    public class GameOptions
    {
        private readonly IStateMachine<IGameState> _stateMachine;
        private readonly ITransitionScreen _transitionScreen;

        public GameOptions(IStateMachine<IGameState> stateMachine, ITransitionScreen transitionScreen)
        {
            _stateMachine = stateMachine;
            _transitionScreen = transitionScreen;
        }

        [Category("Game")]
        public void Reload() => _stateMachine.Enter<ReloadState>();

        [Category("Game")]
        public void ShowTransitionScreen() => _transitionScreen.Show();

        [Category("Game")]
        public void HideTransitionScreen() => _transitionScreen.Hide();
    }
}