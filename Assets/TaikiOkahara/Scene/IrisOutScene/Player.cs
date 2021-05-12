using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 pos = transform.position;
		pos.x -= 0.005f;
		transform.position = pos;
	}
}
