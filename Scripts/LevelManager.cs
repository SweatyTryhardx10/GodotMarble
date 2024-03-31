using Godot;
using System;
using System.Threading;

public partial class LevelManager : Node3D
{
	private static LevelManager Instance;
	
	public static int loadedLevelIndex;
	public static float CompletionTime { get { if (IsInstanceValid(Instance)) return Instance.completionTime; else return -1f; } }

	private float startTime;
	private float timeElapsed => Time.GetTicksMsec() - startTime;
	private float completionTime = -1f;

	public override void _EnterTree() => Instance = this;
	public override void _ExitTree() => Instance = null;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startTime = Time.GetTicksMsec();
		// GetTree().CreateTimer(3.0d).Timeout += () => { Spawn.Instance.SpawnPlayer(); };
		Spawn.SpawnPlayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HUD.SetGameTimer(timeElapsed / 1000.0f);
	}

	public static void ReloadLevel()
	{
		if (!IsInstanceValid(Instance))
			return;

		// Reload the active scene
		Instance.GetTree().ReloadCurrentScene();
	}

	public static void CompleteLevel()
	{
		if (!IsInstanceValid(Instance))
			return;

		Instance.completionTime = Instance.timeElapsed;
		
		SaveLoadUtil.SaveGame();
	}
}
