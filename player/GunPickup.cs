using Godot;
using System;

public class GunPickup : Node2D
{
	private Area2D pickupTrigger;
	private Sprite destroy;
	private Sprite run;

	public override void _Ready()
	{
		pickupTrigger = GetNode<Area2D>("Trigger");
		pickupTrigger.Connect("body_entered", this, nameof(_on_BodyEnter));
		destroy = GetNode<Sprite>("/root/Main/Level1/Hints/destroy");
		run = GetNode<Sprite>("/root/Main/Level1/Hints/run");
	}

	public void _on_BodyEnter(Node2D body)
	{
		if (body.Name == "Player")
		{
			body.Call("PickupGun");
			destroy.Visible = true;
			run.QueueFree();
			this.QueueFree();
		}
	}
}
