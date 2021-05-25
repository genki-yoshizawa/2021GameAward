using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseStageNumber : MonoBehaviour
{
    public Sprite[] _NumberImages;

    [SerializeField]
    private Image _ScoreImage1, _ScoreImage2, _ScoreImage3;//ˆêŒ…–ÚA“ñŒ…–Ú


    public void SetScore(int score)
    {
        _ScoreImage1.enabled = true;
        _ScoreImage2.enabled = true;


        int score1 = 0, score2 = 0, score3 = 0;


        var digit = score;


        if (Digit(digit) == 2)
        {
            score2 = digit / 10;
            score1 = digit % 10;

            _ScoreImage1.sprite = _NumberImages[score2];
            _ScoreImage2.sprite = _NumberImages[score1];
            _ScoreImage3.sprite = _NumberImages[score1];

            _ScoreImage2.enabled = false;
        }
        else
        {
            score3 = digit / 100;
            score2 = (digit / 10) % 10;
            score1 = digit % 10;


            _ScoreImage1.sprite = _NumberImages[score3];
            _ScoreImage2.sprite = _NumberImages[score2];
            _ScoreImage3.sprite = _NumberImages[score1];
        }



        return;
    }

    int Digit(int num)
    {
        // Mathf.Log10(0)‚ÍNegativeInfinity‚ğ•Ô‚·‚½‚ßA•Ê“rˆ—‚·‚éB
        return (num == 0) ? 1 : ((int)Mathf.Log10(num) + 1);
    }

}