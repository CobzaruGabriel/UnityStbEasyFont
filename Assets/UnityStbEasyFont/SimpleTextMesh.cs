﻿using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SimpleTextMesh : MonoBehaviour {
	[Multiline]
	public string text = "ABC";
	public Color32 color = new Color32(255,255,255,255);
	public float characterSize = 1.0f;

	private string prevText = null;
	private Color32 prevColor = new Color32(0,0,0,0);
	private Mesh mesh;
	private Material mat;

	const float kScaleFactor = 0.12f;

	void Start () {
		UpdateMesh();
	}

	void OnDisable()
	{
		DestroyImmediate(mesh);
		DestroyImmediate(mat);
	}
	
	void Update () {
		UpdateMesh(); 

		if (mesh != null)
		{
			UpdateMaterial();
			var mtx = transform.localToWorldMatrix;
			var scale = kScaleFactor * characterSize;
			var scaleMat = Matrix4x4.Scale(new Vector3(scale,-scale,scale));
			Graphics.DrawMesh(mesh, mtx * scaleMat, mat, 0);
		}
	}

	void UpdateMaterial()
	{
		if (mat != null)
			return;
		var shader = Shader.Find ("Hidden/Internal-Colored");
		mat = new Material (shader);
		mat.hideFlags = HideFlags.HideAndDontSave;
		// Turn on alpha blending
		mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		// Turn backface culling off
		mat.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
		// Turn off depth writes
		mat.SetInt ("_ZWrite", 0);
	}

	void UpdateMesh()
	{
		if (text == prevText && color.Equals(prevColor) && mesh != null)
			return;
		prevText = text;
		prevColor = color;

		if (mesh != null)
			mesh.Clear();
		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
		}
		List<Vector3> vertices = new List<Vector3>();
		List<Color32> colors = new List<Color32>();
		StbEasyFont.stb_easy_font_print(0, 0, text, color, vertices, colors);
		mesh.vertices = vertices.ToArray();
		mesh.colors32 = colors.ToArray();
		mesh.subMeshCount = 1;
		var indices = new int[vertices.Count];
		for (var i = 0; i < indices.Length; ++i)
			indices[i] = i;
		mesh.SetIndices(indices, MeshTopology.Quads, 0);
	}

	#if UNITY_EDITOR
	[UnityEditor.MenuItem("GameObject/3D Object/Simple 3D Text")]
	public static void CreateEasy3DText()
	{
		var go = new GameObject("New 3D Text", typeof(SimpleTextMesh));
		go.GetComponent<SimpleTextMesh>().text = "Hello World";

		var view = UnityEditor.SceneView.lastActiveSceneView;
		if (view != null)
			view.MoveToView(go.transform);
		UnityEditor.Selection.activeGameObject = go;
	}
	#endif
}