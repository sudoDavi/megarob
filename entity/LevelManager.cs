using Godot;
using System;

public class LevelManager : Node2D
{
	[Export] public int LevelToManage = 0;
	private Area2D loader;
	private Area2D unloader;
	private PackedScene packedLevel;

	public override void _Ready()
	{
		loader = GetNode<Area2D>("Loader");
		unloader = GetNode<Area2D>("Unloader");

		loader.Connect("body_entered", this, nameof(_LoadLevel));
		unloader.Connect("body_entered", this, nameof(_UnloadLevel));

		packedLevel = GD.Load<PackedScene>($"res://Level{LevelToManage}.tscn");
	}

	private bool IsLoaded()
	{
		if (GetNodeOrNull($"/root/Main/Level{LevelToManage}") != null)
			return true;
		else
			return false;
	}

	public void _LoadLevel(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			if (!IsLoaded())
			{
				Node2D mainNode = GetNode<Node2D>("/root/Main");
				Node2D loadedLevel = (Node2D)packedLevel.Instance();

				mainNode.CallDeferred("add_child", loadedLevel);
			}
		}
	}

	public void _UnloadLevel(Node body)
	{
		if (body.IsInGroup("Player"))
			if (IsLoaded())
				GetNode($"/root/Main/Level{LevelToManage}").QueueFree();
	}
}
