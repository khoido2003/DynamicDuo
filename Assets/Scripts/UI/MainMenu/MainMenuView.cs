using Unity.DynamicDuo.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField]
    private Button m_playButton;

    private void Awake()
    {
        m_playButton.onClick.AddListener(OnPlayClicked);
    }

    private void Start() { }

    private void Update() { }

    private void OnDestroy()
    {
        m_playButton.onClick.RemoveListener(OnPlayClicked);
    }

    private void OnPlayClicked()
    {
        Debug.Log("Click start game");
        LevelSession.StartGenerated(colorCount: 6, tubeCapacity: 4, emptyTubes: 2);

        SceneManager.LoadScene("GamePlay");
    }
}
