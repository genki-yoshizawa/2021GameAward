using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCountScript : MonoBehaviour
{
    [Header("桁が一つ増えたときのテクスチャサイズ倍率")]
    [SerializeField] private float _ScalePerOneDigit = 0.75f;
    [Header("桁が一つ増えたときのずらすポジションの量(x方向)(良き値の組み合わせを探してください・・・)")]
    [SerializeField] private float _OffsetXPerOneDigit = 40.0f;

    [Header("ターン減少テクスチャを表示する秒数")]
    [SerializeField] private float _TurnCountAnimTime = 1.0f;
    [Header("エネミー減少テクスチャを表示する際の最大倍率")]
    [SerializeField] private float _TurnCountMaxScale = 1.0f;

    [Header("Assetから適用する数字テクスチャを入れてください")]
    [SerializeField] private Sprite[] _NumberSprite;
    private List<GameObject> _MyImageObject;

    private bool _isTurnCountAnim = false;

    // ターン更新前のスケール
    private Vector3 _StartLocalScale;

    // ターン更新前の桁数
    private int _PreDigit = 0;

    private TurnManager _TurnManagerScript = null;

    private int _CurTurnCount = 0;

    private float _PassedTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        _MyImageObject.Add(gameObject);

        _TurnManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>().GetTurnManager().GetComponent<TurnManager>();
        // TurnManagerに最大ターン数設定できるようにしてください
        List<int> number = new List<int>();
        int turnLimit = _TurnManagerScript.GetTurnLimit();
        int digit = turnLimit;
        while(turnLimit > 0)
        {
            turnLimit = digit % 10;
            digit = digit / 10;
            number.Add(turnLimit);
        }
        
        for (int i = 1; i < number.Count; ++i)
        {
            _MyImageObject.Add(Instantiate(_MyImageObject[0].gameObject));
            _MyImageObject[i].GetComponent<RectTransform>().localPosition = _MyImageObject[i].GetComponent<RectTransform>().localPosition + new Vector3(_OffsetXPerOneDigit * (number.Count - 1.0f - 2 * i), 0f, 0f);

            for (int j = 0; j < number.Count - j; ++i)
            {
                _MyImageObject[i].GetComponent<RectTransform>().localScale = _MyImageObject[i].GetComponent<RectTransform>().localScale * _ScalePerOneDigit;
            }
        }
        
        // Instantiateで0番基準なので0は最後に設定
        _MyImageObject[0].GetComponent<RectTransform>().localPosition = _MyImageObject[0].GetComponent<RectTransform>().localPosition + new Vector3(_OffsetXPerOneDigit * (number.Count - 1.0f), 0f, 0f);
        for(int i = 0; i < number.Count - 1; ++i)
        {
            _MyImageObject[0].GetComponent<RectTransform>().localScale = _MyImageObject[0].GetComponent<RectTransform>().localScale * _ScalePerOneDigit;
        }
        // テクスチャ設定
        for (int i = 0; i < number.Count; ++i)
            _MyImageObject[i].GetComponent<Image>().sprite = _NumberSprite[number[i]];

        _PreDigit = number.Count;
        _StartLocalScale = _MyImageObject[0].GetComponent<RectTransform>().localScale;
    }

    // Update is called once per frame
    void Update()
    {
        int preTurnCount = _TurnManagerScript.GetTurnCount();

        if(preTurnCount != _CurTurnCount)
        {
            _isTurnCountAnim = true;

            List<int> number = new List<int>();
            int turn = _TurnManagerScript.GetTurnLimit() - preTurnCount;
            int digit = turn;
            while (turn > 0)
            {
                turn = digit % 10;
                digit = digit / 10;
                number.Add(turn);
            }

            // 桁数が変化していたら
            if(_PreDigit != number.Count)
            {
                _MyImageObject[0].GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                _MyImageObject[0].GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

                for (int i = 1; i < number.Count; ++i)
                {
                    _MyImageObject[i].GetComponent<RectTransform>().localPosition = _MyImageObject[0].GetComponent<RectTransform>().localPosition + new Vector3(_OffsetXPerOneDigit * (number.Count - 1.0f - 2 * i), 0f, 0f);

                    for (int j = 0; j < number.Count - j; ++i)
                    {
                        _MyImageObject[i].GetComponent<RectTransform>().localScale = _MyImageObject[0].GetComponent<RectTransform>().localScale * _ScalePerOneDigit;
                    }
                }

                // Instantiateで0番基準なので0は最後に設定
                _MyImageObject[0].GetComponent<RectTransform>().localPosition = _MyImageObject[0].GetComponent<RectTransform>().localPosition + new Vector3(_OffsetXPerOneDigit * (number.Count - 1.0f), 0f, 0f);
                for (int i = 0; i < number.Count - 1; ++i)
                {
                    _MyImageObject[0].GetComponent<RectTransform>().localScale = _MyImageObject[0].GetComponent<RectTransform>().localScale * _ScalePerOneDigit;
                }
                // 前までの最上位桁を消す
                GameObject img = _MyImageObject[number.Count - 1];
                Destroy(_MyImageObject[number.Count - 1]);
                _MyImageObject.Remove(img);
            }

            // スケール保存用に保存
            _StartLocalScale = _MyImageObject[0].GetComponent<RectTransform>().localScale;

            // テクスチャ、サイズ設定
            for (int i = 0; i < number.Count; ++i)
            {
                _MyImageObject[i].GetComponent<Image>().sprite = _NumberSprite[number[i] + 10];
                _MyImageObject[i].GetComponent<RectTransform>().localScale = _TurnCountMaxScale * _MyImageObject[i].GetComponent<RectTransform>().localScale;
            }

            _PreDigit = number.Count;
        }

        if (_isTurnCountAnim)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _TurnCountAnimTime)
            {
                List<int> number = new List<int>();
                int turn = _TurnManagerScript.GetTurnLimit() - preTurnCount;
                int digit = turn;
                while (turn > 0)
                {
                    turn = digit % 10;
                    digit = digit / 10;
                    number.Add(turn);
                }

                for (int i = 0; i < number.Count; ++i)
                {
                    _MyImageObject[i].GetComponent<Image>().sprite = _NumberSprite[number[i] + 10];
                    _MyImageObject[i].GetComponent<RectTransform>().localScale = _TurnCountMaxScale * _MyImageObject[i].GetComponent<RectTransform>().localScale;
                }
                _PassedTime = 0.0f;
                _isTurnCountAnim = false;
            }
        }

        _CurTurnCount = preTurnCount;
    }
}
