using Godot;
using System;

public class MainMenu : TextureRect
{
	private TextureButton play;
	private TextureButton quit;
	private TextureButton resume;
	private TextureButton fs;
	private Control gameUI;
	public override void _Ready()
	{
		play = GetNode<TextureButton>("Play");
		quit = GetNode<TextureButton>("Quit");
		resume = GetNode<TextureButton>("Resume");
		fs = GetNode<TextureButton>("FullScreen");
		gameUI = GetParent().GetNode<Control>("GameUI");
		resume.Visible = false;

		play.Connect("pressed", this, nameof(_pressedPlay));
		quit.Connect("pressed", this, nameof(_pressedQuit));
		resume.Connect("pressed", this, nameof(_pressedPlay));
		fs.Connect("pressed", this, nameof(_fullScreen));
	}

	public void _fullScreen()
	{
		OS.WindowFullscreen = !OS.WindowFullscreen;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsActionPressed("pause"))
		{
			gameUI.Visible = false;
			this.Visible = true;
			play.Visible = false;
			resume.Visible = true;
			GetTree().Paused = true;
			Input.SetMouseMode(Input.MouseMode.Visible);
		}
	}

	public void _pressedPlay()
	{
		gameUI.Visible = true;
		this.Visible = false;
		GetTree().Paused = false;
		Input.SetMouseMode(Input.MouseMode.Hidden);
	}

	public void _pressedQuit()
	{
		GetTree().Quit();
	}

}
