using Unity.DynamicDuo.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Unity.DynamicDuo.UI
{
    public class InMatchView : MonoBehaviour
    {
        [SerializeField]
        private Button m_exitMatch;

        private GameStateMachine m_stateMachine;

        [Inject]
        public void Construct(GameStateMachine stateMachine)
        {
            m_stateMachine = stateMachine;
        }

        private void Start()
        {
            m_exitMatch.onClick.AddListener(OnExitClicked);
        }

        private void OnDestroy()
        {
            m_exitMatch.onClick.RemoveListener(OnExitClicked);
        }

        private void OnExitClicked()
        {
            m_stateMachine.TransitionTo<MainMenuState>();
        }
    }
}
