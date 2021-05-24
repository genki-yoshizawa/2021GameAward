using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCountFrameScript : MonoBehaviour
{
    [Header("Assetから適用する数字テクスチャを入れてください")]
    [SerializeField] private Sprite[] _NumberSprite;
    private List<GameObject> _MyImageObject;

    private bool _isTurnCountAnim = false;

    private TurnManager _TurnManagerScript = null;

    private int _TurnLimit = 0;
    private int _CurTurnCount = 0;

    private float _PassedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _TurnManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>().GetTurnManager().GetComponent<TurnManager>();

        List<int> number = new List<int>();
        int turnLimit = _TurnLimit = _TurnManagerScript.GetTurnLimit();
        int digit = turnLimit;
        while (digit > 0)
        {
            turnLimit = digit % 10;
            digit = digit / 10;
            number.Add(turnLimit);
        }
        Debug.Log(number[0]);
        Debug.Log(number[1]);
        transform.GetChild(0).GetComponent<Image>().sprite = _NumberSprite[number[0]];
        if (number.Count > 1)
            transform.GetChild(1).GetComponent<Image>().sprite = _NumberSprite[number[1]];
        else
            transform.GetChild(1).GetComponent<Image>().sprite = _NumberSprite[0];

    }

    // Update is called once per frame
    void Update()
    {
        int preTurnCount = _TurnManagerScript.GetTurnCount();

        // ターンに変化がなければなにもしない
        if (_CurTurnCount == preTurnCount)
            return;

        // 表示する数字
        List<int> number = new List<int>();
        int displayNumber = _TurnLimit - preTurnCount;
        int digit = displayNumber;
        while (digit > 0)
        {
            displayNumber = digit % 10;
            digit = digit / 10;
            number.Add(displayNumber);

        }
        transform.GetChild(0).GetComponent<Image>().sprite = _NumberSprite[number[0]];
        if(number.Count > 2)
            transform.GetChild(1).GetComponent<Image>().sprite = _NumberSprite[number[1]];
        else
            transform.GetChild(1).GetComponent<Image>().sprite = _NumberSprite[0];

        _CurTurnCount = preTurnCount;
    }
}
