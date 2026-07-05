using Godot;
using System;

[GlobalClass]
public partial class MovementSettings : Resource
{
	[ExportGroup("Grounded")]
	[Export]
	public float jumpSpeed = 10f;
	[Export]
	public float staminaRecoveryRate { get; private set; } = 25f ;
	[ExportGroup("Grounded/Walking")]
	[Export]
	public float walkMaxSpeed { get; private set; } = 5f;
	[Export]
	public float walkTurnSpeed { get; private set; } = 7f;
	[Export]
	public float walkAcceleration { get; private set; } = 20f;
	[ExportGroup("Grounded/Running")]
	[Export]
	public float maxRunSpeed { get; private set; } = 10f;
	[Export]
	public float runTurnSpeed { get; private set; } = 4f;
	[Export]
	public float runStaminaReductionRate { get; private set; } = 20f;

	[ExportGroup("Airborne")]
	[Export]
	public float fallAcceleration { get; private set; } = 45f;
	[Export]
	public float maxFallSpeed { get; private set; } = 75f;
	[ExportGroup("Airborne/Glide")]
	[Export]
	public float glideFallSpeed { get; private set; } = 4f;
	[Export]
	public float glideHorizontalSpeed { get; private set; } = 8f;
	[Export]
	public float glideHorizontalAcceleration { get; private set; } = 8f;
	[Export]
	public float glideTurnAcceleration { get; private set; } = 1f;
	[Export]
	public float glideStaminaReductionRate { get; private set; } = 5f;

	[ExportGroup("Climb")]
	[Export]
	public float climbSpeed { get; private set; } = 3f;
	[Export]
	public float climbJumpSpeed { get; private set; } = 10f;
	[Export]
	public float climbStaminaReductionRate { get; private set; } = 16f;

	public MovementSettings() {}
}
