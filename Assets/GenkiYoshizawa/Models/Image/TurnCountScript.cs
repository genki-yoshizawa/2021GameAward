using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCountScript : MonoBehaviour
{
    [Header("Assetから適用する数字テクスチャを入れてください")]
    [SerializeField] private Sprite[] _NumberSprite;
    private Image _MyImage;

    TurnManager _TurnManagerScript = null;

    // Start is called before the first frame update
    void Start()
    {
        _MyImage = GetComponent<Image>();

        _TurnManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>().GetTurnManager().GetComponent<TurnManager>();
        // TurnManagerに最大ターン数設定できるようにしてください
        //int turnCount = _TurnManagerScript.GetLimitTurn();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
