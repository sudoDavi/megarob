using Godot;
using System;

public class KeyPickup : Node2D
{
	private Area2D pickupTrigger;

	public override void _Ready()
	{
		pickupTrigger = GetNode<Area2D>("Trigger");
		pickupTrigger.Connect("body_entered", this, nameof(_on_BodyEnter));
	}

	public void _on_BodyEnter(Node2D body)
	{
		if (body.Name == "Player")
		{
			body.Call("PickupKey");
			this.QueueFree();
		}
	}
}
