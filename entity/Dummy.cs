using Godot;
using System;

public class Dummy : Actor
{
	public Vector2 velocity = Vector2.Zero;
	private void ApplyGravity(float delta)
	{
		velocity.y += (int)Gravity * delta;
	}

	public override void _PhysicsProcess(float delta)
	{
		ApplyGravity(delta);
		MoveAndSlide(
			velocity,
			FloorNormal,
			StopOnSlopes,
			MaxSlides,
			FloorMaxAngle,
			InfiniteInertia
		);
	}
}
