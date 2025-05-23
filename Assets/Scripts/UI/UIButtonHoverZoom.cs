using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Zooms in deployable object image on hover
/// Since it only zooms a little bit I simply change positions,
/// if it were to be a slow zoom I'd use a coroutine or a bool flag and Update()
/// to Lerp it from one scale to another across a set time
/// </summary>
public class UIButtonHoverZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image innerImage;
    private Vector2 originalImageScale;

    private float zoomScale = 1.5f;

    private SoundManager soundManager;

    void Start()
    {
        soundManager = SoundManager.Instance;

        if (innerImage != null)
        {
            originalImageScale = innerImage.rectTransform.localScale;
        }
        else
        {
            Debug.LogError("Inner image not set up in the director for one button.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Click Entering Button Sound
        soundManager.CheckPlaySound("ButtonEnter");
        innerImage.rectTransform.localScale = originalImageScale * zoomScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        innerImage.rectTransform.localScale = originalImageScale;
        // NOT USED - Leaving Button Sound
        //soundManager.CheckPlaySound("ButtonExit");
    }
}
