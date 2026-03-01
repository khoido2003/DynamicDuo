using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class WinState : IGameState
    {
        public void Enter()
        {
            Debug.Log("[WinState] Enter");
            // Will: show win screen, disable input
        }

        public void Exit()
        {
            Debug.Log("[WinState] Exit");
            // Will: hide win screen
        }
    }
}
