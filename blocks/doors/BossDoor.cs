using Godot;
using System;

public class BossDoor : StaticBody2D
{
	[Export] public int IsInLevel = 0;
	[Export] public bool HasTrigger = false;
	public Area2D Trigger;
	public override void _Ready()
	{
		if (HasTrigger)
		{
			Trigger = GetNode<Area2D>("Trigger");
			Trigger.Connect("body_entered", this, nameof(_on_BodyEnter));

			var doors = GetTree().GetNodesInGroup("L" + IsInLevel + "BossDoor");
			OpenDoor(doors);

			if ((int)GetNode("/root/Main/Player").Get("KilledBoss") >= IsInLevel)
				CloseDoor(doors);
		}
	}

	public void _on_BodyEnter(Node2D body)
	{
		if (body.IsInGroup("Player") && (int)body.Get("IsInLevel") == IsInLevel)
		{
			GD.Print("Triggered door");
			CloseDoor(GetTree().GetNodesInGroup("L" + IsInLevel + "BossDoor"));
			Trigger.SetDeferred("monitoring", false);
		}
	}

	public void CloseDoor(Godot.Collections.Array doors)
	{
		GD.Print("Close door");
		foreach (Node door in doors)
		{
			door.GetNode<Sprite>("Sprite").Visible = true;
			door.GetNode<CollisionShape2D>("Collider")
				.CallDeferred("set", "disabled", false);
		}
	}

	public void OpenDoor(Godot.Collections.Array doors)
	{
		GD.Print("Open door");
		foreach (Node door in doors)
		{
			door.GetNode<Sprite>("Sprite").Visible = false;
			door.GetNode<CollisionShape2D>("Collider")
				.CallDeferred("set", "disabled", true);
		}
	}
}
