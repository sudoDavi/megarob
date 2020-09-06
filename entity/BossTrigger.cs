using Godot;
using System;

public class BossTrigger : Area2D
{
	[Export] public int BossNumber = 0;
	public Position2D levelStart;
	private Sprite run;
	private PackedScene gun = GD.Load<PackedScene>("res://player/gun_pickup.tscn");
	private Position2D gunP;
	public override void _Ready()
	{
		levelStart = GetNode<Position2D>($"/root/Main/Hub/Start{BossNumber}");
		this.Connect("body_entered", this, nameof(_on_BodyEnter));
		if (BossNumber == 1)
		{
			gunP = GetNode<Position2D>("GunPosition");
			run = GetNode<Sprite>("/root/Main/Level1/Hints/run");
		}
	}

	public void _on_BodyEnter(Node2D body)
	{
		if (body.IsInGroup("Player")
			&& body.GlobalPosition != levelStart.GlobalPosition
		)
		{
			GetNode($"/root/Main/Level{BossNumber}/Enemies/Boss{BossNumber}")
				.Set("HasAwakened", true);
			body.Call("IsFighting");
			if (BossNumber == 1)
			{
				run.Visible = true;
				var gunNode = (Node2D)gun.Instance();
				gunNode.GlobalPosition = gunP.GlobalPosition;
				GetParent().CallDeferred("add_child", gunNode);
			}

			this.Disconnect("body_entered", this, nameof(_on_BodyEnter));
		}
	}
}
