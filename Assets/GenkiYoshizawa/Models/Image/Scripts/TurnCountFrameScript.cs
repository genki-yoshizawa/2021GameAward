using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCountFrameScript : MonoBehaviour
{
    [Header("Asset����K�p���鐔���e�N�X�`�������Ă�������")]
    [SerializeField] private Sprite[] _NumberSprite;
    [Header("�^�[�������e�N�X�`����\������b��")]
    [SerializeField] private float _TurnCountAnimTime = 1.0f;

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
        
        int turnLimit = _TurnLimit = StageManager._MaxTurn;
        DisplayTurn(turnLimit, false);

    }

    // Update is called once per frame
    void Update()
    {
        int preTurnCount = _TurnManagerScript.GetTurnCount();

        // �^�[���ɕω����Ȃ���΂Ȃɂ����Ȃ�
        if (_CurTurnCount != preTurnCount)
        {
            // �\�����鐔��
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
                // �\�����鐔��
                List<int> number = new List<int>();
                int displayNumber = _TurnLimit - preTurnCount;
                DisplayTurn(displayNumber, false);

                _isTurnCountAnim = false;
            }
        }

        _CurTurnCount = preTurnCount;
    }

    // ������\�����邽�߂̊֐��i�����Ō��点�邩���߂�j
    private void DisplayTurn(int displayNumber, bool isLight)
    {
        // �\�����鐔��
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
        if (number[0] < 0)
            transform.GetChild(0).GetComponent<Image>().sprite = _NumberSprite[0];
        else
            transform.GetChild(0).GetComponent<Image>().sprite = _NumberSprite[number[0] + (isLight ? 10 : 0)];
        if (number.Count > 1)
            transform.GetChild(1).GetComponent<Image>().sprite = _NumberSprite[number[1] + (isLight ? 10 : 0)];
        else
            transform.GetChild(1).GetComponent<Image>().sprite = _NumberSprite[0 + (isLight ? 10 : 0)];
    }
}
