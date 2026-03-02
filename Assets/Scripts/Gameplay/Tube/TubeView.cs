using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.DynamicDuo.Gameplay;
using UnityEngine;

public class TubeView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] m_slots;

    [SerializeField]
    private GameObject m_selectionHighlight;

    [SerializeField]
    private SpriteRenderer m_tubeBackground;

    [SerializeField]
    private GameObject m_completeIndicator;

    public int TubeIndex { get; private set; }
    private bool m_isComplete = false;

    public event Action<int> OnClicked;

    public void Initialize(int tubeIndex)
    {
        TubeIndex = tubeIndex;
        m_selectionHighlight.SetActive(false);
        m_completeIndicator.SetActive(false);
    }

    public void RefreshSegements(IReadOnlyList<ColorSegment> segments, ColorPalette palette)
    {
        List<TubeColor> flatColors = FlattenSegments(segments);

        for (int i = 0; i < m_slots.Length; i++)
        {
            if (i < flatColors.Count)
            {
                m_slots[i].gameObject.SetActive(true);
                m_slots[i].color = palette.GetColor(flatColors[i]);
            }
            else
            {
                m_slots[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetSelected(bool selected)
    {
        m_selectionHighlight.SetActive(selected);
    }

    public void SetComplete(bool complete)
    {
        m_completeIndicator.SetActive(complete);
        m_isComplete = complete;
    }

    public void PlayShake()
    {
        transform.DOKill();

        transform.DOShakePosition(
            duration: 0.4f,
            strength: new Vector3(0.15f, 0f, 0f),
            vibrato: 20,
            randomness: 0f
        );
    }

    private void OnMouseDown()
    {
        if (m_isComplete)
        {
            return;
        }
        OnClicked?.Invoke(TubeIndex);
    }

    // Convert [(Red, 2), (Blue, 1)] -> [Red, Red, Blue]
    private List<TubeColor> FlattenSegments(IReadOnlyList<ColorSegment> segments)
    {
        List<TubeColor> result = new();

        // iterate bottom → top (model order)
        foreach (ColorSegment seg in segments)
        {
            for (int i = 0; i < seg.Count; i++)
            {
                result.Add(seg.Color);
            }
        }

        return result;
    }
}
