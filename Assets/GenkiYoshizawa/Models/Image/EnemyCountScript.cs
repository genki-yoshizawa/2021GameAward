using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCountScript : MonoBehaviour
{
    // 現状1桁数の敵しか対応できない
    [Header("Assetから適用する数字テクスチャを入れてください")]
    [SerializeField] private Sprite[] _NumberSprite;
    private Image _MyImage;

    private List<GameObject> _Enemys = null;

    private int _CurEnemyCount;

    [Header("エネミー減少テクスチャを表示する秒数")]
    [SerializeField] private float _EnemyCountAnimTime = 1.0f;
    [Header("エネミー減少テクスチャを表示する最大倍率")]
    [SerializeField] private float _EnemyCountMaxScale = 1.0f;
    private bool _isEnemyCountAnim = false;

    private Vector3 _StartLocalScale;

    // スプライトを更新するべきかのフラグ(眠気に任せて書いたので変な名前)
    private bool _SpriteUpdate = false;

    private float _PassedTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        _MyImage = GetComponent<Image>();
        
        _Enemys = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>().GetEnemys();

        _MyImage.sprite = _NumberSprite[_Enemys.Count];
        _SpriteUpdate = false;
        _CurEnemyCount = _Enemys.Count;

        _StartLocalScale = gameObject.GetComponent<RectTransform>().localScale;
        _PassedTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(_Enemys.Count != _CurEnemyCount)
        {
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(_EnemyCountMaxScale, _EnemyCountMaxScale, _EnemyCountMaxScale);
            _isEnemyCountAnim = true;
            _SpriteUpdate = true;

            _MyImage.sprite = _NumberSprite[_Enemys.Count + 10];
        }

        if (_isEnemyCountAnim)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _EnemyCountAnimTime) 
            {
                gameObject.GetComponent<RectTransform>().localScale = _StartLocalScale;
                _MyImage.sprite = _NumberSprite[_Enemys.Count];
                _PassedTime = 0.0f;
                _isEnemyCountAnim = false;
            }
        }

        _CurEnemyCount = _Enemys.Count;
    }
}
