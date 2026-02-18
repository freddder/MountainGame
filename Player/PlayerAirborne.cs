using Godot;
using System;

public partial class PlayerAirborne : State
{
	[ExportGroup("Movement")]
	[Export]
	private float fallAcceleration = 45f;
	[Export]
	private float maxFallSpeed = 75f;
	[ExportGroup("Glide")]
	[Export]
	private float glideFallSpeed = 4f;
	[Export]
	private float glideHorizontalSpeed = 8f;
	[Export]
	private float glideHorizontalAcceleration = 8f;
	[Export]
	private float glideTurnAcceleration = 1f;
	[Export]
	private float glideStaminaReductionRate = 5f;
	private bool isGliding = false;
	private float glideTurnSpeed = 0f;
	private bool isGlideActionBuffered = false;
	private float glideActionBufferTimer = 0.3f;

	private Player player;
	private Node3D mesh;
	private Node3D cameraTarget;
	private MeshInstance3D wings;

	public override void _Ready()
	{
		player = GetNode<Player>("../..");
		mesh = GetNode<Node3D>("../../Mesh");
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
		if (player.IsOnFloor())
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
			Vector2 inputDir = player.GetInputDir();
			Vector3 targetHorizontalVelocity = Vector3.Zero;
			Vector3 currHorizontalVelocity = player.Velocity;
			currHorizontalVelocity.Y = 0f;
			if (inputDir != Vector2.Zero)
			{
				targetHorizontalVelocity = new Vector3(inputDir.X, 0f, inputDir.Y).Rotated(Vector3.Up, cameraTarget.Rotation.Y);
				targetHorizontalVelocity = targetHorizontalVelocity * glideHorizontalSpeed;

				float lookAngle = Mathf.Atan2(-targetHorizontalVelocity.X, -targetHorizontalVelocity.Z);
				float rotY = Mathf.LerpAngle(mesh.Rotation.Y, lookAngle, glideTurnAcceleration * (float)delta);
				mesh.Rotation = new Vector3(mesh.Rotation.X, rotY, mesh.Rotation.Z);
			}

			vel = currHorizontalVelocity.Lerp(targetHorizontalVelocity, glideHorizontalAcceleration * (float)delta);
			vel.Y = player.Velocity.Y + (-fallAcceleration * (float)delta);
			vel.Y = Mathf.Clamp(vel.Y, -glideFallSpeed, glideFallSpeed);

			player.AddStaminaAmount(-glideStaminaReductionRate * (float)delta);
			if (player.isExhausted)
			{
				ToggleGliding();
			}
		}
		else
		{
			vel.Y = player.Velocity.Y + (-fallAcceleration * (float)delta);
			vel.Y = Mathf.Clamp(vel.Y, -maxFallSpeed, maxFallSpeed);
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
