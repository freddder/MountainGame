using Godot;
using System;

public partial class PlayerClimb : State
{
	[Export]
	private float climbSpeed = 3f;
	[Export]
	private float jumpForce = 10f;
	[Export]
	private float climbStaminaReductionRate = 16f;

	private bool isJumping = false;

	private Player player;
	private Node3D mesh;
	private CollisionShape3D collisionShape;
	private Node3D debugSphere;

	public override void _Ready()
	{
		player = GetNode<Player>("../..");
		mesh = GetNode<Node3D>("../../Mesh");
		collisionShape = GetNode<CollisionShape3D>("../../CollisionShape3D");
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
		mesh.Rotation = new Vector3(0f, mesh.Rotation.Y, mesh.Rotation.Z);
		collisionShape.GlobalRotation = new Vector3(0f, collisionShape.GlobalRotation.Y, collisionShape.GlobalRotation.Z);
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
		Vector2 inputDir = player.GetInputDir();
		bool wantsToJump = Input.IsActionJustPressed("jump");
		if (wantsToJump && !isJumping)
		{
			isJumping = true;
			Vector2 jumpHorizontalDir = new Vector2(normal.X, normal.Z).Normalized();
			Vector3 jumpDir = new Vector3(jumpHorizontalDir.X, 1f, jumpHorizontalDir.Y) * jumpForce;
			player.Velocity = jumpDir;
			mesh.LookAt(player.GlobalPosition + normal);
			EmitSignalChangeState("airborne");
			return;
		}

		if (inputDir != Vector2.Zero)
		{
			Vector3 wallRight = Vector3.Up.Cross(normal).Normalized(); // Right direction relative to the wall
			Vector3 wallUp = normal.Cross(wallRight).Normalized(); // Up direction along the wall
			Vector3 climbDir = wallRight * inputDir.X + wallUp * -inputDir.Y;
			player.Velocity = climbDir * climbSpeed;
			player.Velocity += -normal.Normalized(); // Slightly push player towards the wall

			mesh.LookAt(player.GlobalPosition - normal);
			collisionShape.LookAt(player.GlobalPosition - normal);

			player.AddStaminaAmount(-climbStaminaReductionRate * (float)delta);
		}
		else
		{
			player.Velocity = Vector3.Zero;
		}

		// debug_sphere.global_position = body.global_position + normal.normalized() * 3.0
	}
}
