using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpeedDecrease : MonoBehaviour
{
    public float SpeedMultiplier = 0.8f;
    public EnableTextObject _ballSpeedDecreaseText;

    public AudioClip clip;

    private void Start()
    {
        _ballSpeedDecreaseText = GameObject.FindGameObjectWithTag("Ball Speed - ").GetComponent<EnableTextObject>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Ball>() != null)
        {
            collision.GetComponentInParent<Ball>().ApplySpeedMultiplier(SpeedMultiplier);
            _ballSpeedDecreaseText.SetTimer(2);

            AudioHandler.instance.SoundQueue(AudioHandler.instance.queue04, clip);
            Destroy(this.gameObject);
        }
    }
}
