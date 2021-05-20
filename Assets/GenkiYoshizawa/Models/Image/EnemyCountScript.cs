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

    // Start is called before the first frame update
    void Start()
    {
        _MyImage = GetComponent<Image>();
        
        _Enemys = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>().GetEnemys();

        _MyImage.sprite = _NumberSprite[_Enemys.Count];
    }

    // Update is called once per frame
    void Update()
    {
        _MyImage.sprite = _NumberSprite[_Enemys.Count];
    }
}
