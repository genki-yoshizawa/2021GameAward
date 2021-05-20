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
