using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketScript : MonoBehaviour
{
    [SerializeField]
    private Animator _RocketAnimator;

    [SerializeField]
    private AudioClip _StartButtonSound;
    [SerializeField]
    private AudioClip _RocketSound;

    [SerializeField]
    private Animator _FadeIn;

    void Start()
    {
        this.GetComponent<AudioSource>().volume = 0.125f;
        this.GetComponent<AudioSource>().playOnAwake = false;
        

        StartCoroutine("RocketLaunch");
    }


    public IEnumerator RocketLaunch()
    {
        while(!/*Input.GetKeyDown("joystick button 1")*/(Input.GetButtonDown("Controller_B") || Input.GetButtonDown("Controller_Menu") || Input.GetKeyDown(KeyCode.Return)))
        {
            yield return null;
        }

        this.GetComponent<AudioSource>().volume = 0.25f;
        this.GetComponent<AudioSource>().PlayOneShot(_StartButtonSound);

        yield return new WaitForSeconds(1.0f);

        _RocketAnimator.SetBool("Start", true);
        this.GetComponent<AudioSource>().volume = 0.125f;
        this.GetComponent<AudioSource>().PlayOneShot(_RocketSound);

        yield return new WaitForSeconds(1.0f);

        _FadeIn.SetBool("Start", true);
        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene("MenuScene");
    }
}
