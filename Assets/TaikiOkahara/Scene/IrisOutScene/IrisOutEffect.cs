using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisOutEffect : MonoBehaviour
{

	[SerializeField]
	private GameObject _Target;
	
	//[SerializeField]
	//private Material _IrisMat;

	[SerializeField]
	private float _OffsetY;

	[SerializeField]
	private GameObject _IrisTexture;

	void Start()
	{
	}

	void Update()
	{
		//Target�I�u�W�F�N�g���W����ʍ��W�ɕϊ�
		Vector3 target = _Target.gameObject.transform.position;
		target.y = _OffsetY;

		Vector3 screenPos;
		screenPos = Camera.main.WorldToScreenPoint(target);
		//screenPos /= new Vector2(Screen.width, Screen.height);

		//IrisMat�V�F�[�_�[�ɒl�𑗂�
		//_IrisMat.SetVector("_PlayerPosition", screenPos);

		_IrisTexture.transform.position = screenPos;
	}

	//void OnRenderImage(RenderTexture src, RenderTexture dest)
	//{
	//	Graphics.Blit(src, dest, _IrisMat);
	//}
}
