using Godot;
using System;

public class DJumpPickup : Node2D
{
	private Area2D pickupTrigger;
	private Sprite doubleS;
	private Sprite jump;

	public override void _Ready()
	{
		pickupTrigger = GetNode<Area2D>("Trigger");
		pickupTrigger.Connect("body_entered", this, nameof(_on_BodyEnter));
	}

	public void _on_BodyEnter(Node2D body)
	{
		if (body.Name == "Player")
		{
			body.Call("PickupDJmp");
			this.QueueFree();
		}
	}
}
