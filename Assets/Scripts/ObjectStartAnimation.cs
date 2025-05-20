using System;
using System.Collections;
using UnityEngine;

public class ObjectStartAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject particleEffects;

    private GameObject currentParticleEffects;

    private float animationSpeed = 360f; // in degrees per second

    private float animationTimer = 1f; // 1 second animation

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
            Destroy(currentParticleEffects, animationTimer * 0.8f);
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

        Vector3 startPosition = transform.position;
        Vector3 maximumHeightPosition = startPosition + Vector3.up * 5f; // maximum vertical position

        if(comingDown)
            transform.position = maximumHeightPosition;

        // TODO - Edit. There is still a problem where, if I have more than one animation variable it won't end at beginning rotation automatically
        // since it will be in a different rotation
        while (elapsedTime < animationTimer)
        {
            if (rotatingSideways)
            {
                transform.Rotate(0f, animationSpeed * Time.deltaTime, 0f);  
            }
            if (rotatingVertically)
            {
                transform.Rotate(animationSpeed * Time.deltaTime, 0f , 0f);
            }
            if (comingDown)
            {
                transform.position = Vector3.Lerp(transform.position, startPosition, elapsedTime / animationTimer);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Here to make sure rotationalways end up correctly
        transform.rotation = startRotation; // Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime);
        transform.position = startPosition;
    }
}
