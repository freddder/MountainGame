using Godot;
using System;

public partial class PlayerGrounded : State
{
	private bool isRunning = false;

	private Player player;
	private Node3D cameraTarget;
	private MeshInstance3D debugSphere;

	private MovementSettings ms { get { return player.movementSettings; } }

	public override void _Ready()
	{
		player = GetNode<Player>("../..");
		cameraTarget = GetNode<Node3D>("../../CameraTarget");
		debugSphere = GetNode<MeshInstance3D>("../../Mesh/DebugSphere");
	}


	public override void Enter()
	{
		//Vector3 lookPos = player.GlobalPosition + player.Velocity;
		//lookPos.Y = player.GlobalPosition.Y;
		//mesh.LookAt(lookPos);
		//collisionShape.LookAt(lookPos);
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

		Vector2 inputDir = player.GetMoveInputDir();
		isRunning = player.isExhausted ? false : Input.IsActionPressed("run") && inputDir != Vector2.Zero;
		Vector3 targetHorizontalVelocity = new Vector3(inputDir.X, 0f, inputDir.Y).Rotated(Vector3.Up, cameraTarget.Rotation.Y);
		targetHorizontalVelocity *= isRunning ? ms.maxRunSpeed : ms.maxWalkSpeed;

		if (inputDir != Vector2.Zero)
		{
			float targetRotation = Mathf.Atan2(-targetHorizontalVelocity.X, -targetHorizontalVelocity.Z);
			float angleDiff = Mathf.AngleDifference(player.Rotation.Y, targetRotation);
			float step = isRunning ? ms.runTurnSpeed * (float)delta : ms.walkTurnSpeed * (float)delta;
			Vector3 rot = player.Rotation;
			rot.Y += Mathf.Clamp(angleDiff, -step, step);
			rot.Y = rot.Y % float.Tau;
			player.Rotation = rot;
		}

		// Horizontal velocity
		float currSpeed = new Vector3(player.Velocity.X, 0f, player.Velocity.Z).Length();
		Vector3 horizontalVelocity = -player.GlobalBasis.Z * Mathf.Lerp(currSpeed, targetHorizontalVelocity.Length(), ms.walkAcceleration * (float)delta);

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
			player.AddStaminaAmount(-ms.runStaminaReductionRate * (float)delta);
		}
		else
		{
			player.AddStaminaAmount(ms.staminaRecoveryRate * (float)delta);
		}
	}

	public override void Update(double delta)
	{
		
	}
}
