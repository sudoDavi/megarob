using Godot;
using System;

public class Boss3 : Actor
{
	[Export] public bool HasAwakened = false;
	[Export] public float ShotRate = 0.25f;
	[Export] public int bossDamage = 3;
	public int AttackPattern = 1;
	private int currentGun = 0;
	private bool firing = false;
	private Area2D hitZone;
	private Node2D player;
	private static float shotTime = 0.0f;
	private static Vector2[] shotDirections = {
		new Vector2(-1.0f, -1.0f),
		Vector2.Up,
		new Vector2(1.0f, -1.0f),
		Vector2.Right,
		new Vector2(1.0f, 1.0f),
		Vector2.Down,
		new Vector2(-1.0f, 1.0f),
		Vector2.Left,
	};
	private Godot.Collections.Array<Position2D> posititions =
	 new Godot.Collections.Array<Position2D>();
	private Godot.Collections.Array<Position2D> guns =
	 new Godot.Collections.Array<Position2D>();

	private static Random rand = new Random();
	private static int currentPos = 6;
	private static PackedScene shot = GD.Load<PackedScene>("res://entity/enemies/e_shot.tscn");
	private static PackedScene keyPickup = GD.Load<PackedScene>("res://player/key_pickup.tscn");
	private static int hitCounter = 0;

	public override void _Ready()
	{
		player = GetNode<Node2D>("/root/Main/Player");
		Animation = GetNode<AnimatedSprite>("Animation");
		Health = 100;
		hitZone = GetNode<Area2D>("HitZone");
		hitZone.Connect("body_entered", this, nameof(_on_BodyEnter));
		hitSound = GetNode<AudioStreamPlayer>("HitSound");

		for (int i = 0; i < 8; i++)
		{
			guns.Add(GetNode<Position2D>("Gun" + i));
		}

		for (int i = 0; i < 12; i++)
		{
			posititions.Add(GetNode<Position2D>($"/root/Main/Level3/Enemies/BPs/BP{i}"));
		}
	}
	public override void _PhysicsProcess(float delta)
	{
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
	}

	private void FireGuns(float delta)
	{
		shotTime += delta;
		if (shotTime >= ShotRate)
		{
			if (AttackPattern == 1)
			{
				EShot firedShot = (EShot)shot.Instance();
				firedShot.ShotDirection = shotDirections[currentGun];
				firedShot.GlobalPosition = guns[currentGun].GlobalPosition;
				firedShot.ShotDamage = bossDamage;
				firedShot.AddCollisionExceptionWith(this);
				GetParent().AddChild(firedShot);
				currentGun++;
				if (currentGun > 7)
					currentGun = 0;
			}
			else
			{
				for (int i = 0; i < 8; i++)
				{
					EShot firedShot = (EShot)shot.Instance();
					firedShot.ShotDirection = shotDirections[i];
					firedShot.GlobalPosition = guns[i].GlobalPosition;
					firedShot.ShotDamage = bossDamage;
					firedShot.AddCollisionExceptionWith(this);
					GetParent().AddChild(firedShot);
				}
			}


			shotTime = 0.0f;
		}
	}

	public override void Hit(int damage)
	{
		GD.Print("Boss hit");
		hitCounter++;
		Health -= damage;
		hitSound.Play();
		if (Health <= 0)
		{
			Die();
			GD.Print("Boss died");
		}
		if (Health <= 35)
		{
			AttackPattern = 2;
			ShotRate = 1f;
		}

		if (hitCounter > 2)
		{
			int nextRand;
			do
			{
				nextRand = rand.Next(12);
			} while (nextRand == currentPos);
			hitCounter = 0;
			this.GlobalPosition = posititions[nextRand].GlobalPosition;
			currentPos = nextRand;
		}


	}

	public override void Die()
	{
		GetNode("/root/Main/Level3/SpecialBlocks/BossArena").Call("ChangeState");
		GetNode("/root/Main/Level3/BossTrigger").Set("monitoring", false);
		GetNode("/root/Main/Player").Set("KilledBoss", 3);
		GetNode("/root/Main/Player").Call("FightEnded");
		var pickup = (Node2D)keyPickup.Instance();
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
