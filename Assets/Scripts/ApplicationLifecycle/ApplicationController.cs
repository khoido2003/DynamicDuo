using Unity.DynamicDuo.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Unity.DynamicDuo.ApplicationLifecycle
{
    public class ApplicationController : LifetimeScope
    {
        [SerializeField]
        private GameConfig m_gameConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            // Config
            builder.RegisterInstance(m_gameConfig);

            builder.Register<MainMenuState>(Lifetime.Singleton);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("MainMenu");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
