using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DynamicDuo.UI
{
    public class GameResultView : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_root;

        [SerializeField]
        private GameObject m_winPanel;

        [SerializeField]
        private GameObject m_losePanel;

        [SerializeField]
        private Button m_restartBtn;

        [SerializeField]
        private Button m_nextLevelBtn;

        [SerializeField]
        private Button m_menuButton;

        public event Action OnRestartClicked;
        public event Action OnNextLevelClicked;
        public event Action OnMenuClicked;

        private void Awake()
        {
            m_restartBtn.onClick.AddListener(() => OnRestartClicked?.Invoke());
            m_nextLevelBtn.onClick.AddListener(() => OnNextLevelClicked?.Invoke());
            m_menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());

            Hide();
        }

        public void ShowWin()
        {
            m_root.SetActive(true);
            m_winPanel.SetActive(true);
            m_losePanel.SetActive(false);

            m_restartBtn.gameObject.SetActive(true);
            m_nextLevelBtn.gameObject.SetActive(true);
            m_menuButton.gameObject.SetActive(true);
        }

        public void ShowLose()
        {
            m_root.SetActive(true);
            m_winPanel.SetActive(false);
            m_losePanel.SetActive(true);

            m_restartBtn.gameObject.SetActive(true);
            m_nextLevelBtn.gameObject.SetActive(false);
            m_menuButton.gameObject.SetActive(true);
        }

        public void Hide()
        {
            m_root.SetActive(false);
        }
    }
}
