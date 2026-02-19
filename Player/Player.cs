using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody3D
{
	[ExportGroup("Camera")]
	[Export]
	private float cameraMouseSensitivity = 0.003f;
	[Export]
	private float cameraControllerSensitivity = 0.035f;

	[ExportGroup("Stamina")]
	[Export]
	private float maxStamina = 100f;
	[Export]
	private float staminaRecoveryRate = 25f;
	private float stamina;
	public bool isExhausted = false;
	private float wheelVisibleTimer = 2f;

	private Node3D cameraTarget;
	private Node3D topChecksParent;
	private Node3D botChecksParent;
	private TextureProgressBar staminaWheel;
	private PlayerMenu playerMenu;
	private List<RayCast3D> wallChecks;

	public override void _Ready()
	{
		cameraTarget = GetNode<Node3D>("CameraTarget");
		topChecksParent = GetNode<Node3D>("Mesh/UpperChecks");
		botChecksParent = GetNode<Node3D>("Mesh/BottomChecks");
		staminaWheel = GetNode<TextureProgressBar>("PlayerUi/StaminaWheel");
		//playerMenu = GetNode<PlayerMenu>("PlayerUi");
		wallChecks = new List<RayCast3D>();

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
		Vector2 cameraStick = Input.GetVector("camera_left", "camera_right", "camera_forward", "camera_back");
		float x = cameraTarget.Rotation.X - cameraStick.Y * cameraControllerSensitivity;
		float y = cameraTarget.Rotation.Y - cameraStick.X * cameraControllerSensitivity;
		x = Mathf.Clamp(x, -Mathf.DegToRad(85), Mathf.DegToRad(65));
		cameraTarget.Rotation = new Vector3(x, y, cameraTarget.Rotation.Z);

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

	public Vector2 GetInputDir()
	{
		return Input.GetVector("move_left", "move_right", "move_forward", "move_back").LimitLength(1f);
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
		Vector2 inputDir = GetInputDir();
		Vector3 targetHorizontal = cameraTarget.Transform.Basis.Z * inputDir.Y + cameraTarget.Transform.Basis.X * inputDir.X;
		targetHorizontal.Y = 0f;

		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision3D collision = GetSlideCollision(i);
			if (collision.GetAngle() < Mathf.DegToRad(46f))
				continue;
			
			float dot = targetHorizontal.Normalized().Dot(-collision.GetNormal().Normalized());
			if (dot > 0.7f && dot > highestDot) // make 0.7 a variable in degrees
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
}
