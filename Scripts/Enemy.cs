using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	private NavigationAgent3D _navigationAgent;

	[Export]
	private float movementSpeed = 2.0f;
	[Export]
	private float movementFrequency = 1f;
	[Export]
	private float patrolAreaRadius = 3f;

	private Vector3 spawnPosition = Vector3.Zero;

	public Vector3 MovementTarget
	{
		get { return _navigationAgent.TargetPosition; }
		set { _navigationAgent.TargetPosition = value; }
	}

	public override void _Ready()
	{
		base._Ready();

		_navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");

		// These values need to be adjusted for the actor's speed
		// and the navigation layout.
		_navigationAgent.PathDesiredDistance = 0.5f;
		_navigationAgent.TargetDesiredDistance = 0.5f;

		// Get spawn position
		spawnPosition = Position;

		// Make sure to not await during _Ready.
		Callable.From(ActorSetup).CallDeferred();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (_navigationAgent.IsNavigationFinished())
		{
			return;
		}

		Vector3 currentAgentPosition = GlobalTransform.Origin;
		Vector3 nextPathPosition = _navigationAgent.GetNextPathPosition();

		Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * movementSpeed;
		MoveAndSlide();
	}

	private async void ActorSetup()
	{
		// Wait for the first physics frame so the NavigationServer can sync.
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

		// Now that the navigation map is no longer empty, set the movement target.
		MovementTarget = spawnPosition;
	}

	private void OnTimerTimeout()
	{
		// Calculate new position
		float angle = (float)GD.RandRange(0.0d, Math.PI * 2d);
		float distance = (float)GD.RandRange(1d, patrolAreaRadius);
		Vector3 newPosition = spawnPosition + Transform.Basis.Column2.Rotated(Vector3.Up, angle) * distance;

		// Set as new target
		MovementTarget = newPosition;
	}

	private void OnHealthHasDied()
	{
		// Despawn the object
		QueueFree();
	}
}
