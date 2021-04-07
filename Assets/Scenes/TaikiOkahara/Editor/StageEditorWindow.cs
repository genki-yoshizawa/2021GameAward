using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class StageEditorWindow : Editor
{

	




	[MenuItem("Editor/StageEditorWindow")]
	private static void Create()
	{
		// 生成
		EditorWindow.GetWindow<BlockWindow>("ぶろっくえでた").init();
		EditorWindow.GetWindow<FaceWindow>("ふぇいすえでた", typeof(BlockWindow));
		
	}
}

public class BlockWindow : EditorWindow
{

	// マップエディタのマスの数
	private int mapSize = 3;
	// グリッドの大きさ、小さいほど細かくなる
	private float gridSize = 100.0f;
	// グリッドの四角
	private Rect[,] gridRect;

	//class BlockList
 //   {
	//	public Texture2D tex;
	//	public Rect r;
 //   }

	// マップデータ
	private Texture2D[,] _blockTex = new Texture2D[3,3];
	private Rect[,] _blockPos = new Rect[3,3];

	private Vector2 center;

	private bool blockChoice = false;

	private Object _block;

	private Vector2 _clickPos;
	//private Editor _editor;
	//private ScriptableObject _target;

	//スクロール位置
	private Vector2 _scrollPosition = Vector2.zero;

	[SerializeField]
	private List<GameObject> palette = new List<GameObject>();

	private string path = "Assets/Scenes/TaikiOkahara/StageBlock";

    private void OnFocus()
    {
		RefreshPalette();
    }
    public void init()
	{

		center.x = position.size.x * 0.5f - (gridSize * mapSize * 0.5f);
		center.y = 50.0f;

		//_blockTex = new Texture2D[mapSize, mapSize];
		//_blockPos = new Rect[mapSize, mapSize];

		// マップデータを初期化

		for (int i = 0; i < mapSize; i++)
		{
			for (int j = 0; j < mapSize; j++)
			{
				_blockTex[i, j] = null;
				//map[i, j].r = new Rect();
			}
		}
		
		
		

		Debug.Log("通った");
	}



	// A list containing the available prefabs.
	

	private void RefreshPalette()
	{
		palette.Clear();

		string[] prefabFiles = System.IO.Directory.GetFiles(path, "*.prefab");
		foreach (string prefabFile in prefabFiles)
			palette.Add(AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject)) as GameObject);
	}

	[SerializeField]
	private int paletteIndex;

	// Called to draw the MapEditor windows.

	bool paintMode;

	private Texture2D _choicePrefab;

	private void OnGUI()
	{


		BeginWindows();

		//描画範囲が足りなければスクロール出来るように
		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

		//paintMode = GUILayout.Toggle(paintMode, "Start painting", "Button", GUILayout.Height(420f));

		// Get a list of previews, one for each of our prefabs
		List<GUIContent> paletteIcons = new List<GUIContent>();
		foreach (GameObject prefab in palette)
		{
			// Get a preview for the prefab
			Texture2D texture = AssetPreview.GetAssetPreview(prefab);
			paletteIcons.Add(new GUIContent(texture));
			_choicePrefab = texture;
		}

		GUILayout.Space(420);

		// Display the grid
		paletteIndex = GUILayout.SelectionGrid(paletteIndex, paletteIcons.ToArray(), 2);

		if(paintMode)
        {
			//Vector2 cellCenter = GetSelectedCell(); // Refactoring, I moved some code in this function


			//DisplayVisualHelp(cellCenter);
			//HandleSceneViewInputs(cellCenter);

			// Refresh the view
			//sceneView.Repaint();
		}

		

		//現在のサイズ表示
		EditorGUILayout.LabelField($"現在のサイズ : {position.size}");


		center.x = position.size.x * 0.5f - (gridSize * mapSize * 0.5f);
		//face01 = (GameObject)EditorGUILayout.ObjectField("Face01",face01,typeof(GameObject))

		//EditorGUI.BeginChangeCheck();
		//_target = (ScriptableObject)EditorGUILayout.ObjectField("ScriptableObject", _target, typeof(ScriptableObject), true);
		//if (EditorGUI.EndChangeCheck())
		//{
		//	_editor = Editor.CreateEditor(_target);
		//}

		//if (_editor == null)
		//	return;

		//_editor.OnInspectorGUI();


		// グリッドデータを生成
		gridRect = CreateGrid(mapSize);

		// グリッド線を描画する
		for (int yy = 0; yy < mapSize; yy++)
		{
			for (int xx = 0; xx < mapSize; xx++)
			{
				DrawGridLine(gridRect[yy, xx]);

				//map = new BlockList[mapSize, mapSize];

				_blockPos[yy, xx] = gridRect[yy, xx];
			}
		}

		// クリックされた位置を探して、その場所に画像データを入れる
		Event e = Event.current;
		if (e.type == EventType.MouseDown)
		{
			_clickPos =Event.current.mousePosition;
			//int xx;
			//// x位置を先に計算して、計算回数を減らす
			//for (xx = 0; xx < mapSize; xx++)
			//{
			//	Rect r = gridRect[0, xx];
			//	if (r.position.x + center.x <= pos.x && pos.x <= r.position.x + center.x + r.width)
			//	{
			//		break;
			//	}
			//}

            // 後はy位置だけ探す
			for (int xx = 0;xx < mapSize; xx++)
            {
				for (int yy = 0; yy < mapSize; yy++)
				{
					if (gridRect[yy, xx].Contains(_clickPos))
					{
						// 消しゴムの時はデータを消す
						//if (parent.SelectedImagePath.IndexOf("000") > -1)
						//{
						//	map[yy, xx] = "";
						//}
						//else
						{
							//_blockTex[yy, xx] = _choicePrefab;
							//Debug.Log("map[" + yy + "," + xx + "]の範囲内");
							
						}

						_blockTex[yy, xx] = _choicePrefab;

						//範囲内なら右クリック可能にする
						blockChoice = true;

						Repaint();
						break;
					}
				}
			}
           
        }

		for (int i = 0; i < mapSize; i++)
		{
			for (int j = 0; j < mapSize; j++)
			{
				if(_blockTex[i,j] != null)
                {
					
					EditorGUI.DrawPreviewTexture(_blockPos[i,j],_blockTex[i,j]);
				}
			}
		}

		EditorGUILayout.EndScrollView();

		EndWindows();


		var ev = Event.current;
		if (ev.type == EventType.ContextClick)
		{
			if(blockChoice)
            {
				// MyEditorWindowの背景でマウスを右クリックするとここに来る。 
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("ブロックの追加"), false, AddBlock);
				menu.AddItem(new GUIContent("ブロックの削除"), false, DeleteBlock);
				menu.ShowAsContext();
				ev.Use();
			}
			
		}

		// 選択した画像を描画する
		//for (int yy = 0; yy < mapSize; yy++)
		//{
		//	for (int xx = 0; xx < mapSize; xx++)
		//	{
		//		if (map[yy, xx] != null && map[yy, xx].Length > 0)
		//		{
		//			Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(map[yy, xx], typeof(Texture2D));
		//			GUI.DrawTexture(gridRect[yy, xx], tex);
		//		}
		//	}
		//}
	}



	void AddBlock()
	{
		//// GUI
		//GUILayout.BeginHorizontal();
		//GUILayout.Label("Block : ", GUILayout.Width(110));
		//_block = EditorGUILayout.ObjectField(_block, typeof(UnityEngine.Object), true);
		//GUILayout.EndHorizontal();
		//EditorGUILayout.Space();

		//新しいゲームオブジェクトを作成
		//new GameObject("New GameObject");

		EditorWindow.GetWindow<BlockCreateWindow>("CreateBlock").init();



	}

	void DeleteBlock()
	{
		// 後はy位置だけ探す
		for (int xx = 0; xx < mapSize; xx++)
		{
			for (int yy = 0; yy < mapSize; yy++)
			{
				if (gridRect[yy, xx].Contains(_clickPos))
				{
					
					_blockTex[yy, xx] = null;

					Repaint();
					break;
				}
			}
		}
	}


	//private void OnSceneGUI()
	//{
	//	_postOnSceneGUI?.Invoke();
	//	_postOnSceneGUI = null;
	//}


	//void OnGUI()
	//{


	//}



	// グリッドデータを生成
	private Rect[,] CreateGrid(int div)
	{
		int sizeW = div;
		int sizeH = div;

		float x = center.x;
		float y = center.y;
		float w = gridSize;
		float h = gridSize;

		Rect[,] resultRects = new Rect[sizeH, sizeW];

		for (int yy = 0; yy < sizeH; yy++)
		{
			x = center.x;
			for (int xx = 0; xx < sizeW; xx++)
			{
				Rect r = new Rect(new Vector2(x, y), new Vector2(w, h));
				resultRects[yy, xx] = r;
				x += w;
			}
			y += h;
		}

		return resultRects;
	}

	// グリッド線を描画
	private void DrawGridLine(Rect r)
	{
		

		// upper line
		Handles.DrawLine(
			new Vector2(r.position.x, r.position.y),
			new Vector2(r.position.x + r.size.x,r.position.y));

		// bottom line
		Handles.DrawLine(
			new Vector2(r.position.x,  r.position.y + r.size.y),
			new Vector2(r.position.x + r.size.x,  r.position.y + r.size.y));

		// left line
		Handles.DrawLine(
			new Vector2(r.position.x, r.position.y),
			new Vector2(r.position.x, r.position.y + r.size.y));

		// right line
		Handles.DrawLine(
			new Vector2(r.position.x + r.size.x, r.position.y),
			new Vector2(r.position.x + r.size.x, r.position.y + r.size.y));
	}

}

public class FaceWindow : EditorWindow
{
	//スクロール位置
	private Vector2 _scrollPosition = Vector2.zero;

	//public static CreaterBlockWindow WillAppear(MapCreater _parent)
	//{
	//	MapCreaterSubWindow window = (MapCreaterSubWindow)EditorWindow.GetWindow(typeof(MapCreaterSubWindow), false);
	//	window.Show();
	//	window.minSize = new Vector2(WINDOW_W, WINDOW_H);
	//	window.SetParent(_parent);
	//	window.init();
	//	return window;
	//}
	//window.SetParent(_parent);

	Vector2 buttonSize = new Vector2(100, 100);

	Vector2 buttonMinSize = new Vector2(100, 20);
	Vector2 buttonMaxSize = new Vector2(1000, 200);

	bool expandWidth = true;
	bool expandHeight = true;

	//bool _testToggle01 = false;
	//bool _testToggle02 = false;
	//bool _testToggle03 = false;

	//GUISkin _skin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/Scenes/Editor/EditorSkin", typeof(GUISkin));
	int objectSelectionToolbar = 0;


	int _curView;


	void OnGUI()
	{

		


		//描画範囲が足りなければスクロール出来るように
		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

		
		

		EditorGUILayout.BeginVertical(GUI.skin.box);
		{


			string[] stationSelectionToolbarOptions = new string[] { "", "Face04", "", "Face02", "Face01", "Face03", "", "Face05", "", "", "Face06", "" };
			string[] stationSelectionToolbarOptions2 = new string[] { "", "", "", "", "", "", "", "", "", "", "", "" };

			objectSelectionToolbar = GUILayout.Toolbar(GUILayout.SelectionGrid(objectSelectionToolbar, stationSelectionToolbarOptions, 3, GUILayout.Width(300), GUILayout.Height(400)), stationSelectionToolbarOptions2, GUILayout.Height(10));




			//GUI.SelectionGrid(Rect.zero, objectSelectionToolbar, stationSelectionToolbarOptions, 2);

			//GUILayout.Space(50);

		}
		EditorGUILayout.EndVertical();


		//// 自動的にサイズ変更される範囲を指定する場合は
		//// GUILayout.MinWidth/MaxWidth/MinHeight/MaxHeightを使う。
		//buttonMinSize = EditorGUILayout.Vector2Field("ButtonMinSize", buttonMinSize);
		//buttonMaxSize = EditorGUILayout.Vector2Field("ButtonMaxSize", buttonMaxSize);


		//if (GUILayout.Button("最小最大指定ボタン",
		//					  GUILayout.MinWidth(buttonMinSize.x), GUILayout.MinHeight(buttonMinSize.y),
		//					  GUILayout.MaxWidth(buttonMaxSize.x), GUILayout.MaxHeight(buttonMaxSize.y)))
		//{
		//	Debug.Log("最小最大指定ボタン");
		//}

		//// 有効範囲内全体に広げるかどうかは
		//// GUILayout.ExpandWidth/ExpandHeightで指定する。
		//expandWidth = EditorGUILayout.Toggle("ExpandWidth", expandWidth);
		//expandHeight = EditorGUILayout.Toggle("ExpandHeight", expandHeight);
		//if (GUILayout.Button("Expandボタン", GUILayout.ExpandWidth(expandWidth), GUILayout.ExpandHeight(expandHeight)))
		//{
		//	Debug.Log("Expandボタン");
		//}

		



		//スクロール箇所終了
		EditorGUILayout.EndScrollView();


		using (new EditorGUILayout.HorizontalScope())
		{
			var sceneView = SceneView.lastActiveSceneView;

			if (_curView == objectSelectionToolbar)
				return;

			switch (objectSelectionToolbar)
            {
				case 0:
				case 2:
				case 6:
				case 8:
				case 9:
				case 11:
					break;
				case 1:
					sceneView.pivot = new Vector3(0, 8, 0);
					_curView = objectSelectionToolbar;
					break;
				case 3:
					sceneView.pivot = new Vector3(-8, 0, 0);
					_curView = objectSelectionToolbar;

					break;
				case 4:
					sceneView.pivot = new Vector3(0, 0, 0);
					_curView = objectSelectionToolbar;

					break;
				case 5:
					sceneView.pivot = new Vector3(8, 0, 0);
					_curView = objectSelectionToolbar;

					break;
				case 7:
					sceneView.pivot = new Vector3(0, -8, 0);
					_curView = objectSelectionToolbar;

					break;
				case 10:
					sceneView.pivot = new Vector3(0, -16, 0);
					_curView = objectSelectionToolbar;

					break;

				default:
					break;
			}

			SceneView.RepaintAll();

		}

	}




    

}

public class BlockCreateWindow : EditorWindow
{
	private GameObject block;

	public void init()
    {
		var window = this;
        //ウィンドウサイズ設定(minとmaxを=しているのはウィンドウサイズを固定するため)
        window.minSize = new Vector2(400, 300);

		
	}

    void OnGUI()
    {
		// GUI
		GUILayout.BeginHorizontal();
		GUILayout.Label("Block", GUILayout.Width(110));
		block = (GameObject)EditorGUILayout.ObjectField(block, typeof(GameObject), true);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();


		//生成ボタン
		// GUIの使用例.
		if (GUI.Button(new Rect(0.0f, 200.0f, 120.0f, 20.0f), "ブロックを生成する"))
		{
			Debug.Log("Button Pressed.");
			this.Close();
		}
	}
}