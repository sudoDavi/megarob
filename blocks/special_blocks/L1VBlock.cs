using Godot;
using System;

public class L1VBlock : StaticBody2D
{
	private Area2D Trigger;
	private CollisionShape2D Collider;
	private Sprite sprite;
	private Timer timer;
	public override void _Ready()
	{
		sprite = GetNode<Sprite>("Sprite");

		timer = GetNode<Timer>("Timer");
		timer.Connect("timeout", this, nameof(_timeout));

		Trigger = GetNode<Area2D>("Vanish");
		Trigger.Connect("body_entered", this, nameof(_on_BodyEnter));

		Collider = GetNode<CollisionShape2D>("Collider");
	}

	public void _timeout()
	{
		sprite.Visible = false;
		Collider.CallDeferred("set", "disabled", true);
	}

	public void _on_BodyEnter(Node2D body)
	{
		timer.Start();
	}
}
