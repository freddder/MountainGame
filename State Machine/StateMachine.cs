using Godot;
using System.Collections.Generic;

public class StateMachine
{
	public State currState { get; private set; } = null;
	private List<State> states;

	public StateMachine()
	{
		states = new List<State>();
	}

	public void AddState(State newState, bool isDefaultState = false)
	{
		states.Add(newState);

		if (isDefaultState)
			ChangeState(states.Count - 1);			
	}

	public void ChangeState(int newStateIndex)
	{
		if (newStateIndex < 0 || newStateIndex >= states.Count)
			return;
	
		if (currState != null)
		{
			currState.Exit();
		}
		currState = states[newStateIndex];
		currState.Enter();
	}

	public void Update(double delta)
	{
		if (currState != null)
		{
			currState.Update(delta);
		}
	}

	public void PhysicsUpdte(double delta)
	{
		if (currState != null)
		{
			currState.PhysicsUpdate(delta);
		}
	}
}
