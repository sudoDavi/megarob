using Godot;
using System;

public class Boss1 : Actor
{
	[Export] public bool HasAwakened = false;
	private Area2D killZone;

	public override void _Ready()
	{
		Speed = new Vector2(50.0f, 0.0f);
		Animation = GetNode<AnimatedSprite>("Animation");
		Health = 20;
		killZone = GetNode<Area2D>("KillZone");
		killZone.Connect("body_entered", this, nameof(_on_BodyEnter));
	}
	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		if (this.IsQueuedForDeletion())
		{
			GetNode("/root/Main/Level1/SpecialBlocks/BossArena").Call("FreePlayer");
		}

		HandleMovement(delta);
	}


	private Vector2 CalculateDirection()
	{
		if (
			GetNode<Node2D>("/root/Main/Player")
				.GlobalPosition.x < this.GlobalPosition.x
		)
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
			body.Call("Die");
		}
	}
}
