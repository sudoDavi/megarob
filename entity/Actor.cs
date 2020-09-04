using Godot;
using System;

public class Actor : KinematicBody2D
{
	[Export] public Vector2 Speed = new Vector2(150.0f, 250.0f);
	[Export] public int MaxSlides = 4;
	[Export] public float FloorMaxAngle = 0.9f;
	[Export] public bool InfiniteInertia = false;
	[Export] public bool StopOnSlopes = true;
	[Export] public int Health = 10;
	public Vector2 Velocity = Vector2.Zero;
	public AnimatedSprite Animation;
	public RayCast2D PlatformDetector;
	public object Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity");

	public Vector2 FloorNormal
	{
		get { return Vector2.Up; }
	}

	[Export] public Vector2 FloorDetectDistance = new Vector2(0.0f, 5.0f);

	private void ApplyGravity(float delta)
	{
		Velocity.y += (int)Gravity * delta;
	}

	public override void _PhysicsProcess(float delta)
	{
		ApplyGravity(delta);

		if (Health <= 0)
			Die();
	}

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

	public virtual void Die()
	{
		this.QueueFree();
	}

	public virtual void Hit(int damage)
	{
		GD.Print(this.Name, " Got hit took ", damage, " damage");
		Health -= damage;
	}
}
