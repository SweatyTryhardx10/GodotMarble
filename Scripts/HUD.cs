using Godot;
using System;

public partial class HUD : Control
{
	public static bool exists;
	
	private Timer timer;
	private Label timerLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!exists)
			exists = true;
		else
			QueueFree();
		
		timer = GetNode<Timer>("Timer");
		timerLabel = GetNode<Label>("TimerLabel");
		
		timer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timerLabel.Text = SecondsToTimerFormat(timer.TimeLeft);
	}

	private string SecondsToTimerFormat(double value)
	{
		int minutes = (int)Math.Floor(value / 60);
		int seconds = (int)value % 60;
		double milliseconds = value - Math.Floor(value);
		
		return $"{minutes.ToString("0#")}:{seconds.ToString("0#")}.{milliseconds.ToString("#.#0").Replace(',', ' ').TrimStart()}";
	}
}
