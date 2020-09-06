using Godot;
using System;

public class BossArena : Node2D
{
	[Export] public int IsInLevel = 0;
	[Export] public bool StartFree = false;
	public bool Active = true;
	public override void _Ready()
	{
		if (StartFree)
			ChangeState();
	}

	public void ChangeState()
	{
		GD.Print("change state called");
		Active = !Active;
		foreach (Node node in GetTree().GetNodesInGroup($"L{IsInLevel}BossArena"))
		{
			bool spriteVisibility = node.GetNode<Sprite>("Sprite").Visible;
			bool colliderState = node.GetNode<CollisionShape2D>("Collider").Disabled;

			node.GetNode("Sprite").CallDeferred("set", "visible", !spriteVisibility);
			node.GetNode("Collider")
				.CallDeferred("set", "disabled", !colliderState);
		}
	}

}
