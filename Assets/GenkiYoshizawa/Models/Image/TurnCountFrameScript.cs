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

    private int _CurTurnCount = 0;

    private float _PassedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _TurnManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>().GetTurnManager().GetComponent<TurnManager>();

        List<int> number = new List<int>();
        int turnLimit = _TurnManagerScript.GetTurnLimit();
        int digit = turnLimit;
        while (turnLimit > 0)
        {
            turnLimit = digit % 10;
            digit = digit / 10;
            number.Add(turnLimit);
        }
        transform.GetChild(0).GetComponent<Image>().sprite = _NumberSprite[number[0]];
        transform.GetChild(1).GetComponent<Image>().sprite = _NumberSprite[number[1]];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
