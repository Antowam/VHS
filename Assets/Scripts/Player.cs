using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _baseSpeed = 2.0f;
    [SerializeField] private float _currentSpeed = 0.0f;
    [SerializeField] private float _speedLerpMultiplier = 0.1f;
    [SerializeField] private float _positionLerpMultiplier;
    [SerializeField] private float _decelerationMultiplier = 0.025f;
    [SerializeField] private float _MAX_SPEED = 10.0f;
    [SerializeField] public int playerIndex;
    public int GetPlayerIndex() { return playerIndex; }

    [Header("Animation")]
    [SerializeField] private float _startAnimationTime;
    [SerializeField] private Color _impactColor;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [HideInInspector] public bool _shouldAnimatePaddle = false;

    private Rigidbody2D rb;
    private Vector3 _previousScale;
    private Color _baseColor;
    private float _currentAnimationTime;
    private float _previousMoveSpeed = 0;
    private float _angle = 0;
    private int invertVal = 1;
    private bool haveSizeIncrease = false;
    private bool _movementIsActive;

    private delegate void EndPickupDelegate();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _currentSpeed = _baseSpeed;
        _currentAnimationTime = _startAnimationTime;
        _baseColor = _spriteRenderer.color;

        switch (GetPlayerIndex())
        {
            case 0:
                _angle = Mathf.PI;
                break;
            case 1:
                _angle = 0;
                break;
            case 2:
                _angle = Mathf.PI / 2;
                break;
            case 3:
                _angle = 3 * (Mathf.PI / 2);
                break;
            default:
                Debug.Log("Invalid Player index");
                break;
        }
    }
    void Update()
    {
        CalculateMovementSpeed();
        MovePlayer();
        SetPlayerRotation();

        if (_shouldAnimatePaddle)
            AnimatePaddleOnCollision();
    }
    void MovePlayer()
    {
        _movementIsActive = false;

        if ((playerIndex == 0 && Input.GetAxisRaw("Player1Horizontal") > 0) ||
            (playerIndex == 1 && Input.GetAxisRaw("Player2Horizontal") > 0) ||
            (playerIndex == 2 && Input.GetAxisRaw("Player3Horizontal") > 0) ||
            (playerIndex == 3 && Input.GetAxisRaw("Player4Horizontal") > 0))
        {
            _movementIsActive = true;
            _angle += _currentSpeed * Time.deltaTime * invertVal;
        }
        else if ((playerIndex == 0 && Input.GetAxisRaw("Player1Horizontal") < 0) ||
                (playerIndex == 1 && Input.GetAxisRaw("Player2Horizontal") < 0) ||
                (playerIndex == 2 && Input.GetAxisRaw("Player3Horizontal") < 0) ||
                (playerIndex == 3 && Input.GetAxisRaw("Player4Horizontal") < 0))
        {
            _movementIsActive = true;
            _angle -= _currentSpeed * Time.deltaTime * invertVal;
        }
        float newX = Mathf.Cos(_angle) * 4.75f;
        float newY = Mathf.Sin(_angle) * 4.75f;
        Vector3 temp = new Vector3(newX, newY, 0);

        transform.position = Vector3.Lerp(transform.position, temp, Mathf.Log(10) * _positionLerpMultiplier);
    }
    void SetPlayerRotation()
    {
        Vector2 difVector = Vector3.zero - transform.position;
        difVector.Normalize();
        float angle = Mathf.Atan2(difVector.y, difVector.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
    private void CalculateMovementSpeed()
    {
        if (_movementIsActive)
        {
            if (_currentSpeed <= _MAX_SPEED)
            {
                //_currentSpeed *= _accelerationMultiplier + _accelerationSmoothing;
                _currentSpeed = Mathf.Lerp(_currentSpeed, _MAX_SPEED, Mathf.Log(10) * _speedLerpMultiplier);
            }
            else
                _currentSpeed = _MAX_SPEED;
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, _baseSpeed, _speedLerpMultiplier);
            //_currentSpeed = _baseSpeed;

        }
    }
    private void AnimatePaddleOnCollision()
    {
        _spriteRenderer.color = _impactColor;
        _currentAnimationTime -= Time.deltaTime;

        if (_currentAnimationTime <= 0)
            ReverseAnimatePaddleOnCollision();
    }
    public void ReverseAnimatePaddleOnCollision()
    {
        _spriteRenderer.color = _baseColor;
        _currentAnimationTime = _startAnimationTime;
        _shouldAnimatePaddle = false;
    }

    public void SetPlayerWidthMultiplier(float multiplier, bool hasTimer = true, float affectedTime = 2)
    {
        _previousScale = transform.localScale;
        transform.localScale = new Vector3(transform.localScale.x * multiplier, transform.localScale.y, transform.localScale.z);

        if (hasTimer)
        {
            EndPickupDelegate del = ResetPlayerSize;
            StartCoroutine(PickupTimer(affectedTime, del));
        }
    }

    public void SetPlayerHeightMultiplier(float multiplier, bool hasTimer = true, float affectedTime = 2)
    {
        _previousScale = transform.localScale;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * multiplier, transform.localScale.z);

        if (hasTimer)
        {
            EndPickupDelegate del = ResetPlayerSize;
            StartCoroutine(PickupTimer(affectedTime, del));
        }
    }

    public void SetPlayerSizeMultiplier(float multiplier, bool hasTimer = true, float affectedTime = 2)
    {
        if (!haveSizeIncrease)
        {
        haveSizeIncrease = true;
        _previousScale = transform.localScale;
        transform.localScale = new Vector3(transform.localScale.x * multiplier, transform.localScale.y * multiplier, transform.localScale.z);

        if (hasTimer)
        {
            EndPickupDelegate del = EndPlayerSizePickup;
            StartCoroutine(PickupTimer(affectedTime, del));
        }
        }
    }

    private void EndPlayerSizePickup()
    {
        haveSizeIncrease = false;
        transform.localScale = _previousScale;
    }

    public void ResetPlayerSize()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }



    public void SetPlayerSpeedMultiplier(float multiplier, bool hasTimer = true, float affectedTime = 2)
    {
        _previousMoveSpeed = _baseSpeed;
        _currentSpeed *= multiplier;

        if (hasTimer)
        {
            EndPickupDelegate del = EndPlayerSpeedPickup;
            StartCoroutine(PickupTimer(affectedTime, del));
        }
    }

    public void SetPlayerSpeed(float newSpeed, bool hasTimer = true, float affectedTime = 2)
    {
        _previousMoveSpeed = _baseSpeed;
        _currentSpeed = newSpeed;

        if (hasTimer)
        {
            EndPickupDelegate del = EndPlayerSpeedPickup;
            StartCoroutine(PickupTimer(affectedTime, del));
        }
    }

    private void EndPlayerSpeedPickup()
    {
        _currentSpeed = _previousMoveSpeed;
    }

    public void ResetPlayerSpeed()
    {
        _currentSpeed = _baseSpeed;
    }

    public void SetPlayerControlsInverted(bool isInverted)
    {
        if (isInverted)
        {
            invertVal = -1;
        }
        else
        {
            invertVal = 1;
        }
    }

    IEnumerator PickupTimer(float timeAffected, EndPickupDelegate method)
    {
        yield return new WaitForSeconds(timeAffected);
        method();
    }
}
