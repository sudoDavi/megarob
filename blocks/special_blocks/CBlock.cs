using Godot;
using System;

public class CBlock : StaticBody2D
{
	[Export] public int Health = 9;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (Health <= 0)
			this.QueueFree();
	}
	public void Hit(int damage)
	{
		Health -= damage;
	}
}
