using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOVerAnimationEvent : MonoBehaviour
{
    [SerializeField]
    GameOverManager gm;

    public void playFade()
    {
        gm.BeginCreditFade();
    }
}
