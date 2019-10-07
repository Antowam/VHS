using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnableTextObject : MonoBehaviour
{
    public Text text;

    [SerializeField] private Color _enabledColor;
    [SerializeField] private Color _disabledColor;
    private Color _currentColor;

    private bool _isEnabled;
    private float _minAlphaValue;
    private float _maxAlphaValue;

    [SerializeField] private float _timer = 0.0f;
    [SerializeField] private float _multiplier;
    private float _timeLeft;

    private void Start()
    {
        _timer = 0;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
            SetTimer(5);

        if(_timer > 0)
            _timer -= Time.deltaTime;

        if (_timer >= 0.5)
            EnableText();
        else
            DisableText();
    }
    public void SetTimer(float val)
    {
        _timer = val;
    }

    public void EnableText()
    {
        _currentColor = text.GetComponent<Text>().color;
        text.GetComponent<Text>().color = Color.Lerp(_currentColor, _enabledColor, Mathf.Log(_multiplier));
    }
    public void DisableText()
    {
        _currentColor = text.GetComponent<Text>().color;
        text.GetComponent<Text>().color = Color.Lerp(_currentColor, _disabledColor, Mathf.Log(_multiplier));
    }
}
