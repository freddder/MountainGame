using Godot;
using System;

public partial class PlayerGrounded : State
{
	private bool isRunning = false;

	private Player player;

	private MovementSettings ms { get { return player.movementSettings; } }

	public PlayerGrounded(StateMachine sm, Player _player) : base(sm)
	{
		player = _player;
	}

	public override void Enter()
	{
	}

	public override void Exit()
	{		
	}

	public override void PhysicsUpdate(double delta)
	{
		if (!player.IsOnFloor())
		{
			ChangeState((int)Player.MovementStates.AIRBORNE);
			return;
		}

		Vector2 inputDir = player.MoveInputDir;
		isRunning = player.isExhausted ? false : Input.IsActionPressed("run") && player.MoveInputDir != Vector2.Zero;
		Vector3 targetHorizontalVelocity = player.GlobalMoveInputDir;
		targetHorizontalVelocity *= isRunning ? ms.maxRunSpeed : ms.walkMaxSpeed;

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
			verticalVelocity = ms.jumpSpeed;
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
