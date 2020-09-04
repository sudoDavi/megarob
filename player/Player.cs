using Godot;
using System;

public class Player : Actor
{
	[Export] public int PlayerDamage = 3;
	public bool MovementDisabled = false;
	public int IsInLevel = 1;
	public static bool HasGun = false;
	public static bool HasDJmp = false;
	private static bool canDJmp = true;
	private static Position2D respawn;
	private static Position2D gun;
	private static float shotTime = 0.0f;
	[Export] float ShotCap = 0.25f;
	private static PackedScene shot = GD.Load<PackedScene>("res://player/shot.tscn");
	private static PackedScene level1 = GD.Load<PackedScene>("res://Level1.tscn");
	public override void _Ready()
	{
		PlatformDetector = GetNode<RayCast2D>("PlatformDetector");
		Animation = GetNode<AnimatedSprite>("Animation");
		gun = GetNode<Position2D>("Animation/Gun");
		this.Transform = GetNode<Position2D>("/root/Main/Level1/Start").Transform;
	}
	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);

		HandleMovement(delta);
		HandleGun(delta);

		if (Input.IsActionJustPressed("respawn"))
			this.Transform = respawn.Transform;
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
		if (MovementDisabled)
			direction = Vector2.Zero;
		Vector2 snapVector;
		if (direction.y == 0.0f)
			snapVector = Vector2.Down * FloorDetectDistance;
		else
			snapVector = Vector2.Zero;
		HandleAnimation(direction);

		bool isJumpInterrupted = Input.IsActionJustPressed("jump") && Velocity.y < 0.0f;
		Velocity = CalculateMoveVelocity(Velocity, direction, Speed, isJumpInterrupted);

		StopOnSlopes = !PlatformDetector.IsColliding();

		Velocity = MoveAndSlideWithSnap(
			Velocity, snapVector, FloorNormal, StopOnSlopes, MaxSlides, FloorMaxAngle, InfiniteInertia
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
			firedShot.ShotDamage = PlayerDamage;
			firedShot.AddCollisionExceptionWith(this);
			firedShot.AddToGroup("player_shot");
			GetParent().AddChild(firedShot);

			shotTime = 0.0f;
		}
	}

	public override void Die()
	{
		this.Transform =
			GetNode<Position2D>($"/root/Main/Level{IsInLevel}/Start").Transform;
		Health = 10;
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
