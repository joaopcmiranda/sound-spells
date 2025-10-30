using Sound_Spells.Systems.Weather;
using UnityEngine;

public class MagicEffect : MonoBehaviour
{
    private static Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    public static void CastMagicEffect(WeatherType weatherType)
    {
        _animator.SetTrigger(weatherType.ToString());
    }
}
