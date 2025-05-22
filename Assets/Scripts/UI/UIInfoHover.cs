using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Simple class to create item appearance onHovering this UI item (info image)
/// </summary>
public class UIInfoHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject infoPanel;

    void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (infoPanel != null)
            infoPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}
