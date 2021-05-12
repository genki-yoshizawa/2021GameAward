using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisOutEffect : MonoBehaviour
{

	[SerializeField]
	private GameObject _Target;
	
	[SerializeField]
	private Material _IrisMat;

	void Start()
	{
	}

	void Update()
	{
		//Target�I�u�W�F�N�g���W����ʍ��W�ɕϊ�
		Vector3 screenPos;
		screenPos = Camera.main.WorldToScreenPoint(_Target.gameObject.transform.position);
		screenPos /= new Vector2(Screen.width, Screen.height);

		//IrisMat�V�F�[�_�[�ɒl�𑗂�
		_IrisMat.SetVector("_PlayerPosition", screenPos);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, _IrisMat);
	}
}
