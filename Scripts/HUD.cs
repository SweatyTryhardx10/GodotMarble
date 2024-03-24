using Godot;
using System;

public partial class HUD : Control
{
	public static HUD Instance { get; private set; }
	
	[Export]
	private Control summaryScreen;
	[Export]
	private Timer timer;
	[Export]
	private Label timerLabel;

	[Export]
	private Label summaryCompletionTimeLabel;

    public override void _EnterTree() => Instance = this;
    public override void _ExitTree() => Instance = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
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
		if (!IsInstanceValid(Instance))
			return;
			
		Instance.summaryCompletionTimeLabel.Text = Instance.timerLabel.Text;

		Instance.summaryScreen.Visible = true;

		// Change mouse mode
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public static void SetGameTimer(float seconds)
	{
		if (!IsInstanceValid(Instance))
			return;
		
		Instance.timerLabel.Text = SecondsToTimerFormat(seconds);
	}

	private void OnBtnRetryPressed()
	{
		summaryScreen.Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;

		Spawn.SpawnPlayer();
	}

	private void OnBtnContinuePressed()
	{
		// Unload all scenes
		foreach (var c in GetTree().Root.GetChildren())
		{
			if (c.Name != "BackgroundMusic")	// TODO: This is very spaghetti italiano!
				c.QueueFree();
		}

		// Load main menu scene from resources
		var sceneFile = ResourceLoader.Load<PackedScene>("res://Scenes/MainMenu.tscn");
		var scene = sceneFile.Instantiate();

		// Add main menu to the node tree
		GetTree().Root.AddChild(scene);
		

		// Change mouse mode
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
