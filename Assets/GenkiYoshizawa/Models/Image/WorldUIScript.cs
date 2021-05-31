using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIScript : MonoBehaviour
{
    [Header("Assetから適用する裏表テクスチャを入れてください")]
    [SerializeField] private Sprite _FrontSprite;
    [SerializeField] private Sprite _BackSprite;

    [Header("Assetから適用する裏表テクスチャを入れてください")]
    [SerializeField] private CameraWork _CameraWorkScript;

    private PlayerControl _PlayerScript;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_CameraWorkScript.GetIsFront())
        {
            gameObject.GetComponent<Image>().sprite = _FrontSprite;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = _BackSprite;
        }
    }
}
