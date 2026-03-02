using System;
using System.Collections.Generic;
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

    public int TubeIndex { get; private set; }

    public event Action<int> OnClicked;

    public void Initialize(int tubeIndex)
    {
        TubeIndex = tubeIndex;
        m_selectionHighlight.SetActive(false);
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

    private void OnMouseDown()
    {
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

        // reverse so last (top) becomes highest slot
        result.Reverse();

        return result;
    }
}
