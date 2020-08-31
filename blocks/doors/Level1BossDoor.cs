using Godot;
using System;

public class Level1BossDoor : StaticBody2D
{
	private Area2D Trigger;
	private CollisionShape2D Collider;
	private Sprite sprite;
	public override void _Ready()
	{
		sprite = GetNode<Sprite>("Sprite");
		sprite.Visible = false;

		Trigger = GetNode<Area2D>("CloseDoor");
		Trigger.Connect("body_entered", this, nameof(_on_BodyEnter));

		Collider = GetNode<CollisionShape2D>("Collider");
		Collider.Disabled = true;
	}

	public virtual void _on_BodyEnter(Node2D body)
	{
		sprite.Visible = true;
		Collider.CallDeferred("set", "disabled", false);
	}
}
