using UnityEngine;
using ImGuiNET;
using System.Linq;

public class AnimationWindow : ImWindow
{
	public int ComboAnimIndex;
	readonly string[] AnimItems;

	public AnimationWindow(MainGui m) : base(m)
	{
		Title = "Animation";
		Flags = ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize;
		AnimItems = Model.Animations.Select(a => a.Name).Prepend("[None]").ToArray();
	}

	public override void OnContent()
	{
		int prevIndex = ComboAnimIndex;

		ImGui.Combo("Animation", ref ComboAnimIndex, AnimItems, AnimItems.Length);

		if (ImGui.ArrowButton("anim_prev", ImGuiDir.Left)) ComboAnimIndex--;
		ImGui.SameLine();
		if (ImGui.ArrowButton("anim_next", ImGuiDir.Right)) ComboAnimIndex++;
		ComboAnimIndex = Mathf.Clamp(ComboAnimIndex, 0, AnimItems.Length - 1);

		if (Model.Animator.Animation != null)
		{
			if (!Model.Animator.IsPlaying)
			{
				if (ImGui.Button("Play")) Model.Animator.Play();
			}
			else
			{
				if (ImGui.Button("Stop")) Model.Animator.Stop();
			}
		}

		if (prevIndex != ComboAnimIndex)
			AnimChange();
	}

	void AnimChange()
	{
		Model.Animator.Stop();

		int animIndex = ComboAnimIndex - 1;
		if (animIndex < 0)
		{
			Model.ResetVirtualModel();
			return;
		}

		Model.Animator.SetAnimation(Model.Animations[animIndex]);
	}
}