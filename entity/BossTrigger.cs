using Godot;
using System;

public class BossTrigger : Area2D
{
	[Export] public int BossNumber = 0;
	public override void _Ready()
	{
		this.Connect("body_entered", this, nameof(_on_BodyEnter));
	}

	public void _on_BodyEnter(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			GetNode($"/root/Main/Level{BossNumber}/Enemies/Boss{BossNumber}")
				.Set("HasAwakened", true);
		}
	}
}
