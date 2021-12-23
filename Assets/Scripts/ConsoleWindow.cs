using UnityEngine;
using ImGuiNET;
using System.Collections.Generic;

public class ConsoleWindow : ImWindow
{
    public Dictionary<LogEntry, int> Entries;

	public ConsoleWindow(MainGui m) : base(m)
	{
		Title = "Console";
		Flags = ImGuiWindowFlags.NoDocking;

        Entries = new Dictionary<LogEntry, int>();
        Application.logMessageReceived += Application_logMessageReceived;
	}

    void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        LogEntry e = new(condition, stackTrace, type);
        if (Entries.TryGetValue(e, out int count))
            Entries[e] = count + 1;
        else
            Entries.Add(e, 1);
    }

    public override void OnLayout()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(200, 100));
        base.OnLayout();
        ImGui.PopStyleVar();
    }

    public override void OnContent()
    {
        if (ImGui.Button("Clear"))
            Entries.Clear();
        ImGui.SameLine();
        if (ImGui.Button("Restart"))
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

        if (ImGui.BeginChild("Entries"))
        {
            foreach (var pair in Entries)
            {
                LogEntry e = pair.Key;
                int count = pair.Value;
                if (count > 1)
                {
                    ImGui.Text($"({count})");
                    ImGui.SameLine();
                }
                ImGui.Text(e.type + ":");
                ImGui.SameLine();
                ImGui.TextColored(Color.gray, e.condition);
            }
            ImGui.EndChild();
        }
    }

    public readonly struct LogEntry
    {
        public readonly string condition;
        public readonly string stackTrace;
        public readonly LogType type;

        public LogEntry(string condition, string stackTrace, LogType type)
        {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }
}