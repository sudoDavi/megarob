using Godot;
using System;

public class Dummy : Actor
{
	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		MoveAndSlide(
			Velocity,
			FloorNormal,
			StopOnSlopes,
			MaxSlides,
			FloorMaxAngle,
			InfiniteInertia
		);
	}
}
