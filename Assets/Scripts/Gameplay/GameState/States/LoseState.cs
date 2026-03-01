using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class LoseState : IGameState
    {
        public void Enter()
        {
            Debug.Log("[LoseState] Enter");
            // Will: show lose screen, disable input
        }

        public void Exit()
        {
            Debug.Log("[LoseState] Exit");
            // Will: hide lose screen
        }
    }
}
