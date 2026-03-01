using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class PausedGameState : IGameState
    {
        public void Enter()
        {
            Debug.Log("[PausedState] Enter");
            // Will: show pause screen, freeze input
        }

        public void Exit()
        {
            Debug.Log("[PausedState] Exit");
            // Will: hide pause screen
        }
    }
}
