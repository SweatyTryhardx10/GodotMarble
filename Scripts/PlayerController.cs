using Godot;

public partial class PlayerController : RigidBody3D
{
	public static PlayerController Instance { get; private set; }

	public override void _EnterTree()
	{
		Instance = this;

		// Callbacks (for roll sound)
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}
	public override void _ExitTree()
	{
		Instance = null;
		
		// Callbacks (for roll sound)
		BodyEntered -= OnBodyEntered;
		BodyExited -= OnBodyExited;
	}

	public override void _Ready()
	{
		base._Ready();

		// Instantiate and configure camera on spawn
		if (spawnCamera)
		{
			var cam = cameraPrefab.Instantiate<ChaseCamera>();
			cam.target = meshNode;
			GetParent().AddChild(cam);
		}
	}

	private void OnBodyEntered(Node n)
	{
		if (this.GetContactCount() > 0)
		{
			rollSound.Play();
		}
	}

	private void OnBodyExited(Node n)
	{
		if (this.GetContactCount() == 0)
		{
			rollSound.Stop();
		}
	}

	[Export] private float Speed = 5.0f;
	[Export] private float JumpVelocity = 4.5f;
	[Export] private float SlamVelocity = 4.5f;
	[Export] private float SlamForce = 500f;
	[Export] private Area3D area3D;
	[Export] private AudioStreamPlayer3D groundSlamSound;
	[Export] private AudioStreamPlayer3D jumpSound;
	[Export] private AudioStreamPlayer3D landSound;
	[Export] private AudioStreamPlayer3D rollSound;
	[Export] private ParticleCustom slamParticles;
	[Export] private ParticleCustom landParticles;
	[Export] private PackedScene cameraPrefab;
	[Export] private Node3D meshNode;
	private bool isSlamming = false;

	[Export] private bool spawnCamera = true;

	private Vector3 oldVel;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		var cameraBasis = GetViewport().GetCamera3D().Transform.Basis;
		Vector3 direction = inputDir.X * cameraBasis.X.Normalized() + inputDir.Y * cameraBasis.Z.Normalized();
		direction.Y = 0;
		ApplyForce(direction.Normalized() * Speed);

		if (isSlamming)
		{
			ApplyForce(Vector3.Down * SlamVelocity);
			if (GetContactCount() > 0)
			{
				isSlamming = false;
				foreach (var item in area3D.GetOverlappingBodies())
				{
					if (item is RigidBody3D item2)
					{
						if (item2.ContactMonitor == true && item2.MaxContactsReported > 0)
						{
							if (item2.GetContactCount() > 0)
							{
								item2.ApplyForce(Vector3.Up * SlamForce);
								groundSlamSound.Play();
								slamParticles.Play();
							}
						}
						else
						{
							item2.ApplyForce(Vector3.Up * SlamForce);
							groundSlamSound.Play();
							slamParticles.Play();
						}
					}
				}
			}
		}

		// Rolling sound pitch
		rollSound.PitchScale = Mathf.Clamp(LinearVelocity.Length() / 4f, 0.2f, 4f);
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		base._IntegrateForces(state);

		if (state.GetContactCount() > 0)
		{
			Vector3 normal = state.GetContactLocalNormal(0);

			// Play land sound of the "normal-velocity" is above a threshold
			if (oldVel.Project(-normal).Length() > 0.5f && oldVel.Normalized().Dot(normal) < 0.5f)
			{
				landSound.Play();
				landParticles.Play();
			}
		}

		oldVel = state.LinearVelocity;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept") && GetContactCount() > 0)
		{
			jumpSound.Play();
			ApplyCentralImpulse(Vector3.Up * JumpVelocity);
		}
		if (Input.IsActionJustPressed("ui_accept") && GetContactCount() <= 0)
		{
			isSlamming = true;
		}
	}

	public static void Respawn(Vector3 position)
	{
		if (!IsInstanceValid(Instance))
			return;

		// Reset player's position to the spawn position
		Instance.GlobalPosition = position;
		Instance.LinearVelocity = Vector3.Zero;
		Instance.AngularVelocity = Vector3.Zero;

		GD.Print("Player has been respawned!");
	}
}