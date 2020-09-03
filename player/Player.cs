using Godot;
using System;

public class Player : Actor
{
	[Export] public int Health = 10;
	[Export] public Vector2 speed = new Vector2(150.0f, 250.0f);
	public Vector2 velocity = Vector2.Zero;
	public static bool HasGun = false;
	public static bool HasDJmp = false;
	private static bool canDJmp = true;
	private static Position2D respawn;
	private static Position2D gun;
	private static float shotTime = 0.0f;
	[Export] float ShotCap = 0.25f;
	private static PackedScene shot = GD.Load<PackedScene>("res://player/shot.tscn");
	public override void _Ready()
	{
		PlatformDetector = GetNode<RayCast2D>("PlatformDetector");
		Animation = GetNode<AnimatedSprite>("Animation");
		respawn = GetNode<Position2D>("/root/Main/Level1/Respawn");
		gun = GetNode<Position2D>("Animation/Gun");
		this.Transform = GetNode<Position2D>("/root/Main/Start").Transform;
	}
	public override void _PhysicsProcess(float delta)
	{
		HandleMovement(delta);
		HandleGun(delta);

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
		if (lookDirection.x > 0.0f)
		{
			Animation.FlipH = false;
			Animation.Play();
			gun.Position = new Vector2(6.5f, 0);
		}
		else if (lookDirection.x < 0.0f)
		{
			Animation.FlipH = true;
			Animation.Play();
			gun.Position = new Vector2(-6.5f, 0);
		}
		else
			Animation.Stop();
	}
	private void HandleMovement(float delta)
	{
		ApplyGravity(delta);

		float willJump = 0.0f;
		if (Input.IsActionJustPressed("jump"))
		{
			if (IsOnFloor())
			{
				willJump = -1.0f;
				canDJmp = true;
			}
			else if (canDJmp && HasDJmp)
			{
				willJump = -1.0f;
				canDJmp = false;
			}
		}
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

	private void HandleGun(float delta)
	{
		shotTime += delta;
		bool shotCap = GetTree().GetNodesInGroup("player_shot").Count < 3;
		if (Input.IsActionJustPressed("shoot")
			&& (shotTime >= ShotCap)
			&& HasGun
			&& shotCap
		)
		{
			Shot firedShot = (Shot)shot.Instance();
			firedShot.ShotDirection = Animation.FlipH ? -1 : 1;
			firedShot.Transform = gun.GlobalTransform;
			firedShot.AddCollisionExceptionWith(this);
			firedShot.AddToGroup("player_shot");
			GetParent().AddChild(firedShot);

			shotTime = 0.0f;
		}
	}

	public void PickupGun()
	{
		GD.Print("Got Gun");
		HasGun = true;
	}

	public void PickupDJmp()
	{
		GD.Print("Got DJmp");
		HasDJmp = true;
	}
}
