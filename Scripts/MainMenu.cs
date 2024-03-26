using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MainMenu : Control
{
	[Export] private PackedScene[] levels;

	[Export] private Control titlePanel;

	[Export] private Control levelSelectPanel;
	[Export] private Control levelButtonGroup;
	[Export] private PackedScene levelButtonPrefab;
	[Export] private PackedScene hudPrefab;

	[Export] private Control optionsPanel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < levels.Length; i++)
		{
			// Instantiate button
			var btn = levelButtonPrefab.Instantiate();

			// Load scene on button click
			int loadIndex = i;
			(btn as Button).Pressed += () => { LoadScene(loadIndex); };

			// Change text on nested label on button
			btn.GetNode<Label>("Label").Text = levels[i].ResourcePath.Split('/').Last();

			// Add button to the button group
			levelButtonGroup.AddChild(btn);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void LoadScene(int sceneIndex)
	{
		if (sceneIndex > (levels.Length - 1))
		{
			GD.PushWarning("The scene index was out of bounds in the levels-array.");
			return;
		}

		// Remove the previous scene
		GD.Print("Attempting to remove node: " + this.Name);
		QueueFree();

		// Request load
		ResourceLoader.LoadThreadedRequest(levels[sceneIndex].ResourcePath);
		// Get file from load request (halts application until fully loaded)
		PackedScene loadedScene = (PackedScene)ResourceLoader.LoadThreadedGet(levels[sceneIndex].ResourcePath);

		// Instantiate the loaded scene
		var scene = loadedScene.Instantiate();
		GetNode("/root").AddChild(scene);

		// Set loaded scene as the current scene (???)
		GetTree().CurrentScene = scene;

		// Add HUD to the game (the root node)
		// ...if it doesn't exist
		if (!IsInstanceValid(HUD.Instance))
		{
			var hud = hudPrefab.Instantiate();
			GetTree().Root.AddChild(hud);
		}

		GD.Print($"Scene {sceneIndex} loaded!");
	}

	private void OnBtnPlayPressed()
	{
		titlePanel.Visible = false;
		levelSelectPanel.Visible = true;
	}

	private void OnBtnOptionsPressed()
	{
		optionsPanel.Visible = true;
		titlePanel.Visible = false;
		levelSelectPanel.Visible = false;
	}

	private void OnBtnExitPressed()
	{
		GetTree().Quit();
	}

	private void OnBtnBackPressed()
	{
		optionsPanel.Visible = false;
		titlePanel.Visible = true;
		levelSelectPanel.Visible = false;
	}
}
