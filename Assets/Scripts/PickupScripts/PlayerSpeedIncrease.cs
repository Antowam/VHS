using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpeedIncrease : MonoBehaviour
{
    private EnableTextObject _playerSpeedIncreaseText;
    private float spinSpeed = 5;

    [SerializeField]
    private float newSpeed = 1.5f;
    [SerializeField]
    public float duration = 4;
    public bool hasTimer = true;

    public AudioClip[] clip;

    private void Start()
    {
        _playerSpeedIncreaseText = GameObject.FindGameObjectWithTag("Player Speed +").GetComponent<EnableTextObject>();
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, spinSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)                     
    {
        if (collision.GetComponentInParent<Ball>() != null)                 
        {
            collision.GetComponentInParent<Ball>().latestBouncedPlayer?.SetPlayerSpeedMultiplier(newSpeed, hasTimer, duration);
            _playerSpeedIncreaseText.SetTimer(2);

            AudioHandler.instance.SoundQueue(AudioHandler.instance.queue04, clip);
            Destroy(gameObject);    
        }
    }
}