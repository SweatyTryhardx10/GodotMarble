using Godot;
using System;

public partial class HUD : Control
{
	private static HUD Instance;
	public static bool exists;

	[Export]
	private Control summaryScreen;
	[Export]
	private Timer timer;
	[Export]
	private Label timerLabel;

	[Export]
	private Label summaryCompletionTimeLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;

		if (!exists)
			exists = true;
		else
			QueueFree();

		timer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private static string SecondsToTimerFormat(double value)
	{
		int minutes = (int)Math.Floor(value / 60);
		int seconds = (int)value % 60;
		double milliseconds = value - Math.Floor(value);

		return $"{minutes.ToString("0#")}:{seconds.ToString("0#")}.{milliseconds.ToString("#.#0").Replace(',', ' ').TrimStart()}";
	}


	public static void ShowSummaryScreen()
	{
		if (Instance == null)
			return;

		Instance.summaryCompletionTimeLabel.Text = Instance.timerLabel.Text;

		Instance.summaryScreen.Visible = true;
	}

	public static void SetTimer(float seconds)
	{
		if (Instance == null)
			return;

		Instance.timerLabel.Text = SecondsToTimerFormat(seconds);
	}

	private void OnBtnRetryPressed()
	{
		Instance.summaryScreen.Visible = false;

		Spawn.Instance.SpawnPlayer();
	}

	private void OnBtnContinuePressed()
	{
		// Unload all scenes
		foreach (var c in GetTree().Root.GetChildren())
		{
			c.QueueFree();
		}
		
		// Load main menu scene from resources
		var sceneFile = ResourceLoader.Load<PackedScene>("res://Scenes/MainMenu.tscn");
		var scene = sceneFile.Instantiate();
		
		// Add main menu to the node tree
		GetTree().Root.AddChild(scene);
	}
}
