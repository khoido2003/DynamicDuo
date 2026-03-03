using System;
using VContainer.Unity;

namespace Unity.DynamicDuo.Infrastructure
{
    public class GameplayAudioController : IStartable, IDisposable
    {
        readonly IAudioService m_audio;

        readonly ISubscriber<PourSucceededEvent> m_pourSub;
        readonly ISubscriber<PourFailedEvent> m_failSub;
        readonly ISubscriber<WinEvent> m_winSub;
        readonly ISubscriber<LoseEvent> m_loseSub;
        readonly ISubscriber<BoardRestoredEvent> m_undoSub;

        // Tube complete reuses PourSucceeded — we check complete state there
        readonly ISubscriber<TubeCompleteEvent> m_completeSub;

        IDisposable m_pourDisposable;
        IDisposable m_failDisposable;
        IDisposable m_winDisposable;
        IDisposable m_loseDisposable;
        IDisposable m_undoDisposable;
        IDisposable m_completeDisposable;

        public GameplayAudioController(
            IAudioService audio,
            ISubscriber<PourSucceededEvent> pourSub,
            ISubscriber<PourFailedEvent> failSub,
            ISubscriber<WinEvent> winSub,
            ISubscriber<LoseEvent> loseSub,
            ISubscriber<BoardRestoredEvent> undoSub,
            ISubscriber<TubeCompleteEvent> completeSub
        )
        {
            m_audio = audio;
            m_pourSub = pourSub;
            m_failSub = failSub;
            m_winSub = winSub;
            m_loseSub = loseSub;
            m_undoSub = undoSub;
            m_completeSub = completeSub;
        }

        public void Start()
        {
            m_pourDisposable = m_pourSub.Subscribe(_ => m_audio.PlaySFX(SoundId.Pour));
            m_failDisposable = m_failSub.Subscribe(_ => m_audio.PlaySFX(SoundId.Invalid));
            m_winDisposable = m_winSub.Subscribe(OnWin);
            m_loseDisposable = m_loseSub.Subscribe(OnLose);
            m_undoDisposable = m_undoSub.Subscribe(_ => m_audio.PlaySFX(SoundId.Pour));
            m_completeDisposable = m_completeSub.Subscribe(_ =>
                m_audio.PlaySFX(SoundId.TubeComplete)
            );

            // Start background music in start
            m_audio.PlayMusic(SoundId.MusicGameplay);
        }

        void OnWin(WinEvent _)
        {
            m_audio.StopMusic();
            m_audio.PlaySFX(SoundId.Win);
        }

        void OnLose(LoseEvent _)
        {
            m_audio.StopMusic();
            m_audio.PlaySFX(SoundId.Lose);
        }

        public void Dispose()
        {
            m_pourDisposable?.Dispose();
            m_failDisposable?.Dispose();
            m_winDisposable?.Dispose();
            m_loseDisposable?.Dispose();
            m_undoDisposable?.Dispose();
            m_completeDisposable?.Dispose();
            m_audio.StopMusic();
        }
    }
}
