using Godot;
using System;

public class Boss2 : Actor
{
	[Export] public bool HasAwakened = false;
	[Export] public float ShotRate = 1f;
	[Export] public int bossDamage = 3;
	private bool firing = false;
	private Area2D hitZone;
	private Node2D player;
	private static float shotTime = 0.0f;
	private static Vector2[] shotDirections = {
		Vector2.Left,
		new Vector2(-1.0f, -1.0f),
		Vector2.Up,
		new Vector2(1.0f, -1.0f),
		Vector2.Right
	};
	private Godot.Collections.Array<Position2D> posititions =
	 new Godot.Collections.Array<Position2D>();
	private Godot.Collections.Array<Position2D> guns =
	 new Godot.Collections.Array<Position2D>();

	private static Random rand = new Random();
	private static int currentPos = 7;
	private static PackedScene shot = GD.Load<PackedScene>("res://entity/enemies/e_shot.tscn");
	private static PackedScene dJmpP = GD.Load<PackedScene>("res://player/d_jump_pickup.tscn");
	private static int hitCounter = 0;

	public override void _Ready()
	{
		player = GetNode<Node2D>("/root/Main/Player");
		Animation = GetNode<AnimatedSprite>("Animation");
		Health = 50;
		hitZone = GetNode<Area2D>("HitZone");
		hitZone.Connect("body_entered", this, nameof(_on_BodyEnter));
		hitSound = GetNode<AudioStreamPlayer>("HitSound");

		for (int i = 0; i < 5; i++)
		{
			guns.Add(GetNode<Position2D>("Gun" + i));
		}

		for (int i = 0; i < 8; i++)
		{
			posititions.Add(GetNode<Position2D>($"/root/Main/Level2/Enemies/BPs/BP{i}"));
		}
	}
	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);

		HandleBoss(delta);
	}

	private void HandleBoss(float delta)
	{
		if (HasAwakened)
		{
			Animation.Play();
			FireGuns(delta);
		}
		else
			Animation.Stop();

		Velocity = MoveAndSlide(
			Velocity,
			FloorNormal,
			StopOnSlopes,
			MaxSlides,
			FloorMaxAngle,
			InfiniteInertia
		);
	}

	private void FireGuns(float delta)
	{
		shotTime += delta;
		if (shotTime >= ShotRate)
		{
			for (int i = 0; i < 5; i++)
			{
				EShot firedShot = (EShot)shot.Instance();
				firedShot.ShotDirection = shotDirections[i];
				firedShot.GlobalPosition = guns[i].GlobalPosition;
				firedShot.ShotDamage = bossDamage;
				firedShot.AddCollisionExceptionWith(this);
				GetParent().AddChild(firedShot);
			}

			shotTime = 0.0f;
		}
	}

	public override void Hit(int damage)
	{
		GD.Print("Boss hit");
		Health -= damage;
		if (Health <= 0)
		{
			Die();
			GD.Print("Boss died");
		}

		if (hitCounter == 3)
		{
			int nextRand;
			do
			{
				nextRand = rand.Next(8);
			} while (nextRand == currentPos);
			hitCounter = 0;
			this.GlobalPosition = posititions[nextRand].GlobalPosition;
			currentPos = nextRand;
		}

		hitCounter++;
	}

	public override void Die()
	{
		GetNode("/root/Main/Level2/SpecialBlocks/BossArena").Call("ChangeState");
		GetNode("/root/Main/Level2/BossTrigger").Set("monitoring", false);
		GetNode("/root/Main/Player").Set("KilledBoss", 2);
		GetNode("/root/Main/Player").Call("FightEnded");
		var pickup = (Node2D)dJmpP.Instance();
		pickup.GlobalPosition = this.GlobalPosition;
		GetParent().AddChild(pickup);
		this.QueueFree();
	}

	public void _on_BodyEnter(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			GD.Print("Player hit");
			body.CallDeferred("Hit");
		}
	}
}
