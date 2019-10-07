using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedDecrease : MonoBehaviour
{
    private EnableTextObject _playerSpeedDecreaseText;
    private float spinSpeed = 5;

    [SerializeField]
    private float newSpeed = 0.8f;
    [SerializeField]
    public float duration = 8;
    public bool hasTimer = true;

    public AudioClip[] clip;

    private void Start()
    {
        _playerSpeedDecreaseText = GameObject.FindGameObjectWithTag("Player Speed -").GetComponent<EnableTextObject>();
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
            _playerSpeedDecreaseText.SetTimer(2);

            //AudioHandler.instance.SoundQueue(AudioHandler.instance.queue04, clip);
            Destroy(gameObject);
        }
    }
}
