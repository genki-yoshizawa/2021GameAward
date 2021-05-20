using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCountScript : MonoBehaviour
{
    // ����1�����̓G�����Ή��ł��Ȃ�
    [Header("Asset����K�p���鐔���e�N�X�`�������Ă�������")]
    [SerializeField] private Sprite[] _NumberSprite;
    private Image _MyImage;

    private List<GameObject> _Enemys = null;

    private int _CurEnemyCount;

    [Header("�G�l�~�[�����A�j���[�V�����ɂ�����b��")]
    [SerializeField] private float _EnemyCountAnimTime = 1.0f;
    [Header("�G�l�~�[�����A�j���[�V�����̍ۂ̍ő�{��")]
    [SerializeField] private float _EnemyCountMaxScale = 1.0f;
    private bool _isEnemyCountAnim = false;

    private Vector3 _StartLocalScale;

    private float _PassedTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        _MyImage = GetComponent<Image>();
        
        _Enemys = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>().GetEnemys();

        _MyImage.sprite = _NumberSprite[_Enemys.Count];
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
        }

        if (_isEnemyCountAnim)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _EnemyCountAnimTime) 
            {
                gameObject.GetComponent<RectTransform>().localScale = _StartLocalScale;
                _PassedTime = 0.0f;
                _isEnemyCountAnim = false;
            }


            _MyImage.sprite = _NumberSprite[_Enemys.Count + 10];
        }
        else
        {
            _MyImage.sprite = _NumberSprite[_Enemys.Count];
        }

        _CurEnemyCount = _Enemys.Count;
    }
}
