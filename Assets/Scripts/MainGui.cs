using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UImGui;
using ImGuiNET;
using ImGuizmoNET;
using System.IO;
using PM.Models;

public class MainGui : MonoBehaviour
{
	public static MainGui Instance { get; private set; }

	public Transform rootTransform;
	public Camera sceneCamera;

	public PMModel model;
	public List<ImWindow> windows;
	public List<ImWindow> staticWindows;

	void Awake()
	{
		Instance = this;
		windows = new();
		staticWindows = new();

		staticWindows.Add(new ConsoleWindow(this));
		ClearWindows();

		UImGuiUtility.Layout += OnLayout;
	}

	void ClearWindows()
    {
		windows.Clear();
		windows.AddRange(staticWindows);
	}

	void OpenDialog()
	{
		using (Ookii.Dialogs.VistaOpenFileDialog dialog = new())
		{
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				SetModel(PMModel.Load(dialog.FileName));
		}
	}

	void CloseModel()
	{
		if (model)
		{
			ClearWindows();
			Destroy(model.gameObject);
			model = null;
		}
	}

	void SetModel(PMModel m)
    {
		if (model)
			CloseModel();

		model = m;

		new AnimationWindow(this);
		if (model.HasTextures)
			new TextureWindow(this);

		Debug.Log("Set model to " + model.Model.Header.modelName);
	}

	void DrawGizmos()
	{
		Matrix4x4 view = sceneCamera.worldToCameraMatrix;
		Matrix4x4 proj = sceneCamera.projectionMatrix;
		Matrix4x4 m = Matrix4x4.identity;

		//ImGuizmo.DrawGrid(ref view.m00, ref proj.m00, ref m.m00, 10f);

		if (model)
		{
			m = model.transform.localToWorldMatrix;
			if (ImGuizmo.Manipulate(ref view.m00, ref proj.m00, OPERATION.TRANSLATE, MODE.LOCAL, ref m.m00))
				model.transform.position = m.GetColumn(3);
			if (ImGuizmo.Manipulate(ref view.m00, ref proj.m00, OPERATION.ROTATE, MODE.LOCAL, ref m.m00))
				model.transform.rotation = m.rotation;
		}
	}

	void OnLayout(UImGui.UImGui obj)
	{
		ImGuizmo.SetRect(0f, 0f, Screen.width, Screen.height);
		DrawGizmos();

		ImGui.BeginMainMenuBar();
		if (ImGui.BeginMenu("File"))
		{
			if (ImGui.MenuItem("Open"))
				OpenDialog();

			if (ImGui.MenuItem("Close"))
				CloseModel();

			if (ImGui.MenuItem("Exit"))
			{
				Debug.Log("Exit");
				Application.Quit();
			}
			ImGui.EndMenu();
			ImGui.EndMainMenuBar();
		}

		ImGui.SetNextWindowPos(ImGui.GetWindowContentRegionMin() + new Vector2(5f, 5f), ImGuiCond.Once);
		if (ImGui.Begin("Scene Graph", ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings))
		{
			GraphTree(rootTransform);
			ImGui.End();
		}

		foreach (ImWindow win in windows)
			win.OnLayout();
	}

	void GraphTree(Transform parent)
    {
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			string label = child.gameObject.name;

			if (child.childCount == 0)
			{
				ImGui.Indent();
				ImGui.Text(label);
				ImGui.Unindent();
			}
			else if (ImGui.TreeNode(label))
			{
				GraphTree(child);
				ImGui.TreePop();
			}
		}
	}

	void OnDisable()
	{
		UImGuiUtility.Layout -= OnLayout;
	}
}

public class TextureWindow : ImWindow
{
	public int TextureIndex;

	public TextureWindow(MainGui m) : base(m)
    {
		Title = "Textures";
		Flags = ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize;
	}

    public override void OnContent()
    {
		ImGui.Text($"Textures from '{Model.Model.Header.textureName}-'");

		if (ImGui.ArrowButton("texview_left", ImGuiDir.Left)) TextureIndex--;
		ImGui.SameLine();
		if (ImGui.ArrowButton("texview_right", ImGuiDir.Right)) TextureIndex++;
		ImGui.SameLine();

		TextureIndex = Mathf.Clamp(TextureIndex, 0, Model.Textures.Length - 1);
		ImGui.Text($"Texture {TextureIndex}/{Model.Textures.Length - 1}");

		ImGui.Text(Model.Model.Textures[TextureIndex].unk_c);

		Texture2D tex = Model.Textures[TextureIndex];
		ImGui.Image(UImGuiUtility.GetTextureId(tex), new Vector2(tex.width, tex.height));
		ImGui.End();
	}
}