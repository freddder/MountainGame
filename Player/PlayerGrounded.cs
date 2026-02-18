using Godot;
using System;

public partial class PlayerGrounded : State
{
	[Export]
	private float staminaRecoveryRate = 25f;

	[ExportGroup("Walking")]
	[Export]
	private float maxWalkSpeed = 5f;
	[Export]
	private float walkTurnSpeed = 7f;
	[Export]
	private float walkAcceleration = 20f;

	[ExportGroup("Running")]
	[Export]
	private float maxRunSpeed = 10f;
	[Export]
	private float runTurnSpeed = 4f;
	[Export]
	private float runStaminaReductionRate = 20f;
	private bool isRunning = false;

	private Player player; // = $"../.."
	private Node3D cameraTarget; //$"../../CameraTarget"
	private Node3D mesh; //$"../../Mesh"
	private CollisionShape3D collisionShape; // $"../../CollisionShape3D"
	private MeshInstance3D debugSphere; // $"../../Mesh/DebugSphere"

	public override void _Ready()
	{
		player = GetNode<Player>("../..");
		cameraTarget = GetNode<Node3D>("../../CameraTarget");
		mesh = GetNode<Node3D>("../../Mesh");
		collisionShape = GetNode<CollisionShape3D>("../../CollisionShape3D");
		debugSphere = GetNode<MeshInstance3D>("../../Mesh/DebugSphere");
	}


	public override void Enter()
	{
		Vector3 lookPos = player.GlobalPosition + player.Velocity;
		lookPos.Y = player.GlobalPosition.Y;
		mesh.LookAt(lookPos);
		collisionShape.LookAt(lookPos);
	}

	public override void Exit()
	{
		
	}

	public override void PhysicsUpdate(double delta)
	{
		if (!player.IsOnFloor())
		{
			EmitSignalChangeState("airborne");
			return;
		}

		Vector2 inputDir = player.GetInputDir();
		isRunning = player.isExhausted ? false : Input.IsActionPressed("run") && inputDir != Vector2.Zero;
		Vector3 targetHorizontalVelocity = new Vector3(inputDir.X, 0f, inputDir.Y).Rotated(Vector3.Up, cameraTarget.Rotation.Y);
		targetHorizontalVelocity *= isRunning ? maxRunSpeed : maxWalkSpeed;

		if (inputDir != Vector2.Zero)
		{
			float targetRotation = Mathf.Atan2(-targetHorizontalVelocity.X, -targetHorizontalVelocity.Z);
			float angleDiff = Mathf.AngleDifference(mesh.Rotation.Y, targetRotation);
			float step = isRunning ? runTurnSpeed * (float)delta : walkTurnSpeed * (float)delta;
			Vector3 rot = mesh.Rotation;
			rot.Y += Mathf.Clamp(angleDiff, -step, step);
			rot.Y = rot.Y % float.Tau;
			mesh.Rotation = rot;
		}

		// Horizontal velocity
		float currSpeed = new Vector3(player.Velocity.X, 0f, player.Velocity.Z).Length();
		Vector3 horizontalVelocity = -mesh.GlobalBasis.Z * Mathf.Lerp(currSpeed, targetHorizontalVelocity.Length(), walkAcceleration * (float)delta);

		// Vertical velocity
		KinematicCollision3D collision = player.GetBestWallCollision();
		float verticalVelocity = 0f;
		bool wantsToJump = Input.IsActionJustPressed("jump");
		if (wantsToJump || 
			(collision != null && collision.GetNormal().Dot(targetHorizontalVelocity) < 0f))
		{
			verticalVelocity = 10f;
		}

		player.Velocity = new Vector3(horizontalVelocity.X, verticalVelocity, horizontalVelocity.Z);

		if (isRunning)
		{
			player.AddStaminaAmount(-runStaminaReductionRate * (float)delta);
		}
		else
		{
			player.AddStaminaAmount(staminaRecoveryRate * (float)delta);
		}
	}

	public override void Update(double delta)
	{
		
	}
}
