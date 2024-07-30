using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField]
    GameObject goodEnd, badEnd;

    [SerializeField]
    Image deathFadeImage, creditImage;

    [SerializeField]
    Color32 goodColor, badColor, neutralColor;

    Color32 goalColor;

    bool fading;
    [SerializeField]
    float fadeDuration;
    float fadeTimer;

    [SerializeField]
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        goodEnd.SetActive(false);
        badEnd.SetActive(false);
        fading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            CreditFade();
        }
    }

    public void Fade()
    {

    }

    public void PlayGoodEnd()
    {
        goodEnd.SetActive(true);
        goalColor = goodColor;
        player.gameOver = true;
    }

    public void PlayBadEnd()
    {
        badEnd.SetActive(true);
        goalColor = badColor;
        player.gameOver = true;
    }

    public void playNeutralEnd()
    {
        player.gameOver = true;
        goalColor = neutralColor;
        BeginCreditFade();
    }

    public void BeginCreditFade()
    {
        fading = true;
        fadeTimer = 0;

        Color32 startColor = goalColor;
        goalColor.a = 0;
    }

    public void CreditFade()
    {
        fadeTimer += Time.deltaTime;

        Color titleColor = goalColor;
        titleColor.a = Mathf.Lerp(0, 1, (fadeTimer) / fadeDuration);
        Color fadeColor = Color.black;
        fadeColor.a = Mathf.Lerp(0, 1, fadeTimer / fadeDuration);
        deathFadeImage.color = fadeColor;
        creditImage.color = titleColor;
    }
}
