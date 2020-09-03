using Godot;
using System;

public class Dummy : Actor
{
	[Export] public int Health = 10;
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

		if (Health <= 0)
			this.QueueFree();
	}

	public void Hit(int damage)
	{
		GD.Print(this.Name, " Got hit took ", damage, " damage");
		Health -= damage;
	}
}
