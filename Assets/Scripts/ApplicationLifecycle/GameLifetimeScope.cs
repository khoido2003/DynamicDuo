using Unity.DynamicDuo.Gameplay;
using Unity.DynamicDuo.Infrastructure;
using Unity.DynamicDuo.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Unity.DynamicDuo.ApplicationLifecycle
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private LevelData m_levelData;

        [SerializeField]
        private ColorPalette m_colorPalette;

        protected override void Configure(IContainerBuilder builder)
        {
            // Config
            builder.RegisterInstance(m_levelData);
            builder.RegisterInstance(m_colorPalette);

            // Domain
            builder.Register<BoardModel>(Lifetime.Singleton);
            builder.Register<LevelGeneratorModel>(Lifetime.Singleton);

            // State machine + states
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.Register<LoadingState>(Lifetime.Singleton);
            builder.Register<GameplayState>(Lifetime.Singleton);
            builder.Register<PausedGameState>(Lifetime.Singleton);
            builder.Register<WinState>(Lifetime.Singleton);
            builder.Register<LoseState>(Lifetime.Singleton);

            builder.Register<MainMenuState>(Lifetime.Singleton);
            // Presentation
            builder.Register<BoardPresenter>(Lifetime.Singleton);
            builder.Register<GameResultPresenter>(Lifetime.Singleton);

            // Message channels
            RegisterChannel<TubeClickedEvent>(builder);
            RegisterChannel<UndoRequestedEvent>(builder);
            RegisterChannel<PauseRequestedEvent>(builder);
            RegisterChannel<ResumeRequestedEvent>(builder);
            RegisterChannel<RestartRequestedEvent>(builder);
            RegisterChannel<NextLevelRequestedEvent>(builder);
            RegisterChannel<PourSucceededEvent>(builder);
            RegisterChannel<PourFailedEvent>(builder);
            RegisterChannel<MoveCountChangedEvent>(builder);
            RegisterChannel<BoardRestoredEvent>(builder);
            RegisterChannel<WinEvent>(builder);
            RegisterChannel<LoseEvent>(builder);
            RegisterChannel<LevelLoadedEvent>(builder);
            RegisterChannel<ReturnToMenuRequestedEvent>(builder);
            RegisterChannel<TubeCompleteEvent>(builder);

            // Bootstrapper runs when this scene loads
            builder.RegisterEntryPoint<GameBootstrapper>();

            // Components
            builder.RegisterComponentInHierarchy<EventTest>();
            builder.RegisterComponentInHierarchy<BoardView>();
            builder.RegisterComponentInHierarchy<GameResultView>();
            builder.RegisterComponentInHierarchy<InMatchView>();

            // Audio
            builder.RegisterComponentInHierarchy<AudioSetup>();

            builder.Register<IAudioService>(
                resolver =>
                {
                    var setup = resolver.Resolve<AudioSetup>();
                    var (sfx, music) = setup.CreateSources();
                    return new AudioService(sfx, music, setup.Registry);
                },
                Lifetime.Singleton
            );
            // Audio controller — listens to events, calls audio service
            builder.RegisterEntryPoint<GameplayAudioController>();

            base.Configure(builder);
        }

        protected void RegisterChannel<T>(IContainerBuilder builder)
        {
            builder
                .Register<MessageChannel<T>>(Lifetime.Singleton)
                .As<IMessageChannel<T>>()
                .As<IPublisher<T>>()
                .As<ISubscriber<T>>();
        }
    }
}
