using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSizeIncrease : MonoBehaviour
{
    private EnableTextObject _playerSizeIncreaseText;
    public float playerSizeMultiplier = 2;
    public float duration = 15;
    public bool hasTimer = true;

    public AudioClip[] clip;

    private void Start()
    {
        _playerSizeIncreaseText = GameObject.FindGameObjectWithTag("Player Size +").GetComponent<EnableTextObject>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Ball>() != null)
        {
            collision.GetComponentInParent<Ball>().latestBouncedPlayer?.SetPlayerSizeMultiplier(playerSizeMultiplier, hasTimer, duration);
            _playerSizeIncreaseText.SetTimer(2);

            AudioHandler.instance.SoundQueue(AudioHandler.instance.queue04, clip);
            Destroy(gameObject);
        }
    }
}
