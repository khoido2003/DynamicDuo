using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class LoadingState : IGameState
    {
        public void Enter()
        {
            Debug.Log("[LoadingState] Enter");
            // Will: load level data, initialize BoardModel, transition to GameplayState
        }

        public void Exit()
        {
            Debug.Log("[LoadingState] Exit");
        }
    }
}
