using Godot;
using System;

public partial class ParticleCustom : CpuParticles3D
{
	[Export] private bool playChildren = true;

	public virtual void Play()
	{
		this.Restart();

		if (playChildren)
			PlayChildrenRecursively(this);
	}

	private void PlayChildrenRecursively(Node n)
	{
		foreach (Node c in n.GetChildren())
		{
			if (c is CpuParticles3D)
			{
				(c as CpuParticles3D).Restart();
				PlayChildrenRecursively(c);
			}
		}
	}
}
