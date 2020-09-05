using Godot;
using System;

public class EShot : KinematicBody2D
{
	[Export] Vector2 ShotVelocity = new Vector2(1f, 1f);
	Vector2 shotVector = Vector2.Up;
	[Export] public Vector2 ShotDirection = Vector2.Up;
	[Export] public int ShotDamage = 3;
	private Area2D playerDetect;
	public override void _Ready()
	{
		playerDetect = GetNode<Area2D>("PlayerDetect");
		playerDetect.Connect("body_entered", this, nameof(_playerHit));
	}

	public override void _PhysicsProcess(float delta)
	{
		shotVector = ShotDirection * ShotVelocity;
		// shotVector *= delta;
		KinematicCollision2D collision = MoveAndCollide(shotVector);
	}

	public void _playerHit(Node2D body)
	{
		if (body.IsInGroup("Player"))
			body.Call("Hit", ShotDamage);

		this.QueueFree();
	}
}
