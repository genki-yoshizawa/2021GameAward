using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private AudioManager _AudioManager;

    [SerializeField] AudioSource[] _Source;

    [SerializeField] private float _BgVolume;
    [SerializeField] private float _SeVolume;

    public static AudioManager Instance
    {
        get; private set;
    }

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _AudioManager = GetComponent<AudioManager>();

    }

    public void Start()
    {
        _Source = GetComponents<AudioSource>();

        _Source[0].volume = _BgVolume;
        _Source[0].loop = true;
        _Source[1].volume = _SeVolume;
        _Source[1].loop = false;
        //_Source[2].volume = _BgVolume;
    }

    public void Update()
    {
    }

    public void PlayBGM(AudioClip audioClip)
    {
        _Source[0].PlayOneShot(audioClip, _BgVolume);
    }

    public void PlaySE(AudioClip audioClip)
    {
        _Source[1].PlayOneShot(audioClip, _SeVolume);
    }

    public void SetBGMVol()
    {

    }
}
