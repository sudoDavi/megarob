using Godot;
using System;

public class Shot : KinematicBody2D
{
	[Export] float ShotVelocity = 3.5f;
	[Export] Vector2 shotVector;
	// Needs to be either -1 or 1 to change the direction of the shot
	// 1 the shot goes right, -1 it goes left
	public int ShotDirection = 1;
	public override void _Ready()
	{
		shotVector = new Vector2((float)ShotDirection * ShotVelocity, 0);
	}

	public override void _PhysicsProcess(float delta)
	{
		KinematicCollision2D collision = MoveAndCollide(shotVector);
		if (collision != null)
		{
			GD.Print(collision.ColliderId, collision.Collider);
			this.QueueFree();
		}
	}
}
