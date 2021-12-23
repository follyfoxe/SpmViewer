using ImGuiNET;
using PM.Models;

public abstract class ImWindow
{
	public PMModel Model => Main.model;
	public MainGui Main { get; }

	public string Title;
	public ImGuiWindowFlags Flags;

	public ImWindow(MainGui mainGui)
    {
		Main = mainGui;
		mainGui.windows.Add(this);
    }

	public abstract void OnContent();

	public virtual void OnLayout()
    {
		if (ImGui.Begin(Title, Flags))
        {
			OnContent();
			ImGui.End();
		}
	}

	public void Close()
    {
		Main.windows.Remove(this);
	}
}