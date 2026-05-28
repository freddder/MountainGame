using Godot;
using System;

public partial class PlayerClimb : State
{
	private bool isJumping = false;

	private Player player;
	private Node3D debugSphere;

	private MovementSettings ms { get { return player.movementSettings; } }

	public override void _Ready()
	{
		player = GetNode<Player>("../..");
		debugSphere = GetNode<Node3D>("../../Mesh/DebugSphere");
	}

	public override void Enter()
	{
		player.Velocity = Vector3.Zero;
	}

	public override void Exit()
	{
		isJumping = false;
		debugSphere.GlobalPosition = player.GlobalPosition;
		player.Rotation = new Vector3(0f, player.Rotation.Y, player.Rotation.Z);
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

		Vector3 normal = player.GetAverageWallCheckNormal();
		float dot = normal.Dot(Vector3.Up);
		if (normal == Vector3.Zero || player.isExhausted || Mathf.Abs(dot) > 0.75f)
		{
			EmitSignalChangeState("airborne");
			return;
		}

		// Movement
		Vector2 inputDir = player.GetMoveInputDir();
		bool wantsToJump = Input.IsActionJustPressed("jump");
		if (wantsToJump && !isJumping)
		{
			isJumping = true;
			Vector2 jumpHorizontalDir = new Vector2(normal.X, normal.Z).Normalized();
			Vector3 jumpDir = new Vector3(jumpHorizontalDir.X, 1f, jumpHorizontalDir.Y) * ms.jumpForce;
			player.Velocity = jumpDir;
			player.LookAt(player.GlobalPosition + normal);
			EmitSignalChangeState("airborne");
			return;
		}

		if (inputDir != Vector2.Zero)
		{
			Vector3 wallRight = Vector3.Up.Cross(normal).Normalized(); // Right direction relative to the wall
			Vector3 wallUp = normal.Cross(wallRight).Normalized(); // Up direction along the wall
			Vector3 climbDir = wallRight * inputDir.X + wallUp * -inputDir.Y;
			player.Velocity = climbDir * ms.climbSpeed;
			player.Velocity += -normal.Normalized(); // Slightly push player towards the wall

			Quaternion targetQuaternion = new Quaternion(Basis.LookingAt(-normal));
			player.Quaternion = player.Quaternion.Slerp(targetQuaternion, (float)delta * 20f);

			player.AddStaminaAmount(-ms.climbStaminaReductionRate * (float)delta);
		}
		else
		{
			player.Velocity = Vector3.Zero;
		}

		// debug_sphere.global_position = body.global_position + normal.normalized() * 3.0
	}
}
