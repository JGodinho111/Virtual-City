using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Changes UI panel position on Hover
/// </summary>
public class UIPanelPositionChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform panelRectTransform;
    private Vector2 panelStartPosition;
    private Vector2 hoverUp;

    private Vector2 panelSize;

    //private SoundManager soundManager;

    void Awake()
    {
        panelRectTransform = this.gameObject.GetComponent<RectTransform>();
        panelStartPosition = panelRectTransform.anchoredPosition;
        hoverUp = new Vector3(0f, 11f);
        panelSize = panelRectTransform.sizeDelta;
    }

    private void Start()
    {
        //soundManager = SoundManager.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        panelRectTransform.anchoredPosition = panelStartPosition + hoverUp;

        // this if here for this panel specifically to make sure it always reaches the bottom of the screen
        // and doesn't start going back and forth between OnPointerEnter and OnPointerExit non-stop
        Vector2 newSize = panelSize + new Vector2(0f, 30f);
        panelRectTransform.sizeDelta = newSize;
        // NOT USED - Entering Selectable Area Sound
        //soundManager.CheckPlaySound("PanelEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        panelRectTransform.anchoredPosition = panelStartPosition;
        panelRectTransform.sizeDelta = panelSize;
        // NOT USED - Leaving Selectable Area Sound
        //soundManager.CheckPlaySound("PanelExit");
    }
}
