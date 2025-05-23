using System.Collections;
using UnityEngine;

/// <summary>
/// Class that handles individual object animations on Start by instantiating a particle effect
/// and doing an animation based on three parameters
/// - These parameters are set up by the classes that extend this class overriding the SetAnimationParameters() method
/// </summary>
public abstract class ObjectStartAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject particleEffects;

    private GameObject currentParticleEffects;

    private float totalRotation = 360f; // in degrees

    private float animationTimer = 0.5f; // 1 second animation

    // Animation Values - updated in the class that extends this one in their SetAnimationParameters() override method
    protected bool rotatingSideways = false;
    protected bool rotatingVertically = false;
    protected bool comingDown = false;

    // In the class that extends this, on Awake, change animation values
    private void Awake()
    {
        SetAnimationParameters();
    }

    // Method that will be overriden by the animation classes extending this one - to set up animation parameters
    public virtual void SetAnimationParameters()
    {
        Debug.Log("Animation Parameters set up with standard values.");
    }

    // On Instantiation, call for animation
    private void Start()
    {
        StartAnimation();
    }

    // Instantiates the particle effect and calls for the animation to play
    private void StartAnimation()
    {
        if(particleEffects != null)
        {
            currentParticleEffects = Instantiate(particleEffects, this.transform.position, Quaternion.identity);
            Destroy(currentParticleEffects, animationTimer * 1.2f);
        }
        else
        {
            Debug.LogWarning("Particle effects non existent");
        }

        StartCoroutine(PlayAnimation());
    }

    // Plays Animation, given the parameters for the animation
    private  IEnumerator PlayAnimation()
    {
        float elapsedTime = 0f;

        Quaternion startRotation = transform.rotation; // for the rotation animations
        float rotationPerSecond = totalRotation / animationTimer; // degrees to rotate per animation timer second

        Vector3 startPosition = transform.position;
        Vector3 maximumHeightPosition = startPosition + Vector3.up * 5f; // maximum vertical position

        if(comingDown)
            transform.position = maximumHeightPosition;

        // I centered the building Pivot to allow for simpler rotation
        while (elapsedTime < animationTimer)
        {
            if (rotatingSideways)
            {
                transform.Rotate(Vector3.up, rotationPerSecond * Time.deltaTime, Space.Self); 
            }
            if (rotatingVertically)
            {
                transform.Rotate(Vector3.right, rotationPerSecond * Time.deltaTime, Space.Self);
            }
            if (comingDown)
            {
                transform.position = Vector3.Slerp(transform.position, startPosition, elapsedTime / animationTimer);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Here to make sure rotationalways end up correctly regardless
        transform.rotation = startRotation;
        transform.position = startPosition;
    }
}
