using Godot;
using System;

public partial class ExpandingBlock : AnimatableBody3D
{
	[Export]
	private float growthRate = 20f;
	[Export]
	private float aliveTime = 5f;
	[Export]
	private float launchSpeed = 25f;
	private float timer = 0f;
	private float blockHeight;
	private bool isFullyGrown = false;

	private CollisionShape3D collider;
	private MeshInstance3D mesh;
	private Area3D launchArea;

	public override void _Ready()
	{
		collider = GetNode<CollisionShape3D>("CollisionShape3D");
		mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		launchArea = GetNode<Area3D>("CollisionShape3D/Area3D");

		blockHeight = (collider.Shape as BoxShape3D).Size.Y;
	}

	public override void _Process(double delta)
	{
		if (isFullyGrown)
		{
			timer += (float)delta;
			//ConstantLinearVelocity = timer <= 0.05f ? Transform.Basis.Y * launchSpeed : Vector3.Zero;

			if (timer >= aliveTime)
			{
				QueueFree();
			}
		}
		else
		{
			Vector3 pos = mesh.Position + mesh.Basis.Y * (float)delta * growthRate;
			mesh.Position = pos;

			isFullyGrown = pos.Length() >= blockHeight * 0.5f;
			if (isFullyGrown)
			{
				UpdateCollider();
				foreach (Node3D node in launchArea.GetOverlappingBodies())
				{
					if (node is Player player && player.GetCurrentState() is not PlayerClimb)
					{
						player.ChangeState("airborne");
						player.Velocity = Transform.Basis.Y * launchSpeed;
					}
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isFullyGrown)
			return;

		UpdateCollider();
	}

	private void UpdateCollider()
	{
		collider.Position = mesh.Position;
	}
}
