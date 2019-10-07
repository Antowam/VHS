using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSizeOnSpawn : MonoBehaviour
{
    [SerializeField] private float _multiplier;
    [SerializeField] private float _maxSize;
    private Vector3 _targetScale;

    private void Start()
    {
        _targetScale = new Vector3(0.25f, 0.25f, 0.25f);
    }
    void Update()
    {
        Vector3 _currentScale = transform.localScale;
        transform.localScale = Vector3.Lerp(_currentScale, _targetScale, Mathf.Log(_multiplier));
        //if (transform.localScale.x < _maxSize)
    }
}
