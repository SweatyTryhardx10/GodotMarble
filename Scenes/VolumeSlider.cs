using Godot;
using System;

public partial class VolumeSlider : HSlider
{
	[Export] private float maxDecibel = -5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OnValueChanged((float)Value);
	}

	private void OnValueChanged(float value)
	{
		float newDb = (float)Mathf.Lerp(-60, maxDecibel, value / MaxValue);
		AudioServer.SetBusVolumeDb(0, newDb);
	}
}
