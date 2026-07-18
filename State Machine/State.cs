using Godot;
using System;

public abstract partial class State
{
	protected StateMachine stateMachine;

	public State(StateMachine sm)
	{
		stateMachine = sm;
	}

	public void ChangeState(int newState)
	{
		stateMachine.ChangeState(newState);
	}

	public abstract void Enter();
	public abstract void Exit();
	public abstract void Update(double delta);
	public abstract void PhysicsUpdate(double delta);
}
