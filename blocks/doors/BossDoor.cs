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

			OpenDoor(GetTree().GetNodesInGroup("L" + IsInLevel + "BossDoor"));
		}
	}

	public void _on_BodyEnter(Node2D body)
	{
		if (body.IsInGroup("Player") && (int)body.Get("IsInLevel") == IsInLevel)
		{
			CloseDoor(GetTree().GetNodesInGroup("L" + IsInLevel + "BossDoor"));
		}
	}

	public void CloseDoor(Godot.Collections.Array doors)
	{
		foreach (Node door in doors)
		{
			door.GetNode<Sprite>("Sprite").Visible = true;
			door.GetNode<CollisionShape2D>("Collider")
				.CallDeferred("set", "disabled", false);
		}
	}

	public void OpenDoor(Godot.Collections.Array doors)
	{
		foreach (Node door in doors)
		{
			door.GetNode<Sprite>("Sprite").Visible = false;
			door.GetNode<CollisionShape2D>("Collider")
				.CallDeferred("set", "disabled", true);
		}
	}
}
