using Godot;
using System;

public class Player : Actor
{
	[Export] public int PlayerDamage = 3;
	public bool IsRespawning = true;
	public int IsInLevel = 1;
	public int KilledBoss = 0;
	[Export] public bool HasGun = false;
	[Export] public bool HasDJmp = false;
	private static bool canDJmp = true;
	private static Vector2 respawn;
	private static Position2D gun;
	private static float shotTime = 0.0f;
	[Export] float ShotRate = 0.25f;
	private static PackedScene shot = GD.Load<PackedScene>("res://player/shot.tscn");
	private static PackedScene level1 = GD.Load<PackedScene>("res://Level1.tscn");
	private static PackedScene level2 = GD.Load<PackedScene>("res://Level2.tscn");
	public override void _Ready()
	{
		IsRespawning = false;
		PlatformDetector = GetNode<RayCast2D>("PlatformDetector");
		Animation = GetNode<AnimatedSprite>("Animation");
		gun = GetNode<Position2D>("Animation/Gun");
		respawn = GetNode<Position2D>($"/root/Main/Hub/Start{IsInLevel}")
			.GlobalPosition;
		this.GlobalPosition = respawn;
	}
	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);

		HandleMovement(delta);
		HandleGun(delta);

		if (Input.IsActionJustPressed("respawn"))
			this.GlobalPosition = respawn;
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
			&& (shotTime >= ShotRate)
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
		IsRespawning = true;
		GetTree().Paused = true;
		GetNode($"/root/Main/Level{IsInLevel}")
			.Connect("tree_exited", this, nameof(_Respawn));
		GetNode($"/root/Main/Level{IsInLevel}").QueueFree();
	}

	public void _Respawn()
	{
		GetTree().Paused = false;
		this._Ready();
		Node2D loadedLevel;
		switch (IsInLevel)
		{
			case 1:
				loadedLevel = (Node2D)level1.Instance();
				HasGun = false;
				break;
			case 2:
				loadedLevel = (Node2D)level2.Instance();
				HasGun = true;
				HasDJmp = false;
				break;
			default:
				loadedLevel = new Node2D();
				break;
		}
		GetParent().CallDeferred("add_child", loadedLevel);
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
