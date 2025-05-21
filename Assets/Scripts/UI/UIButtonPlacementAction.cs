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

    private SoundManager soundManager;

    void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // TODO - Swoosh sound as if I were "grabbing the object from nothing"
        soundManager.CheckPlaySound("ButtonPress");
        if(dragNDropPlacer != null)
            dragNDropPlacer.StartGameObjectPlacement(gameObjectToPlacePrefab, imageGameObjectToPlacePrefab);
    }

    
}
