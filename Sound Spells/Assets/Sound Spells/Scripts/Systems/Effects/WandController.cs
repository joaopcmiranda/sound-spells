using UnityEngine;
using System.Collections;
using System;
using Sound_Spells.Systems.Weather;

public class WandController : MonoBehaviour
{
    public event Action OnAnimationStart;
    public event Action OnAnimationEnd;
    
    public Transform targetWaypoint;
    public float animationDuration = 3.0f;
    public float figure8Width = 0.5f;
    public float figure8Height = 0.2f;
    public float figure8Frequency = 3.0f;
    public float maxRotationAngle = 15.0f;
    public float rotationFrequency = 4.0f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isCasting = false;

    [SerializeField] private MagicEffect _magicEffewand;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void CastSpell(WeatherType weatherType)
    {
        if (!isCasting)
        {
            StartCoroutine(AnimateWand(weatherType));
        }
    }

    private IEnumerator AnimateWand(WeatherType weatherType)
    {
        isCasting = true;
        OnAnimationStart?.Invoke();
        
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
        Vector3 startPos = initialPosition;
        Vector3 targetPos;
        if (targetWaypoint != null)
        {
            targetPos = targetWaypoint.position;
        }
        else
        {
            targetPos = Camera.main.ViewportToWorldPoint(new Vector3(0.85f, 0.4f, 10f));
        }
        
        float journeyTime = animationDuration * 0.5f;
        float holdTime = animationDuration * 0.5f;   
        
        float timer = 0f;
        while (timer < journeyTime)
        {
            float progress = timer / journeyTime;
            transform.position = Vector3.Lerp(startPos, targetPos, progress);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        MagicEffect.CastMagicEffect(weatherType);

        timer = 0f;
        while (timer < holdTime)
        {
            
            // Figure 8 motion
            float figure8OffsetX = Mathf.Sin(timer * figure8Frequency) * figure8Width;
            float figure8OffsetY = Mathf.Sin(timer * figure8Frequency * 2.0f) * figure8Height;
            transform.position = targetPos + new Vector3(figure8OffsetX, figure8OffsetY, 0);

            // Back-and-forth rotation
            float angle = Mathf.Sin(timer * rotationFrequency) * maxRotationAngle;
            transform.rotation = initialRotation * Quaternion.Euler(0, 0, angle);
            
            timer += Time.deltaTime;
            yield return null;
        }

        Vector3 finalPos = initialPosition;
        timer = 0f;
        while (timer < journeyTime)
        {
            float progress = timer / journeyTime;
            transform.position = Vector3.Lerp(targetPos, finalPos, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
        OnAnimationEnd?.Invoke();
        isCasting = false;
    }
}