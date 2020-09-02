using Godot;
using System;

public class Actor : KinematicBody2D
{
	[Export] public int MaxSlides = 4;
	[Export] public float FloorMaxAngle = 0.9f;
	[Export] public bool InfiniteInertia = false;
	[Export] public bool StopOnSlopes = true;
	public AnimatedSprite Animation;
	public RayCast2D PlatformDetector;
	public object Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity");

	public Vector2 FloorNormal
	{
		get { return Vector2.Up; }
	}

	public Vector2 FloorDetectDistance
	{
		get { return new Vector2(0.0f, 10.0f); }
	}
}
