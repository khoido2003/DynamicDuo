using System;
using System.Collections.Generic;
using Unity.DynamicDuo.Gameplay;
using Unity.DynamicDuo.Infrastructure;
using UnityEngine;

public class BoardPresenter : IDisposable
{
    private readonly BoardModel m_boardModel;
    private readonly BoardView m_boardView;
    private readonly ColorPalette m_palette;

    private readonly ISubscriber<TubeClickedEvent> m_tubeClickedSub;
    private readonly ISubscriber<UndoRequestedEvent> m_undoSub;
    private readonly ISubscriber<PourSucceededEvent> m_pourSucceededSub;
    private readonly ISubscriber<PourFailedEvent> m_pourFailedSub;
    private readonly ISubscriber<BoardRestoredEvent> m_boardRestoredSub;

    private readonly IPublisher<TubeClickedEvent> m_tubeClickedPub;
    private readonly IPublisher<TubeCompleteEvent> m_tubeCompletePub;
    private readonly IPublisher<PourSucceededEvent> m_pourSucceededPub;
    private readonly IPublisher<PourFailedEvent> m_pourFailedPub;
    readonly IPublisher<MoveCountChangedEvent> m_moveCountPub;
    readonly IPublisher<BoardRestoredEvent> m_boardRestoredPub;
    readonly IPublisher<WinEvent> m_winPub;
    readonly IPublisher<LoseEvent> m_losePub;

    private IDisposable m_tubeClickedDisposable;
    private IDisposable m_undoDisposable;

    int m_selectedTubeIndex = -1;

    readonly List<TubePresenter> m_tubePresenters = new();

    ////////////////////////////////////////

    public BoardPresenter(
        BoardModel boardModel,
        BoardView boardView,
        ColorPalette palette,
        ISubscriber<TubeClickedEvent> tubeClickedSub,
        ISubscriber<UndoRequestedEvent> undoSub,
        ISubscriber<PourSucceededEvent> pourSucceededSub,
        ISubscriber<PourFailedEvent> pourFailedSub,
        ISubscriber<BoardRestoredEvent> boardRestoredSub,
        IPublisher<TubeClickedEvent> tubeClickedPub,
        IPublisher<PourSucceededEvent> pourSucceededPub,
        IPublisher<PourFailedEvent> pourFailedPub,
        IPublisher<MoveCountChangedEvent> moveCountPub,
        IPublisher<BoardRestoredEvent> boardRestoredPub,
        IPublisher<WinEvent> winPub,
        IPublisher<LoseEvent> losePub,
        IPublisher<TubeCompleteEvent> tubeCompletePub
    )
    {
        m_boardView = boardView;
        m_palette = palette;
        m_boardModel = boardModel;

        m_tubeClickedSub = tubeClickedSub;
        m_undoSub = undoSub;
        m_pourSucceededSub = pourSucceededSub;
        m_pourFailedSub = pourFailedSub;
        m_boardRestoredSub = boardRestoredSub;

        m_tubeClickedPub = tubeClickedPub;
        m_pourSucceededPub = pourSucceededPub;
        m_pourFailedPub = pourFailedPub;
        m_moveCountPub = moveCountPub;
        m_boardRestoredPub = boardRestoredPub;
        m_tubeCompletePub = tubeCompletePub;
        m_winPub = winPub;
        m_losePub = losePub;
    }

    public void Activate()
    {
        m_boardView.BuildBoard(m_boardModel.Tubes, m_palette);

        foreach (TubeView tubeView in m_boardView.TubeViews)
        {
            TubeModel tubeModel = m_boardModel.Tubes[tubeView.TubeIndex];

            TubePresenter tubePresenter = new(
                tubeModel,
                tubeView,
                m_palette,
                m_pourSucceededSub,
                m_pourFailedSub,
                m_boardRestoredSub,
                m_tubeClickedPub,
                m_tubeCompletePub
            );

            tubePresenter.Activate();
            m_tubePresenters.Add(tubePresenter);
        }

        m_tubeClickedDisposable = m_tubeClickedSub.Subscribe(OnTubeClicked);
        m_undoDisposable = m_undoSub.Subscribe(OnUndoRequested);
        m_selectedTubeIndex = -1;
    }

    public void Deactivate()
    {
        foreach (var p in m_tubePresenters)
        {
            p.Dispose();
        }
        m_tubePresenters.Clear();

        m_tubeClickedDisposable?.Dispose();
        m_undoDisposable?.Dispose();
        m_selectedTubeIndex = -1;
    }

    #region Action

    private void OnTubeClicked(TubeClickedEvent @event)
    {
        if (m_selectedTubeIndex == -1)
        {
            if (m_boardModel.Tubes[@event.TubeIndex].IsEmpty)
            {
                Debug.Log("[BoardPresenter] Tapped empty tube — nothing to pour");
                return;
            }

            m_selectedTubeIndex = @event.TubeIndex;
            m_boardView.TubeViews[m_selectedTubeIndex].SetSelected(true);

            Debug.Log($"[BoardPresenter] Selected tube {m_selectedTubeIndex}");
            return;
        }

        // Click Same tube twice -> deselect
        if (@event.TubeIndex == m_selectedTubeIndex)
        {
            m_boardView.TubeViews[m_selectedTubeIndex].SetSelected(false);
            m_selectedTubeIndex = -1;
            Debug.Log("[BoardPresenter] Deselected");
            return;
        }

        // Second click on other tube
        int from = m_selectedTubeIndex;
        int to = @event.TubeIndex;
        m_selectedTubeIndex = -1;

        m_boardView.TubeViews[from].SetSelected(false);

        PourResult result = m_boardModel.TryPour(from, to);

        if (result == PourResult.Success)
        {
            Debug.Log($"[BoardPresenter] Pour {from}→{to} succeeded");

            m_pourSucceededPub.Publish(new PourSucceededEvent { FromIndex = from, ToIndex = to });

            m_moveCountPub.Publish(
                new MoveCountChangedEvent { MoveCount = m_boardModel.MoveCount }
            );

            if (m_boardModel.CheckWin())
            {
                Debug.Log("[BoardPresenter] Win condition met!");
                m_winPub.Publish(new WinEvent());
            }
            else if (!m_boardModel.HasAnyValidMove())
            {
                Debug.Log("[BoardPresenter] No valid moves — lose!");
                m_losePub.Publish(new LoseEvent());
            }
        }
        else
        {
            Debug.Log($"[BoardPresenter] Pour {from}→{to} failed: {result}");
            m_pourFailedPub.Publish(new PourFailedEvent { TubeIndex = to });
        }
    }

    private void OnUndoRequested(UndoRequestedEvent _)
    {
        if (m_boardModel.Undo())
        {
            m_boardRestoredPub.Publish(new BoardRestoredEvent());
            m_moveCountPub.Publish(
                new MoveCountChangedEvent { MoveCount = m_boardModel.MoveCount }
            );
            Debug.Log("[BoardPresenter] Undo successful");
        }
    }

    #endregion

    public void Dispose()
    {
        Deactivate();
    }
}
