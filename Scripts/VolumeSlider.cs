using Godot;
using System;
using System.Collections.Generic;

public partial class VolumeSlider : HSlider
{
	public static List<VolumeSlider> All { get; private set; } = new List<VolumeSlider>();

	[Export] private string busName = "Master";
	[Export(PropertyHint.Range, "-60, 0")] private float maxDecibel = -5f;

	/// <summary>The slider value normalized to the range of the slider.</summary>
	public float NormalizedValue { get => (float)(Value / MaxValue); private set { Value = Mathf.Lerp(MinValue, MaxValue, value); } }

	public override void _EnterTree()
	{
		// Register slider in the global list
		if (!All.Contains(this))
			All.Add(this);
	}
    public override void _ExitTree()
    {
		// Unregister slider in the global list
        if (All.Contains(this))
			All.Remove(this);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		OnValueChanged((float)Value);

		this.DragEnded += OnDragEnded;
		
		// Load the config file and set this slider's value (if it is saved)
		var config = SaveLoadUtil.LoadConfig();
		if (config.ContainsKey(Name))
			NormalizedValue = (float)config[Name];
	}

	private void OnValueChanged(float value)
	{
		float newDb = (float)Mathf.Lerp(-60, maxDecibel, value / MaxValue);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(busName), newDb);
	}

	private void OnDragEnded(bool valueChanged)
	{
		if (!valueChanged)
			return;

		// Save settings
		SaveLoadUtil.SaveConfig();
	}
}
