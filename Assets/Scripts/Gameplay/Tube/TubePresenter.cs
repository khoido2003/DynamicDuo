using System;
using Unity.DynamicDuo.Gameplay;
using Unity.DynamicDuo.Infrastructure;
using UnityEngine;

public class TubePresenter : IDisposable
{
    private readonly TubeModel m_tubeModel;
    private readonly TubeView m_tubeView;
    private readonly ColorPalette m_palette;

    private readonly ISubscriber<PourSucceededEvent> m_pourSucceededSub;
    private readonly ISubscriber<PourFailedEvent> m_pourFailedSub;
    private readonly ISubscriber<BoardRestoredEvent> m_boardRestoreSub;

    private readonly IPublisher<TubeClickedEvent> m_tubesClickedPub;
    private readonly IPublisher<TubeCompleteEvent> m_tubeCompletePub;

    IDisposable m_pourSucceededDisposable;
    IDisposable m_pourFailedDisposable;
    IDisposable m_boardRestoredDisposable;

    public TubePresenter(
        TubeModel model,
        TubeView view,
        ColorPalette palette,
        ISubscriber<PourSucceededEvent> pourSucceededSub,
        ISubscriber<PourFailedEvent> pourFailedSub,
        ISubscriber<BoardRestoredEvent> boardRestoredSub,
        IPublisher<TubeClickedEvent> tubeClickedPub,
        IPublisher<TubeCompleteEvent> tubeCompletePub
    )
    {
        m_tubeModel = model;
        m_tubeView = view;
        m_palette = palette;
        m_pourSucceededSub = pourSucceededSub;
        m_pourFailedSub = pourFailedSub;
        m_boardRestoreSub = boardRestoredSub;
        m_tubesClickedPub = tubeClickedPub;
        m_tubeCompletePub = tubeCompletePub;

        // Wire view input → event bus
        m_tubeView.OnClicked += OnViewClicked;
    }

    public void Activate()
    {
        m_pourSucceededDisposable = m_pourSucceededSub.Subscribe(OnPourSucceeded);
        m_pourFailedDisposable = m_pourFailedSub.Subscribe(OnPourFailed);
        m_boardRestoredDisposable = m_boardRestoreSub.Subscribe(OnBoardRestored);
    }

    public void Deactivate()
    {
        m_pourSucceededDisposable?.Dispose();
        m_pourFailedDisposable?.Dispose();
        m_boardRestoredDisposable?.Dispose();
    }

    public void Dispose()
    {
        m_tubeView.OnClicked -= OnViewClicked;
        Deactivate();
    }

    ///////////////////////////////

    #region Event

    private void OnBoardRestored(BoardRestoredEvent @event)
    {
        m_tubeView.RefreshSegements(m_tubeModel.Segments, m_palette);
    }

    private void OnPourFailed(PourFailedEvent @event)
    {
        if (@event.TubeIndex == m_tubeModel.Index)
        {
            Debug.Log($"[TubePresenter] Tube {m_tubeModel.Index} invalid target");

            m_tubeView.PlayShake();
        }
    }

    private void OnPourSucceeded(PourSucceededEvent @event)
    {
        if (@event.FromIndex == m_tubeModel.Index || @event.ToIndex == m_tubeModel.Index)
        {
            m_tubeView.RefreshSegements(m_tubeModel.Segments, m_palette);

            if (m_tubeModel.IsComplete)
            {
                Debug.Log($"Tube {@event.ToIndex} is complete");
                m_tubeView.SetComplete(true);

                m_tubeCompletePub.Publish(new TubeCompleteEvent { TubeIndex = m_tubeModel.Index });
            }
        }
    }

    private void OnViewClicked(int index)
    {
        m_tubesClickedPub.Publish(new TubeClickedEvent { TubeIndex = index });
    }

    #endregion
}
