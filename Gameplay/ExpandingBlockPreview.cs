using Godot;
using System;

public partial class ExpandingBlockPreview : Node3D
{
	[Export]
	private float sideSize = 3f;
	[Export]
	private float raycastStartHeight = 0.5f;
	[Export]
	private float raycastLength = 1f;
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

			float x = i < 2 ? sideSize * 0.5f : sideSize * -0.5f;
			float z = i % 2 == 0 ? sideSize * 0.5f : sideSize * -0.5f;
			terrainRaycasts[i].Position = new Vector3(terrainRaycasts[i].Position.X + x, terrainRaycasts[i].Position.Y, terrainRaycasts[i].Position.Z + z);

			terrainRaycasts[i].TargetPosition = Vector3.Down * raycastLength;
			AddChild(terrainRaycasts[i]);
		}
	}

	public override void _Process(double delta)
	{
		bool good = true;
		for (int i = 0; i < 4; i++)
		{
			if (!terrainRaycasts[i].IsColliding())
			{
				good = false;
				break;
			}
			
			Vector3 normal = terrainRaycasts[i].GetCollisionNormal();
			float dot = normal.Dot(-Transform.Basis.Y);
			if (-dot < flatSurfaceThreshold)
			{
				good = false;
				break;
			}
		}
		GD.Print(good);
	}
}
