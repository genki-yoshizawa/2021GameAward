using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisOutEffect : MonoBehaviour
{

	[SerializeField]
	private GameObject _Target;
	
	[SerializeField]
	private Material _IrisMat;

	[SerializeField]
	private float _OffsetY;

	void Start()
	{
	}

	void Update()
	{
		//Targetオブジェクト座標を画面座標に変換
		Vector3 target = _Target.gameObject.transform.position;
		target.y = _OffsetY;

		Vector3 screenPos;
		screenPos = Camera.main.WorldToScreenPoint(target);
		screenPos /= new Vector2(Screen.width, Screen.height);

		//IrisMatシェーダーに値を送る
		_IrisMat.SetVector("_PlayerPosition", screenPos);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, _IrisMat);
	}
}
