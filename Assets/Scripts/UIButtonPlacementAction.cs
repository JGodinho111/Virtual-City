using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attached to the image button to call if button is pressed, rather than directly through OnClick in the editor
/// This is because it needs to be a drag and drop and not a click and the click again in the scene
/// </summary>
public class UIButtonPlacementAction : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GameObject gameObjectToPlacePrefab;

    [SerializeField]
    private GameObject imageGameObjectToPlacePrefab;

    [SerializeField]
    private DragNDropPlacer dragNDropPlacer;

    public void OnPointerDown(PointerEventData eventData)
    {
        dragNDropPlacer.StartGameObjectPlacement(gameObjectToPlacePrefab, imageGameObjectToPlacePrefab);
    }

    void Start()
    {
        // TODO: If I change it to an instance get it here rather than as a SerializeField
    }
}
