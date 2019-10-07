using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallCountdown : MonoBehaviour
{
    public GameManager gm;

    public void CallCountdownFromGameManager()
    {
        gm.SetCountDown();
    }
}
