using Godot;
using System;

public class Player : Actor
{
	private Position2D respawn;
	public override void _Ready()
	{
		PlatformDetector = GetNode<RayCast2D>("PlatformDetector");
		Animation = GetNode<AnimatedSprite>("Animation");
		respawn = GetNode<Position2D>("/root/Main/Level1/Respawn");
	}
	public override void _PhysicsProcess(float delta)
	{
		HandleMovement(delta);

		if (Input.IsActionJustPressed("respawn"))
			this.Transform = respawn.Transform;
	}

	private Vector2 CalculateMoveVelocity(
		Vector2 linearVelocity, Vector2 direction, Vector2 speed, bool isJumpInterrupted
	)
	{
		Vector2 _velocity = linearVelocity;
		_velocity.x = speed.x * direction.x;

		if (direction.y != 0.0f)
			_velocity.y = speed.y * direction.y;
		if (isJumpInterrupted)
			// Decrease the Y velocity by multiplying it, but don't set it to 0
			// as to not be too abrupt.
			_velocity.y *= 0.6f;

		return _velocity;
	}

	private void ApplyGravity(float delta)
	{
		velocity.y += (int)Gravity * delta;
	}
	private void HandleAnimation(Vector2 lookDirection)
	{
		// Animation.Stop();
		if (lookDirection.x > 0.0f)
		{
			Animation.FlipH = false;
			Animation.Play();
		}
		else if (lookDirection.x < 0.0f)
		{
			Animation.FlipH = true;
			Animation.Play();
		}
		else
			Animation.Stop();
	}
	private void HandleMovement(float delta)
	{
		ApplyGravity(delta);

		float willJump = IsOnFloor() && Input.IsActionJustPressed("jump") ? -1 : 0;
		Vector2 direction = new Vector2(
			Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
			willJump
		);
		Vector2 snapVector;
		if (direction.y == 0.0f)
			snapVector = Vector2.Down * FloorDetectDistance;
		else
			snapVector = Vector2.Zero;
		HandleAnimation(direction);

		bool isJumpInterrupted = Input.IsActionJustPressed("jump") && velocity.y < 0.0f;
		velocity = CalculateMoveVelocity(velocity, direction, speed, isJumpInterrupted);

		StopOnSlopes = !PlatformDetector.IsColliding();

		velocity = MoveAndSlideWithSnap(
			velocity, snapVector, FloorNormal, StopOnSlopes, MaxSlides, FloorMaxAngle, InfiniteInertia
		);
	}


}
