using UnityEngine;
using UnityEngine.InputSystem;

// Makes instantiated image follow cursor
public class ImageFollowMouseCursor : MonoBehaviour
{
    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue(); // mouse screen position
        this.transform.position = mousePosition;
    }
}
