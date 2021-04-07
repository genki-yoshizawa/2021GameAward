using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectSnap))]
[CanEditMultipleObjects]
public class SnapSampleEditor : Editor
{
    private ObjectSnap[] _Instances;
    private Vector3 _Center = Vector3.zero;

    private void OnEnable()
    {
        _Instances = targets.Cast<ObjectSnap>().ToArray();
    }

    /// <summary>
    /// �V�[���r���[��GUI
    /// </summary>
    private void OnSceneGUI()
    {
        Tools.current = Tool.None;

        _Center = GetCenterOfInstances(_Instances);

        // �t���[�n���h��
        FreeHandle();

        // X��
        AxisHandle(Color.red, Vector2Int.right);

        // Y��
        AxisHandle(Color.green, Vector2Int.up);
    }

    /// <summary>
    /// �t���[�n���h���̕`��
    /// </summary>
    private void FreeHandle()
    {
        Handles.color = Color.magenta;

        // �t���[�ړ��n���h���̍쐬
        EditorGUI.BeginChangeCheck();
        var pos = Handles.FreeMoveHandle(_Center, Quaternion.identity, 1f, Vector3.one,
            Handles.CircleHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            MoveObject(pos - _Center);
        }
    }

    /// <summary>
    /// �����̃C���X�^���X�̒��S��Ԃ�
    /// </summary>
    private static Vector3 GetCenterOfInstances(ObjectSnap[] instances)
    {
        float x = 0f, y = 0f;

        foreach (var ins in instances)
        {
            var position = ins.transform.position;
            x += position.x;
            y += position.y;
        }

        return new Vector3(x / instances.Length, y / instances.Length, 0);
    }

    /// <summary>
    /// ���n���h���̕`��
    /// </summary>
    private void AxisHandle(Color color, Vector2 direction)
    {
        // �n���h���̍쐬
        Handles.color = color;
        EditorGUI.BeginChangeCheck();
        var deltaMovement = Handles.Slider(_Center, new Vector3(direction.x, direction.y, 0)) - _Center;

        if (EditorGUI.EndChangeCheck())
        {
            var dot = Vector2.Dot(deltaMovement, direction);
            if (!(Mathf.Abs(dot) > Mathf.Epsilon)) return;

            MoveObject(dot * direction);
        }
    }

    /// <summary>
    /// �X�i�b�v���ăI�u�W�F�N�g�𓮂���
    /// </summary>
    private void MoveObject(Vector3 vec3)
    {
        float gridSize = Resources.Load<GameConfig>("ScriptableObject/GameConfig").GridSize;
        var vec2 = new Vector2Int(Mathf.RoundToInt(vec3.x / gridSize), Mathf.RoundToInt(vec3.y / gridSize));

        if (vec2 == Vector2.zero) return;

        foreach (var ins in _Instances)
        {
            Object[] objects = { ins, ins.transform };
            Undo.RecordObjects(objects, "�I�u�W�F�N�g�̈ړ�");
            ins.Move(vec2);
        }
    }
}