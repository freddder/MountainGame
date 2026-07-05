using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class ExpandingBlockPreview : Power
{
	private PackedScene expandingBlockScene = GD.Load<PackedScene>("res://Gameplay/Expanding Block/expanding_block.tscn");

	[Export]
	private float sideSize = 3f;
	[Export]
	private float raycastStartHeight = 0.5f;
	[Export]
	private float raycastLength = 1f;
	[Export(PropertyHint.Layers3DPhysics)]
	private uint raycastCollisionMask;
	[Export]
	private float flatSurfaceThreshold = 0.1f;

	private RayCast3D[] terrainRaycasts;

	public override void _Ready()
	{
		terrainRaycasts = new RayCast3D[4];
		for (int i = 0; i < 4; i++)
		{
			terrainRaycasts[i] = new RayCast3D();
			terrainRaycasts[i].Position = Vector3.Up * raycastStartHeight;
			terrainRaycasts[i].CollisionMask = raycastCollisionMask;

			float x = i < 2 ? sideSize * 0.5f : sideSize * -0.5f;
			float z = i % 2 == 0 ? sideSize * 0.5f : sideSize * -0.5f;
			terrainRaycasts[i].Position = new Vector3(terrainRaycasts[i].Position.X + x, terrainRaycasts[i].Position.Y, terrainRaycasts[i].Position.Z + z);

			terrainRaycasts[i].TargetPosition = Vector3.Down * raycastLength;
			AddChild(terrainRaycasts[i]);
		}
	}

	public override void _Process(double delta)
	{
		if (!isInUse) return;

		// Move block preview to middle of screen
		PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
		Camera3D cam = GetViewport().GetCamera3D();
		Vector2 mousePos = GetViewport().GetMousePosition();

		Vector3 origin = cam.ProjectRayOrigin(mousePos);
		Vector3 end = origin + cam.ProjectRayNormal(mousePos) * 1000f;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, end, raycastCollisionMask);
		Dictionary result = spaceState.IntersectRay(query);
		if (result.Any())
		{
			GlobalPosition = (Vector3)result["position"];
			Vector3 normal = (Vector3)result["normal"];
			
			Vector3 forward = Vector3.Forward;
			if (Mathf.Abs(normal.Dot(forward)) > 0.99f)
			{
    			forward = Vector3.Right; // fallback if too parallel
			}

			// Build orthonormal basis
			Vector3 right = forward.Cross(normal).Normalized();
			forward = normal.Cross(right).Normalized();
			Basis basis = new Basis(right, normal, forward);
			Transform3D transform = Transform;
			transform.Basis = basis;
			Transform = transform;
		}
	}

	public override bool UsePower()
	{
		if (isInUse)
		{
			if (CanSpawnBlock())
			{
				// Spawn the block
				Node3D n = expandingBlockScene.Instantiate() as Node3D;
				n.Transform = Transform;
				GetTree().Root.AddChild(n);

				ResetPower();

				return true;
			}
		}
		else
		{
			Visible = true;
			isInUse = true;
		}

		return false;
	}

	private bool CanSpawnBlock()
	{
		for (int i = 0; i < 4; i++)
		{
			if (!terrainRaycasts[i].IsColliding())
				return false;
			
			Vector3 normal = terrainRaycasts[i].GetCollisionNormal();
			float dot = normal.Dot(-Transform.Basis.Y);
			if (-dot < flatSurfaceThreshold)
				return false;
		}

		return true;
	}

	public override void ResetPower()
	{
		Visible = false;
		isInUse = false;
	}
}
