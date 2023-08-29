using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable CS1591

/// <summary>
/// Port of <c>ScrollBarHandle</c>
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class ScrollBarHandle : MonoBehaviour
{
    public Scrollbar scrollBar;

    private RectTransform trans;

    public ScrollBarHandle() { }

    private void Awake()
    {
        trans = GetComponent<RectTransform>();
        if (!scrollBar)
        {
            scrollBar = GetComponentInParent<Scrollbar>();
        }
    }

    private void Start()
    {
        if (scrollBar)
        {
            scrollBar.onValueChanged.AddListener(new UnityAction<float>(UpdatePosition));
        }
    }

    private void UpdatePosition(float value)
    {
        trans.pivot = new Vector2(0.5f, value);
        trans.anchorMin = new Vector2(0.5f, value);
        trans.anchorMax = new Vector2(0.5f, value);
        trans.anchoredPosition.Set(trans.anchoredPosition.x, 0f);
    }
}
