using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private AudioManager am;

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

        am = GetComponent<AudioManager>();
    }

    public void Start()
    {
        _Source = GetComponents<AudioSource>();

        //_Source[0].volume = _BgVolume;
        _Source[1].volume = _SeVolume;
        //_Source[2].volume = _BgVolume;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneManager.LoadScene("SceneA");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("SceneB");
        }
    }

    public void PlaySE(AudioClip audioClip)
    {
        _Source[1].PlayOneShot(audioClip, _SeVolume);
    }
}
