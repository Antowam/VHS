using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [HideInInspector] public GameManager gm;
    [HideInInspector] public bool rotateTowardsVelocity;
    [HideInInspector] public Player latestBouncedPlayer { get; private set; }

    private bool _shouldAnimateBall;
    private int _currentAnimationColorIndex = 0;
    private float _startAnimationTime;
    private float _currentAnimationTime;
    private Color _baseColor;

    private Rigidbody2D rb;
    private TrailRenderer tr;
    private SpriteRenderer sr;
    private ParticleSystem bs;

    [Header("Stats")]
    public float ballSpeed;
    public float ballSpeedIncrease = 0.2f;
    private float ballSpeedDelta = 0;
    private float initBallSpeed;
    private int bounces = 0;

    [Header("Animation")]
    [SerializeField] public Color _lightImpactColor;
    [SerializeField] public Color _heavyImpactColor;
    [SerializeField] public SpriteRenderer _spriteRenderer;

    [Header("Images")]
    [SerializeField] public GameObject _deathImage;
    [SerializeField] private Sprite[] ballSprites;

    [Header("Audio")]
    public AudioClip[] pongSounds;
    public AudioClip[] humanSounds;
    private bool _shouldApplySpeedDecrease;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        sr = GetComponentInChildren<SpriteRenderer>();

        _currentAnimationTime = _startAnimationTime;
        _baseColor = _spriteRenderer.color;
        initBallSpeed = ballSpeed;

        Launch();
    }

    void Update()
    {
        if ((transform.position - Vector3.zero).magnitude > 6f)
            DestroyBall();

        if (rotateTowardsVelocity)
        {
            Vector3 velDirection = rb.velocity.normalized;
            float angle = Mathf.Atan2(velDirection.y, velDirection.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.AngleAxis(angle - 90, transform.forward);

            Debug.DrawLine(transform.position, transform.position + velDirection * 5);
        }

        if (_shouldAnimateBall)
            AnimateBallOnCollision();
    }

    void Launch()
    {
        ballSpeedDelta = 0;
        float randomAngle = Random.Range(0f, Mathf.PI * 2);
        rb.velocity = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized * ballSpeed;
        SpawnParticles();
    }
    private void SpawnParticles()
    {
        bs = GetComponent<ParticleSystem>();
        bs.Play();
        bs.Clear();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        bounces++;
        SpawnParticles();

        // Selecting index to deduce if it is a light or heavy hit, and if sprites are to be changed.
        _currentAnimationColorIndex = 0;
        _startAnimationTime = 0.1f;
        if (bounces % 2 == 0)
        {
            if (bounces / 2 < ballSprites.Length)
            {
                _currentAnimationColorIndex = 1;
                _startAnimationTime = 0.2f;
                sr.sprite = ballSprites[bounces / 2];
            }
        }
        _shouldAnimateBall = true;
        
        rb.velocity *= (1 + ballSpeedIncrease);
        ballSpeedDelta += ballSpeedIncrease;

        if (collision.gameObject.GetComponent<Player>() != null)
        {
            latestBouncedPlayer = collision.gameObject.GetComponent<Player>();
            latestBouncedPlayer._shouldAnimatePaddle = true;
            AudioHandler.instance.SoundQueue(AudioHandler.instance.queue01, pongSounds);

            gm.IncreaseScore(1);
            gm.IncreaseBps(1);
            gm.AnimatePortrait(latestBouncedPlayer);
        }

        if (bounces >= 8)
        {
            gm.SpawnBall();
            gm.SpawnBall();

            SpawnDeathImage();
            DestroyBall();
        }
    }
    private void AnimateBallOnCollision()
    {
        if (_currentAnimationColorIndex == 0) 
            _spriteRenderer.color = _lightImpactColor;
        else
            _spriteRenderer.color = _heavyImpactColor;

        _currentAnimationTime -= Time.deltaTime;
        if (_currentAnimationTime <= 0)
            ReverseAnimateBallOnCollision();
    }
    public void ReverseAnimateBallOnCollision()
    {
        _spriteRenderer.color = _baseColor;
        _currentAnimationTime = _startAnimationTime;
        _shouldAnimateBall = false;
    }

    public void DestroyBall()
    {
        AudioHandler.instance.SoundQueue(AudioHandler.instance.queue02, humanSounds);
        gm.DecreaseActiveBalls();
        SpawnParticles();
        Destroy(this.gameObject);
    }

    public void SetBps()
    {
        gm.SetBpsMultiplierText();
    }

    private void SpawnDeathImage()
    {
        GameObject instance = Instantiate(_deathImage, transform.position, new Quaternion(Random.Range(0, 360), Random.Range(0, 360), 0, 1));
        gm.AddDeathImageToList(instance);
    }

    public void ApplySpeedMultiplier(float multiplier)
    {
        rb.velocity *= multiplier;
        ballSpeedDelta += (multiplier - 1);
    }
}
