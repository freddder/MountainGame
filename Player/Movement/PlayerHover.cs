using Godot;
using System;

public partial class PlayerHover : State
{
	private Player player;

	private float enterVelocity;
	private float timer = 0f;

	private MovementSettings ms { get { return player.movementSettings; } }

	public PlayerHover(StateMachine sm, Player _player) : base(sm)
	{
		player = _player;
	}

	public override void Enter()
	{
		enterVelocity = player.Velocity.Length();
		timer = 0f;
	}

	public override void Exit()
	{
		
	}

	public override void PhysicsUpdate(double delta)
	{
		timer += (float)delta;
		float t = Mathf.Clamp(timer / ms.hoverStopTime, 0f, 1f);
		float newVelocity = Mathf.Lerp(enterVelocity, 0f, t);
		player.Velocity = player.Velocity.Normalized() * newVelocity;
	}

	public override void Update(double delta)
	{
		
	}
}