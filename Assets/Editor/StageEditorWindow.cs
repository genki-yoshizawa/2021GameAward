//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;



//public class StageEditorWindow : Editor
//{

	
//	[MenuItem("Editor/StageEditorWindow")]
//	private static void Create()
//	{
//		// ����
//		EditorWindow.GetWindow<BlockWindow>("BlockEditor").Init();
//		EditorWindow.GetWindow<FaceWindow>("FaceEditor", typeof(BlockWindow));	
//	}
//}

//public class BlockWindow : EditorWindow
//{
//	/// <summary>
//	/// �u���b�N�G�f�B�^
//	/// </summary>

//	// �}�b�v�G�f�B�^�̃}�X�̐�
//	private int _MapSize = 3;
//	// �O���b�h�̑傫���A�������قǍׂ����Ȃ�
//	private float _GridSize = 100.0f;
//	// �O���b�h�̎l�p
//	private Rect[,] _GridRect;

//	private Texture2D[,] _BlockTex = new Texture2D[3,3];
//	private Rect[,] _BlockPos = new Rect[3,3];

//	private Vector2 _Center;

//	private bool _BlockChoice = false;

//	private Object _Block;

//	private Vector2 _ClickPos;

//	private List<GameObject> _Palette = new List<GameObject>();

//	//�u���b�N�̃v���n�u���擾����t�H���_�p�X
//	private string _Path = "Assets/TaikiOkahara/StageBlock";

//	private int _PaletteIndex;

//	bool _PaintMode;

//	private Texture2D _ChoicePrefabTex;


//	/// <summary>
//	/// �G�f�B�^�ݒ�
//	/// </summary>

//	//�X�N���[���ʒu
//	private Vector2 _ScrollPosition = Vector2.zero;

//	/// <summary>
//	/// �֐�
//	/// </summary>

//	private void OnFocus()
//    {
//		RefreshPalette();
//    }
//    public void Init()
//	{
//		_Center.x = position.size.x * 0.5f - (_GridSize * _MapSize * 0.5f);
//		_Center.y = 50.0f;

//		// �}�b�v�f�[�^��������
//		for (int i = 0; i < _MapSize; i++)
//		{
//			for (int j = 0; j < _MapSize; j++)
//			{
//				_BlockTex[i, j] = null;
//			}
//		}
//	}

//	private void RefreshPalette()
//	{
//		_Palette.Clear();

//		string[] prefabFiles = System.IO.Directory.GetFiles(_Path, "*.prefab");
//		foreach (string prefabFile in prefabFiles)
//			_Palette.Add(AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject)) as GameObject);
//	}
	


//	private void OnGUI()
//	{
//		BeginWindows();
//		_ScrollPosition = EditorGUILayout.BeginScrollView(_ScrollPosition);


//		//�v���n�u�����o���ăe�N�X�`���擾
//		List<GUIContent> paletteIcons = new List<GUIContent>();
//		foreach (GameObject prefab in _Palette)
//		{
//			Texture2D texture = AssetPreview.GetAssetPreview(prefab);
//			paletteIcons.Add(new GUIContent(texture));
//			_ChoicePrefabTex = texture;
//		}

//		GUILayout.Space(420);

//		_PaletteIndex = GUILayout.SelectionGrid(_PaletteIndex, paletteIcons.ToArray(), 2);

//		if(_PaintMode)
//        {
//			//None
//		}

		

//		//���݂̃T�C�Y�\��
//		EditorGUILayout.LabelField($"���݂̃T�C�Y : {position.size}");

		
//		_Center.x = position.size.x * 0.5f - (_GridSize * _MapSize * 0.5f);
		

//		// �O���b�h�f�[�^�𐶐�
//		_GridRect = CreateGrid(_MapSize);

//		// �O���b�h����`�悷��
//		for (int yy = 0; yy < _MapSize; yy++)
//		{
//			for (int xx = 0; xx < _MapSize; xx++)
//			{
//				DrawGridLine(_GridRect[yy, xx]);

//				_BlockPos[yy, xx] = _GridRect[yy, xx];
//			}
//		}

//		// �N���b�N���ꂽ�ʒu��T���āA���̏ꏊ�ɉ摜�f�[�^������
//		Event e = Event.current;
//		if (e.type == EventType.MouseDown)
//		{
//			_ClickPos =Event.current.mousePosition;

            
//			for (int xx = 0;xx < _MapSize; xx++)
//            {
//				for (int yy = 0; yy < _MapSize; yy++)
//				{
//					if (_GridRect[yy, xx].Contains(_ClickPos))
//					{
						
//						_BlockTex[yy, xx] = _ChoicePrefabTex;

//						//�͈͓��Ȃ�E�N���b�N�\�ɂ���
//						_BlockChoice = true;

//						Repaint();
//						break;
//					}
//				}
//			}
           
//        }

//		for (int i = 0; i < _MapSize; i++)
//		{
//			for (int j = 0; j < _MapSize; j++)
//			{
//				if(_BlockTex[i,j] != null)
//                {
					
//					EditorGUI.DrawPreviewTexture(_BlockPos[i,j],_BlockTex[i,j]);
//				}
//			}
//		}

//		EditorGUILayout.EndScrollView();

//		EndWindows();


//		var ev = Event.current;
//		if (ev.type == EventType.ContextClick)
//		{
//			if(_BlockChoice)
//            {
//				var menu = new GenericMenu();
//				menu.AddItem(new GUIContent("�u���b�N�̒ǉ�"), false, AddBlock);
//				menu.AddItem(new GUIContent("�u���b�N�̍폜"), false, DeleteBlock);
//				menu.ShowAsContext();
//				ev.Use();
//			}
			
//		}

//	}



//	void AddBlock()
//	{
//		EditorWindow.GetWindow<BlockCreateWindow>("CreateBlock").Init();
//	}

//	void DeleteBlock()
//	{
		
//		for (int xx = 0; xx < _MapSize; xx++)
//		{
//			for (int yy = 0; yy < _MapSize; yy++)
//			{
//				if (_GridRect[yy, xx].Contains(_ClickPos))
//				{
//					_BlockTex[yy, xx] = null;
//					Repaint();
//					break;
//				}
//			}
//		}
//	}


//	// �O���b�h�f�[�^�𐶐�
//	private Rect[,] CreateGrid(int div)
//	{
//		int sizeW = div;
//		int sizeH = div;

//		float x = _Center.x;
//		float y = _Center.y;
//		float w = _GridSize;
//		float h = _GridSize;

//		Rect[,] resultRects = new Rect[sizeH, sizeW];

//		for (int yy = 0; yy < sizeH; yy++)
//		{
//			x = _Center.x;
//			for (int xx = 0; xx < sizeW; xx++)
//			{
//				Rect r = new Rect(new Vector2(x, y), new Vector2(w, h));
//				resultRects[yy, xx] = r;
//				x += w;
//			}
//			y += h;
//		}

//		return resultRects;
//	}

//	// �O���b�h����`��
//	private void DrawGridLine(Rect r)
//	{
//		// upper line
//		Handles.DrawLine(
//			new Vector2(r.position.x, r.position.y),
//			new Vector2(r.position.x + r.size.x,r.position.y));

//		// bottom line
//		Handles.DrawLine(
//			new Vector2(r.position.x,  r.position.y + r.size.y),
//			new Vector2(r.position.x + r.size.x,  r.position.y + r.size.y));

//		// left line
//		Handles.DrawLine(
//			new Vector2(r.position.x, r.position.y),
//			new Vector2(r.position.x, r.position.y + r.size.y));

//		// right line
//		Handles.DrawLine(
//			new Vector2(r.position.x + r.size.x, r.position.y),
//			new Vector2(r.position.x + r.size.x, r.position.y + r.size.y));
//	}

//}

//public class FaceWindow : EditorWindow
//{
//	//�X�N���[���ʒu
//	private Vector2 _ScrollPosition = Vector2.zero;

	
//	int _SelectFace = 0;


//	int _curView;


//	void OnGUI()
//	{
//		_ScrollPosition = EditorGUILayout.BeginScrollView(_ScrollPosition);

		

//		EditorGUILayout.BeginVertical(GUI.skin.box);
//		{
//			string[] selectFacName = new string[] { "", "Face04", "", "Face02", "Face01", "Face03", "", "Face05", "", "", "Face06", "" };
//			string[] selectFacePos = new string[] { "", "", "", "", "", "", "", "", "", "", "", "" };

//			_SelectFace = GUILayout.Toolbar(GUILayout.SelectionGrid(_SelectFace, selectFacName, 3, GUILayout.Width(300), GUILayout.Height(400)), selectFacePos, GUILayout.Height(10));
//		}
//		EditorGUILayout.EndVertical();


//		//�X�N���[���ӏ��I��
//		EditorGUILayout.EndScrollView();

//		using (new EditorGUILayout.HorizontalScope())
//		{
//			var sceneView = SceneView.lastActiveSceneView;

//			if (_curView == _SelectFace)
//				return;

//			switch (_SelectFace)
//            {
//				case 0:
//				case 2:
//				case 6:
//				case 8:
//				case 9:
//				case 11:
//					break;
//				case 1:
//					sceneView.pivot = new Vector3(0, 8, 0);
//					_curView = _SelectFace;
//					break;
//				case 3:
//					sceneView.pivot = new Vector3(-8, 0, 0);
//					_curView = _SelectFace;
//					break;
//				case 4:
//					sceneView.pivot = new Vector3(0, 0, 0);
//					_curView = _SelectFace;
//					break;
//				case 5:
//					sceneView.pivot = new Vector3(8, 0, 0);
//					_curView = _SelectFace;
//					break;
//				case 7:
//					sceneView.pivot = new Vector3(0, -8, 0);
//					_curView = _SelectFace;
//					break;
//				case 10:
//					sceneView.pivot = new Vector3(0, -16, 0);
//					_curView = _SelectFace;
//					break;
//				default:
//					break;
//			}

//			SceneView.RepaintAll();
//		}
//	}
//}

//public class BlockCreateWindow : EditorWindow
//{
//	private GameObject block;

//	public void Init()
//    {
//		var window = this;
//        window.minSize = new Vector2(400, 300);
//	}

//    void OnGUI()
//    {
//		GUILayout.BeginHorizontal();
//		GUILayout.Label("Block", GUILayout.Width(110));
//		block = (GameObject)EditorGUILayout.ObjectField(block, typeof(GameObject), true);
//		GUILayout.EndHorizontal();
//		EditorGUILayout.Space();


//		//�����{�^��
//		// GUI�̎g�p��.
//		if (GUI.Button(new Rect(0.0f, 200.0f, 120.0f, 20.0f), "�u���b�N�𐶐�����"))
//		{
//			Debug.Log("Button Pressed.");
//			this.Close();
//		}
//	}
//}

///////////////////////////////////////////////////////////////////
/////



//public static class DragAndDropUtility
//{
//	/// <summary>
//	/// D&D���J�n����
//	/// </summary>
//	public static void Begin(Object[] objs, object data)
//	{
//		DragAndDrop.PrepareStartDrag();
//		DragAndDrop.paths = null;
//		DragAndDrop.objectReferences = objs;
//		DragAndDrop.SetGenericData("data", data);
//		DragAndDrop.StartDrag(objs.Length == 1 ? objs[0].name : "<Multiple>");
//	}

//	/// <summary>
//	/// D&D���󂯕t����
//	/// </summary>
//	/// <param name="window"></param>
//	/// <param name="rect"></param>
//	/// <returns></returns>
//	public static bool Check(EditorWindow window, Rect? rect = null)
//	{
//		var ev = Event.current;
//		// �G���A���w�肳��Ă���Δ͈͓����m�F
//		if (rect.HasValue && !rect.Value.Contains(ev.mousePosition))
//			return false;

//		if (ev.type != EventType.DragUpdated && ev.type != EventType.DragPerform)
//			return false;

//		var paths = DragAndDrop.paths;
//		if (EditorWindow.mouseOverWindow != window || paths.Length <= 0)
//			return false;

//		DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
//		if (ev.type == EventType.DragPerform)
//		{
//			DragAndDrop.activeControlID = 0;
//			DragAndDrop.AcceptDrag();
//			return true;
//		}

//		DragAndDrop.activeControlID = GUIUtility.GetControlID(FocusType.Passive);
//		Event.current.Use();

//		return false;
//	}
//}


//public class SamEditorWindow : EditorWindow
//{
//	private void OnGUI()
//	{
//		if (DragAndDropUtility.Check(this))
//		{
//			foreach (var path in DragAndDrop.paths)
//			{
//				Debug.Log("�u" + path + "�v�̃I�u�W�F�N�gD&D���ꂽ��");
//			}
//		}
//	}


//	[MenuItem("Editor/SamEditorWindow")]
//	private static void Create()
//	{
//		// ����
//		EditorWindow.GetWindow<SamEditorWindow>("SamEditorWindow");
//	}
//}