using Godot;
using System;

public class EscapeDoor : Node2D
{
	private int killedBosses = 0;
	private Node2D player;
	private Area2D doorTrigger;
	private Area2D consoleTrigger;
	private AnimatedSprite consoleSprite;
	private bool openDoor = false;
	private bool playerEntered = false;

	public override void _Ready()
	{
		player = GetNode<Node2D>("/root/Main/Player");
		doorTrigger = GetNode<Area2D>("DoorTrigger");
		consoleTrigger = GetNode<Area2D>("ConsoleTrigger");
		consoleSprite = GetNode<AnimatedSprite>("Console");

		consoleTrigger.Connect("body_entered", this, nameof(_inputConsole));
		doorTrigger.Connect("body_entered", this, nameof(_on_BodyEnter));
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		var hasKey = (bool)player.Get("HasKey");
		if (hasKey && @event.IsActionPressed("use") && playerEntered)
		{
			consoleSprite.Frame = 1;
			openDoor = true;
		}
	}

	public void _inputConsole(Node2D body)
	{
		if (body.IsInGroup("Player"))
			playerEntered = true;
	}

	public void _on_BodyEnter(Node2D body)
	{
		if (openDoor && body.IsInGroup("Player"))
			GD.Print("Finished Game");
	}
}
