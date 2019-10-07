using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingScript : MonoBehaviour
{

    [Header("Post Process")]
    [SerializeField] private PostProcessVolume volume;

    [Header("Chromatic Aberration")]
    ChromaticAberration chromaticLayer = null;
    [SerializeField] private float _multiplier;
    [SerializeField] private float _intensity;
    [SerializeField] private float _startVal;
    [SerializeField] private float _minVal = 0.1f;
    [SerializeField] private float _maxVal = 1.0f;

    [Header("Animation")]
    [SerializeField] private float _startAnimationTime;
    [SerializeField] private bool _shouldAnimate;
    [SerializeField] private bool _animateOnce;
    private float _currentAnimationTime = 0.5f;

    void Start()
    {
        _currentAnimationTime = _startAnimationTime;

        PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out chromaticLayer);
        chromaticLayer.intensity.value = Mathf.Lerp(chromaticLayer.intensity.value, _maxVal, Mathf.Log(_startVal * Time.deltaTime));
    }

    void Update()
    {
        ApplyChromaticFilter();
    }
    private void ApplyChromaticFilter()
    {
        if (_currentAnimationTime <= 0)
        {
            if (_shouldAnimate)
            {
                _shouldAnimate = false;
                _currentAnimationTime = _startAnimationTime;
            }
            else
            {
                _shouldAnimate = true;
                _currentAnimationTime = _startAnimationTime;
            }
        }

        _currentAnimationTime -= Time.deltaTime;
        _intensity = chromaticLayer.intensity.value;
        if(_shouldAnimate)
            chromaticLayer.intensity.value = Mathf.Lerp(_intensity, _minVal, Mathf.Log(_multiplier));
        else
            chromaticLayer.intensity.value = Mathf.Lerp(_intensity, _maxVal, Mathf.Log(_multiplier));
    }
}
