using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundScript : MonoBehaviour
{
    [Header("ï\ê¢äEÇÃîwåiâÊëú")]
    [SerializeField] private Sprite _FrontWorldImage;
    [Header("ó†ê¢äEÇÃîwåiâÊëú")]
    [SerializeField] private Sprite _BackWorldImage;

    private Image _MyImage;
    private GameObject _PlayerObject;

    // Start is called before the first frame update
    void Start()
    {
        _MyImage = GetComponent<Image>();
        _PlayerObject = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>().GetPlayer();

        _MyImage.sprite = _PlayerObject.transform.GetComponent<PlayerControl>().GetIsFront() ? _FrontWorldImage : _BackWorldImage;
    }

    // Update is called once per frame
    void Update()
    {
        _MyImage.sprite = _PlayerObject.transform.GetComponent<PlayerControl>().GetIsFront() ? _FrontWorldImage : _BackWorldImage;
    }
}
