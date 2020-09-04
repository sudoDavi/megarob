using Godot;
using System;

public class BossArena : Node2D
{
	[Export] public int IsInLevel = 0;

	public void FreePlayer()
	{
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
