using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private AudioManager _AudioManager;

    [SerializeField] AudioSource[] _Source;

    [Header("����"),SerializeField] private float _SeVolume;
    [SerializeField] private float _BgVolume;

    [Header("�t�F�[�h�A�E�g�܂łɉ��b�����邩"), SerializeField]
    private float _FadeOutTime;
    private float _PassedTime = 0.0f;
    private bool _IsFadeOut = false;


    private float _StartSEVolume;
    private float _StartBGMVolume;


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

        _Source[0].volume = _SeVolume;
        _Source[0].loop = false;

        _Source[1].volume = _BgVolume;
        _Source[1].loop = true;

        _Source[2].volume = 0.0f;
        _Source[2].loop = true;

        _StartBGMVolume = _BgVolume;
        _StartSEVolume = _SeVolume;
    }

    public void Update()
    {
        if (_IsFadeOut)
        {
            float time = Time.deltaTime;
            if((_PassedTime += time) > _FadeOutTime)
            {
                _Source[1].Stop();
                //_Source[1].volume = _BgVolume;
                _Source[1].volume = _StartBGMVolume;
                _Source[2].Stop();
                _Source[2].volume = 0.0f;

                _IsFadeOut = false;
            }
            if (_Source[1].volume <= 0.01f)  //�ۂߌ덷�x��
                _Source[2].volume = _BgVolume * (1 - _PassedTime / _FadeOutTime);
            else
                _Source[1].volume = _BgVolume * (1 - _PassedTime / _FadeOutTime);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        _Source[1].clip = clip;
        _Source[1].Play();
    }

    //�Q�[����ʗp��PlayBGM�֐�
    public void PlayGameBGM(AudioClip clip1, AudioClip clip2)
    {
        if (_Source[1].isPlaying)
            _Source[1].UnPause();

        if (_Source[2].isPlaying)
            _Source[2].UnPause();

        _Source[1].clip = clip1;
        _Source[2].clip = clip2;

        _Source[1].Play();
        _Source[2].Play();
    }
    
    //���]���Ă���Ăяo���Ă�������
    public void ReverseBGM(bool isFront)
    {
        if (isFront)
        {
            _Source[1].volume = _BgVolume;
            _Source[2].volume = 0.0f;
        }
        else
        {
            _Source[1].volume = 0.0f;
            _Source[2].volume = _BgVolume;
        }

    }

    public void PlaySE(AudioClip audioClip)
    {
        _Source[0].PlayOneShot(audioClip, _SeVolume);
    }

    public void SetBGMVol(float vol)
    {
        _BgVolume = vol;
    }

    public void ResetBGMVol()
    {
        _BgVolume = _StartBGMVolume;
    }

    public void SetSEVol( float vol) {_SeVolume = vol; }

    public void ResetSEVol() {_SeVolume = _StartSEVolume; }

    public void StopBGM()
    {
        _IsFadeOut = true;
    }

    public void PauseBGM()
    {
        _Source[0].Pause();
        _Source[1].Pause();
    }

    public void UnpauseBGM()
    {
        _Source[0].UnPause();
        _Source[1].UnPause();
    }
}
 