using Godot;
using System;

public class EscapeDoor : Node2D
{
	private int killedBosses = 0;
	private Node2D player;
	private Area2D doorTrigger;
	private Area2D consoleTrigger;
	private AnimatedSprite consoleSprite;
	private AnimatedSprite doorSprite;
	private bool openDoor = false;
	private bool playerEntered = false;
	private Sprite need;
	private Sprite key;
	private Sprite f;
	private Sprite press;

	public override void _Ready()
	{
		player = GetNode<Node2D>("/root/Main/Player");
		doorTrigger = GetNode<Area2D>("DoorTrigger");
		consoleTrigger = GetNode<Area2D>("ConsoleTrigger");
		consoleSprite = GetNode<AnimatedSprite>("Console");
		doorSprite = GetNode<AnimatedSprite>("Door");
		need = GetNode<Sprite>("need");
		key = GetNode<Sprite>("key");
		press = GetNode<Sprite>("press");
		f = GetNode<Sprite>("f");

		consoleTrigger.Connect("body_entered", this, nameof(_inputConsole));
		consoleTrigger.Connect("body_exited", this, nameof(_consoleExit));
		doorTrigger.Connect("body_entered", this, nameof(_on_BodyEnter));
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		var hasKey = (bool)player.Get("HasKey");
		if (hasKey && @event.IsActionPressed("use") && playerEntered)
		{
			consoleSprite.Frame = 1;
			doorSprite.Frame = 1;
			openDoor = true;
		}
	}

	public void _inputConsole(Node2D body)
	{
		if (body.IsInGroup("Player") && (bool)player.Get("HasKey"))
		{
			playerEntered = true;
			press.Visible = true;
			f.Visible = true;
		}
		else if (body.IsInGroup("Player"))
		{
			playerEntered = true;
			need.Visible = true;
			key.Visible = true;
		}
	}

	public void _consoleExit(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			playerEntered = false;
			need.Visible = false;
			key.Visible = false;
			press.Visible = false;
			f.Visible = false;
		}
	}

	public void _on_BodyEnter(Node2D body)
	{
		if (openDoor && body.IsInGroup("Player"))
		{
			GetNode<TextureRect>("/root/Main/Ui/EndScreen").Visible = true;
			GetNode<Control>("/root/Main/Ui/GameUI").Visible = false;
			GetNode<TextureRect>("/root/Main/Ui/MainMenu").SetProcess(false);
			GetTree().Paused = true;
		}
	}
}
