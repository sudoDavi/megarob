using Godot;
using System;

public class LevelDetector : Area2D
{
	[Export] public int LevelNumber = 0;
	public override void _Ready()
	{
		this.Connect("body_entered", this, nameof(_on_BodyEnter));
	}

	public void _on_BodyEnter(Node body)
	{
		if (body.IsInGroup("Player"))
			body.Set("IsInLevel", LevelNumber);
	}

}
