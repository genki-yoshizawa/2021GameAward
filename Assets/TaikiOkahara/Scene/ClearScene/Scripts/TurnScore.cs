using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnScore : MonoBehaviour
{
    public Sprite[] _NumberImages;

    [SerializeField]
    private Image _ScoreImage1, _ScoreImage2;//ˆêŒ…–ÚA“ñŒ…–Ú


    public void SetScore(int score)
    {
        _ScoreImage1.enabled = true;
        _ScoreImage2.enabled = true;


        int score1 = 0, score2 = 0;
        var digit = score;

        if(digit != 0)
        {
            score1 = digit % 10;
            score2 = digit / 10;

            if (score2 == 0)
                _ScoreImage2.enabled = false;
        }
        
        _ScoreImage1.sprite = _NumberImages[score1];
        _ScoreImage2.sprite = _NumberImages[score2];

        return;
    }
    
}