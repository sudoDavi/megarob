using Godot;
using System;

public class Actor : KinematicBody2D
{
	[Export] public int MaxSlides = 4;
	[Export] public float FloorMaxAngle = 0.9f;
	[Export] public bool InfiniteInertia = false;
	[Export] public bool StopOnSlopes = true;
	public Vector2 velocity = Vector2.Zero;
	public AnimatedSprite Animation;
	public RayCast2D PlatformDetector;
	public object Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity");

	public Vector2 FloorNormal
	{
		get { return Vector2.Up; }
	}

	[Export] public Vector2 FloorDetectDistance = new Vector2(0.0f, 5.0f);

	public virtual Vector2 CalculateMoveVelocity(
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
}
