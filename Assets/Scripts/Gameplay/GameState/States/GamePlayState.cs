using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class GameplayState : IGameState
    {
        public void Enter()
        {
            Debug.Log("[GameplayState] Enter");
            // Will: enable input, start gameplay
        }

        public void Exit()
        {
            Debug.Log("[GameplayState] Exit");
            // Will: disable input
        }
    }
}
