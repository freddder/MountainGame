using Godot;
using System;

public partial class AirCurrent : Area3D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach (Node3D body in GetOverlappingBodies())
		{
			if (body is Player player && player.IsGliding())
			{
				player.Velocity += Vector3.Up * 10f;
			}
		}
	}
}
