using Godot;
using System;

public partial class PlayerAirborne : State
{
	private bool isGliding = false;
	private float glideTurnSpeed = 0f;
	private bool isGlideActionBuffered = false;
	private float glideActionBufferTimer = 0.3f;

	private Player player;
	private Node3D cameraTarget;
	private MeshInstance3D wings;

	private MovementSettings ms { get { return player.movementSettings; } }

	public override void _Ready()
	{
		player = GetNode<Player>("../..");
		cameraTarget = GetNode<Node3D>("../../CameraTarget");
		wings = GetNode<MeshInstance3D>("../../Mesh/Wings");
	}

	public override void Enter()
	{
	}

	public override void Exit()
	{
		isGliding = false;
		isGlideActionBuffered = false;
		wings.Visible = false;
		ResetGlideActionBuffer();
	}

	public override void Update(double delta)
	{
	}

	public override void PhysicsUpdate(double delta)
	{
		if (player.IsOnFloor() && player.Velocity.Y <= 0f)
		{
			EmitSignalChangeState("grounded");
			return;
		}

		KinematicCollision3D collision = player.GetBestWallCollision();
		if (collision != null && player.Velocity.Y < 0f && !player.isExhausted)
		{
			EmitSignalChangeState("climb");
			return;
		}

		bool wantsToJump = Input.IsActionJustPressed("jump");
		if (!player.isExhausted && player.Velocity.Y < 0f && (wantsToJump || isGlideActionBuffered))
		{
			ToggleGliding();
		}
		else if (wantsToJump && !isGlideActionBuffered)
		{
			isGlideActionBuffered = true;
		}

		if (isGlideActionBuffered)
		{
			glideActionBufferTimer -= (float)delta;
			if (glideActionBufferTimer <= 0f)
			{
				ResetGlideActionBuffer();
			}
		}

		Vector3 vel = player.Velocity;
		if (isGliding)
		{
			Vector3 targetHorizontalVelocity = Vector3.Zero;
			Vector3 currHorizontalVelocity = player.Velocity;
			currHorizontalVelocity.Y = 0f;
			if (player.MoveInputDir != Vector2.Zero)
			{
				targetHorizontalVelocity = player.GlobalMoveInputDir * ms.glideHorizontalSpeed;

				float lookAngle = Mathf.Atan2(-targetHorizontalVelocity.X, -targetHorizontalVelocity.Z);
				float rotY = Mathf.LerpAngle(player.Rotation.Y, lookAngle, ms.glideTurnAcceleration * (float)delta);
				player.Rotation = new Vector3(player.Rotation.X, rotY, player.Rotation.Z);
			}

			vel = currHorizontalVelocity.Lerp(targetHorizontalVelocity, ms.glideHorizontalAcceleration * (float)delta);
			vel.Y = player.Velocity.Y + (-ms.fallAcceleration * (float)delta);
			vel.Y = Mathf.Clamp(vel.Y, -ms.glideFallSpeed, ms.glideFallSpeed);

			player.AddStaminaAmount(-ms.glideStaminaReductionRate * (float)delta);
			if (player.isExhausted)
			{
				ToggleGliding();
			}
		}
		else
		{
			vel.Y = player.Velocity.Y + (-ms.fallAcceleration * (float)delta);
			vel.Y = Mathf.Clamp(vel.Y, -ms.maxFallSpeed, ms.maxFallSpeed);
		}
		player.Velocity = vel;
	}

	private void ResetGlideActionBuffer()
	{
		isGlideActionBuffered = false;
		glideActionBufferTimer = 0.3f;
	}

	private void ToggleGliding()
	{
		isGliding = !isGliding;
		wings.Visible = !wings.Visible;
		ResetGlideActionBuffer();
	}
}
