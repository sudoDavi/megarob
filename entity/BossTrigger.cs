using Godot;
using System;

public class BossTrigger : Area2D
{
	[Export] public int BossNumber = 0;
	public Position2D levelStart;
	public override void _Ready()
	{
		levelStart = GetNode<Position2D>($"/root/Main/Hub/Start{BossNumber}");
		this.Connect("body_entered", this, nameof(_on_BodyEnter));
	}

	public void _on_BodyEnter(Node2D body)
	{
		if (body.IsInGroup("Player")
			&& body.GlobalPosition != levelStart.GlobalPosition
		)
		{
			GetNode($"/root/Main/Level{BossNumber}/Enemies/Boss{BossNumber}")
				.Set("HasAwakened", true);
			this.Disconnect("body_entered", this, nameof(_on_BodyEnter));
		}
	}
}
