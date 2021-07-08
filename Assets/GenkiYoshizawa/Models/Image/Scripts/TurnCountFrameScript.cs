using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCountFrameScript : MonoBehaviour
{
    [Header("Assetから適用する数字テクスチャを入れてください")]
    [SerializeField] private Sprite[] _NumberSprite;
    [Header("ターン減少テクスチャを表示(光らせる)する秒数")]
    [SerializeField] private float _TurnCountAnimTime = 1.0f;
    [Header("10以上時の数字同士の距離(中心同士)")]
    [SerializeField] private float _NemuberTextureOffset = 10.0f;

    GameObject _NumberTextureOneDigit;
    GameObject _NumberTextureTwoDigit;

    Vector3 _NumberLocalPosition;

    private bool _isTurnCountAnim = false;

    private TurnManager _TurnManagerScript = null;

    private int _TurnLimit = 0;
    private int _CurTurnCount = 0;

    private float _PassedTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        _TurnManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>().GetTurnManager().GetComponent<TurnManager>();

        _NumberTextureOneDigit = transform.GetChild(0).gameObject;
        _NumberTextureTwoDigit = transform.GetChild(1).gameObject;

        _NumberLocalPosition = _NumberTextureOneDigit.transform.localPosition;

        int turnLimit = _TurnLimit = StageManager._MaxTurn;
        DisplayTurn(turnLimit, false);

        
    }

    // Update is called once per frame
    void Update()
    {
        int preTurnCount = _TurnManagerScript.GetTurnCount();

        // ターンに変化がなければなにもしない
        if (_CurTurnCount != preTurnCount)
        {
            // 表示する数字
            int displayNumber = _TurnLimit - preTurnCount;
            DisplayTurn(displayNumber, true);

            _isTurnCountAnim = true;
        }

        if (_isTurnCountAnim)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _TurnCountAnimTime)
            {
                //gameObject.GetComponent<RectTransform>().localScale = _StartLocalScale;
                // 表示する数字
                List<int> number = new List<int>();
                int displayNumber = _TurnLimit - preTurnCount;
                DisplayTurn(displayNumber, false);

                _isTurnCountAnim = false;
                _PassedTime = 0f;
            }
        }

        _CurTurnCount = preTurnCount;
    }

    // 数字を表示するための関数（引数で光らせるか決める）(二桁以下前提)
    private void DisplayTurn(int displayNumber, bool isLight)
    {
        // 表示する数字
        List<int> number = new List<int>();
        int digit = displayNumber;
        if (digit <= 0)
        {
            number.Add(displayNumber);
        }
        while (digit > 0)
        {
            displayNumber = digit % 10;
            digit = digit / 10;
            number.Add(displayNumber);
        }

        // 一桁目の数字の設定
        if (number[0] < 0)
            _NumberTextureOneDigit.GetComponent<Image>().sprite = _NumberSprite[0];
        else                      
            _NumberTextureOneDigit.GetComponent<Image>().sprite = _NumberSprite[number[0] + (isLight ? 10 : 0)];

        // 二桁目の数字の設定
        if (number.Count > 1)
        { //桁が二桁
            _NumberTextureTwoDigit.SetActive(true);
            _NumberTextureTwoDigit.GetComponent<Image>().sprite = _NumberSprite[number[1] + (isLight ? 10 : 0)];
            _NumberTextureOneDigit.GetComponent<RectTransform>().localPosition = new Vector3(_NumberLocalPosition.x + _NemuberTextureOffset * 0.5f, _NumberLocalPosition.y, _NumberLocalPosition.z);
            _NumberTextureTwoDigit.GetComponent<RectTransform>().localPosition = new Vector3(_NumberLocalPosition.x - _NemuberTextureOffset * 0.5f, _NumberLocalPosition.y, _NumberLocalPosition.z);
        }
        else
        { //桁が一桁
            //_NumberTextureTwoDigit.sprite = _NumberSprite[0 + (isLight ? 10 : 0)];
            _NumberTextureOneDigit.GetComponent<RectTransform>().localPosition = _NumberLocalPosition;
            _NumberTextureTwoDigit.SetActive(false);
        }
    }
}
