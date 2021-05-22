using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class Gauss : MonoBehaviour
{
    Material _Material;

    [SerializeField]
    [Range(0, 20)]
    int _Resolution = 0;

    public int Resolution { get { return _Resolution; } set { _Resolution = value; } }

    void Awake()
    {
        var shader = Shader.Find("Hidden/Gauss");
        _Material = new Material(shader);
    }

    // ����̓V�F�[�_�[�ɒ萔���x�^�ł����Ă��邽�ߖ��g�p
    // �V�F�[�_�[���ł�CalcWeight(3.0f, 4)�̒l���g�p
    float[] CalcWeight(float dispersion, int count)
    {
        float[] weight = new float[count];
        float total = 0;
        for (int i = 0; i < weight.Length; i++)
        {
            weight[i] = Mathf.Exp(-0.5f * (i * i) / dispersion);
            total += weight[i] * ((0 == i) ? 1 : 2);
        }
        for (int i = 0; i < weight.Length; ++i) weight[i] /= total;
        return weight;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Resolution == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }
        CommandBuffer command = new CommandBuffer();

        int rt1 = Shader.PropertyToID("RT1");
        command.GetTemporaryRT(rt1, -_Resolution, -_Resolution, 0, FilterMode.Point);
        int rt2 = Shader.PropertyToID("rt2");
        command.GetTemporaryRT(rt2, -_Resolution, -_Resolution, 0, FilterMode.Trilinear);

        var weight = CalcWeight(4, 8);
        command.SetGlobalVector(Shader.PropertyToID("_PixelSize"), new Vector4((float)_Resolution / Screen.width, (float)_Resolution / Screen.height, 0, 0));

        command.Blit((RenderTargetIdentifier)source, rt1, _Material, 0);
        command.Blit(rt1, rt2, _Material, 1);
        command.Blit(rt2, destination);

        Graphics.ExecuteCommandBuffer(command);
    }


    void Update()
    {
        Camera cam = Camera.main;
        this.transform.position = cam.transform.position;
        this.transform.rotation = cam.transform.rotation;
    }
}