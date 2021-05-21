using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClearTransition : MonoBehaviour
{
    [SerializeField]
    private Material _TransitionIn;

    void Start()
    {
        StartCoroutine(BeginTransition());
    }
 
    IEnumerator BeginTransition()
    {
        yield return Animate(_TransitionIn, 1);
    }

    IEnumerator Animate(Material material,float time)
    {
        GetComponent<Image>().material = material;
        float current = 0;
        while(current < time)
        {
            material.SetFloat("_Alpha", current / time);
            yield return new WaitForEndOfFrame();
            current += Time.deltaTime;
        }
        material.SetFloat("_Alpha", 1);
    }

}
