using Godot;
using System;

public class Boss1 : Actor
{
	[Export] public bool HasAwakened;
	private Area2D killZone;
	private Node2D player;

	public override void _Ready()
	{
		HasAwakened = false;
		player = GetNode<Node2D>("/root/Main/Player");
		Speed = new Vector2(50.0f, 0.0f);
		Animation = GetNode<AnimatedSprite>("Animation");
		Health = 20;
		killZone = GetNode<Area2D>("KillZone");
		killZone.Connect("body_entered", this, nameof(_on_BodyEnter));
	}
	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		HandleMovement(delta);
	}

	public override void Die()
	{
		GetNode("/root/Main/Level1/SpecialBlocks/BossArena").Call("ChangeState");
		GetNode("/root/Main/Level1/BossTrigger").Set("monitoring", false);
		GetNode("/root/Main/Player").Set("KilledBoss", 1);

		this.QueueFree();
	}


	private Vector2 CalculateDirection()
	{
		if (player.GlobalPosition.x < this.GlobalPosition.x)
			return Vector2.Left;
		else
			return Vector2.Right;
	}

	private void HandleAnimation(Vector2 lookDirection)
	{
		if (lookDirection.x != 0.0f)
			Animation.Play();
		else
			Animation.Stop();
	}

	private void HandleMovement(float delta)
	{
		if (HasAwakened)
		{
			Vector2 direciton = CalculateDirection();
			Velocity = CalculateMoveVelocity(
				Velocity,
				direciton,
				Speed,
				true
			);

			HandleAnimation(direciton);
		}

		Velocity = MoveAndSlide(
			Velocity,
			FloorNormal,
			StopOnSlopes,
			MaxSlides,
			FloorMaxAngle,
			InfiniteInertia
		);
	}

	public void _on_BodyEnter(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			GD.Print("DIEEE");
			body.CallDeferred("Die");
		}
	}
}
