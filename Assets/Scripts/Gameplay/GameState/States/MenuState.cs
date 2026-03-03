using UnityEngine.SceneManagement;

namespace Unity.DynamicDuo.Gameplay
{
    public class MainMenuState : IGameState
    {
        public void Enter()
        {
            LevelSession.Reset();
            SceneManager.LoadScene("MainMenu");
        }

        public void Exit() { }
    }
}
