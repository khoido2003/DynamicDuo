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

        [SerializeField]
        private LevelData m_levelData;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            // Config
            builder.RegisterInstance(m_gameConfig);
            builder.RegisterInstance(m_levelData);

            // Domain
            builder.Register<BoardModel>(Lifetime.Singleton);

            // State machine
            builder.Register<GameStateMachine>(Lifetime.Singleton);

            builder.Register<LoadingState>(Lifetime.Singleton);
            builder.Register<GameplayState>(Lifetime.Singleton);
            builder.Register<PausedGameState>(Lifetime.Singleton);
            builder.Register<WinState>(Lifetime.Singleton);
            builder.Register<LoseState>(Lifetime.Singleton);

            // Entry point
            builder.RegisterEntryPoint<GameBootstrapper>();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
