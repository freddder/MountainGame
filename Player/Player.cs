using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : CharacterBody3D
{
	[Export]
	public MovementSettings movementSettings { get; private set; }

	[ExportGroup("Camera")]
	[Export]
	private float cameraMouseSensitivity = 0.003f;
	[Export]
	private float cameraControllerSensitivity = 0.035f;

	[ExportGroup("Stamina")]
	[Export]
	private float maxStamina = 100f;
	private float stamina;
	public bool isExhausted { get; private set; } = false;
	[Export]
	private float wheelVisibleTimer = 2f;

	private StateMachine stateMachine;
	private Node3D cameraTarget;
	private Vector3 cameraTargetStartingPos;
	private Node3D topChecksParent;
	private Node3D botChecksParent;
	private TextureProgressBar staminaWheel;
	private List<RayCast3D> wallChecks;

	public Vector2 MoveInputDir 
	{ 
		get { return Input.GetVector("move_left", "move_right", "move_forward", "move_back").LimitLength(1f); } 
	}

	public Vector3 GlobalMoveInputDir 
	{ 
		get 
		{
			Vector2 inputDir = MoveInputDir;
			return new Vector3(inputDir.X, 0f, inputDir.Y).Rotated(Vector3.Up, cameraTarget.Rotation.Y); 
		} 
	}

	public override void _Ready()
	{
		stateMachine = GetNode<StateMachine>("StateMachine");
		cameraTarget = GetNode<Node3D>("CameraTarget");
		topChecksParent = GetNode<Node3D>("Mesh/UpperChecks");
		botChecksParent = GetNode<Node3D>("Mesh/BottomChecks");
		staminaWheel = GetNode<TextureProgressBar>("PlayerUi/StaminaWheel");
		wallChecks = new List<RayCast3D>();

		cameraTargetStartingPos = cameraTarget.Position;
		stamina = maxStamina;

		foreach (Node check in topChecksParent.GetChildren())
		{
			if (check is RayCast3D raycast)
			{
				wallChecks.Add(raycast);
			}
		}
		foreach (Node check in botChecksParent.GetChildren())
		{
			if (check is RayCast3D raycast)
			{
				wallChecks.Add(raycast);
			}
		}
	}
	
	public override void _Process(double delta)
	{
		// Camera movement
		cameraTarget.Position = Position + cameraTargetStartingPos;
		Vector2 cameraStick = Input.GetVector("camera_left", "camera_right", "camera_forward", "camera_back");
		float x = cameraTarget.Rotation.X - cameraStick.Y * cameraControllerSensitivity;
		float y = cameraTarget.Rotation.Y - cameraStick.X * cameraControllerSensitivity;
		x = Mathf.Clamp(x, -Mathf.DegToRad(85f), Mathf.DegToRad(65f));
		cameraTarget.Rotation = new Vector3(x, y, cameraTarget.Rotation.Z);

		// Stamina visibility
		if (staminaWheel.Value == 100)
		{
			if  (staminaWheel.Visible)
			{
				wheelVisibleTimer -= (float)delta;
				if (wheelVisibleTimer < 0f)
				{
					staminaWheel.Visible = false;
					wheelVisibleTimer = 2f;
				}
			}
		}
		else
		{
			staminaWheel.Visible = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	public override void _Input(InputEvent _event)
	{
		if (_event is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			Vector3 rot = cameraTarget.Rotation;
			rot.X -= motion.Relative.Y * cameraMouseSensitivity;
			rot.X = Mathf.Clamp(rot.X, -Mathf.DegToRad(85), Mathf.DegToRad(65));
			rot.Y += -motion.Relative.X * cameraMouseSensitivity;
			cameraTarget.Rotation = rot;
		}
	}

	public Vector3 GetAverageWallCheckNormal()
	{
		int checkCount = 0;
		Vector3 normalSum = Vector3.Zero;
		foreach (RayCast3D check in wallChecks)
		{
			if (!check.IsColliding())
				continue;
			
			normalSum += check.GetCollisionNormal();
			checkCount++;
		}

		if (checkCount == 0)
			return Vector3.Zero;
		return normalSum / (float)checkCount;
	}

	public KinematicCollision3D GetBestWallCollision()
	{
		float highestDot = 0f;
		KinematicCollision3D bestColliion = null;
		Vector3 targetHorizontal = cameraTarget.Transform.Basis.Z * MoveInputDir.Y + cameraTarget.Transform.Basis.X * MoveInputDir.X;
		targetHorizontal.Y = 0f;

		// TODO: make magic numbers exposed variables (0.7 and 46)
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision3D collision = GetSlideCollision(i);
			if (collision.GetAngle() < Mathf.DegToRad(46f))
				continue;
			
			float dot = targetHorizontal.Normalized().Dot(-collision.GetNormal().Normalized());
			if (dot > 0.7f && dot > highestDot)
			{
				highestDot = dot;
				bestColliion = collision;
			}
		}
		return bestColliion;
	}

	public void AddStaminaAmount(float amount)
	{
		stamina += amount;
		if (amount < 0f && stamina < 0f)
		{
			isExhausted = true;
			staminaWheel.Modulate = new Color("c90000");
		}
		else if (amount > 0f && stamina > maxStamina)
		{
			isExhausted = false;
			staminaWheel.Modulate = new Color("00d000");
		}

		stamina = Mathf.Clamp(stamina, 0f, maxStamina);
		staminaWheel.Value = stamina;
	}

	public void ChangeState(string newState)
	{
		stateMachine.ChangeState(newState);
	}

	public State GetCurrentState()
	{
		return stateMachine.currState;
	}
}
