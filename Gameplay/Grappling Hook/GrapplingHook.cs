using Godot;
using System;

public partial class GrapplingHook : Power
{
	[Export]
	private float grappleDistance = 5f;
	[Export]
	private float grappleSpeed = 15f;
	[Export]
	private float launchSpeed = 15f;

	private Vector3 grappleStartDir = Vector3.Zero;
	private Vector3 grappleAttachPoint;
	private bool hasUsed = false;

	private Area3D area;
	private Player player;

	public override void _Ready()
	{
		area = GetNode<Area3D>("Area3D");
		player = GetNode<Player>("../..");
	}

	public override void _Process(double delta)
	{
		if (player.IsOnFloor())
		{
			hasUsed = false;
		}

		if (isInUse) 
		{
			GlobalPosition += grappleStartDir * grappleSpeed * (float)delta;
			if ((GlobalPosition - player.GlobalPosition).Length() >= grappleDistance)
			{
				ResetPower();
			}
		}
	}

	public override bool UsePower()
	{
		if (isInUse || hasUsed) return false;

		isInUse = true;
		hasUsed = true;
		player.ChangeState((int)Player.MovementState.HOVER);

		GlobalPosition = player.GlobalPosition;
		area.SetDeferred("monitoring", true);
		area.SetDeferred("monitorable", true);
		Visible = true;

		grappleStartDir = player.GlobalMoveInputDir;
		if (grappleStartDir == Vector3.Zero)
			grappleStartDir = -player.GlobalBasis.Z;
		grappleStartDir = grappleStartDir.Normalized();

		return true;
	}

	public override void ResetPower()
	{
		player.ChangeState((int)Player.MovementState.AIRBORNE);

		isInUse = false;
		GlobalPosition = player.GlobalPosition;
		area.SetDeferred("monitoring", false);
		area.SetDeferred("monitorable", false);
		Visible = false;
	}

	public void OnBodyEntered(Node3D other)
	{
		// TODO: check if body is a small object
		
		grappleAttachPoint = GlobalPosition;
		ResetPower();

		Vector3 dir = (grappleAttachPoint - player.GlobalPosition).Normalized();
		Vector3 launch = dir.Slerp(Vector3.Up, 0.5f) * launchSpeed;
		player.ChangeState((int)Player.MovementState.AIRBORNE);

		player.Velocity = launch;
		player.LookAt(player.GlobalPosition + new Vector3(launch.X, 0f, launch.Z));
	}
}
