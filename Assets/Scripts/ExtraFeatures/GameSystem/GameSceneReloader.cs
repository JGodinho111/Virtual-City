using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Just for fun - Extra Feature
///
/// Called by endscreen panel button to reload the scene
/// </summary>
public class GameSceneReloader : MonoBehaviour
{
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
