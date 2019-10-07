using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpeedIncrease : MonoBehaviour
{
    [SerializeField] private float _speedMultiplier;
    private EnableTextObject _ballSpeedIncreaseText;
    public AudioClip[] clip;

    void Start()
    {
        _ballSpeedIncreaseText = GameObject.FindGameObjectWithTag("Ball Speed +").GetComponent<EnableTextObject>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Ball>() != null)
        {
            collision.GetComponentInParent<Ball>().ApplySpeedMultiplier(_speedMultiplier);
            _ballSpeedIncreaseText.SetTimer(2);

            AudioHandler.instance.SoundQueue(AudioHandler.instance.queue04, clip);
            Destroy(this.gameObject);
        }
    }
}
