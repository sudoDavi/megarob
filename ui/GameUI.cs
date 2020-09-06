using Godot;
using System;

public class GameUI : Control
{
	private ColorRect gun;
	private ColorRect dJmp;
	private ColorRect key;
	private Label health;
	private Node2D player;
	public override void _Ready()
	{
		gun = GetNode<ColorRect>("CenterContainer/HBoxContainer/Gun");
		key = GetNode<ColorRect>("CenterContainer/HBoxContainer/Key");
		dJmp = GetNode<ColorRect>("CenterContainer/HBoxContainer/DJmp");
		health = GetNode<Label>("HP");
		player = GetNode<Node2D>("/root/Main/Player");
	}

	public override void _Process(float delta)
	{
		health.Text = "HP: " + player.Get("Health");
		gun.Visible = (bool)player.Get("HasGun");
		dJmp.Visible = (bool)player.Get("HasDJmp");
		key.Visible = (bool)player.Get("HasKey");
	}
}
