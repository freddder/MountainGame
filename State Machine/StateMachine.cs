using Godot;
using System.Collections.Generic;

public partial class StateMachine : Node
{
	[Export]
	private State initialState = null;
	private State currState = null;
	private Dictionary<string, State> states;

	public override void _Ready()
	{
		states = new Dictionary<string, State>();
		foreach (Node child in GetChildren())
		{
			if (child is State state)
			{
				states.Add(child.Name.ToString().ToLower(), state);
				state.ChangeState += ChangeState;
			}
		}

		if (initialState != null)
		{
			currState = initialState;
			initialState.Enter();
		}
	}

	public void ChangeState(string newStateName)
	{
		if (!states.ContainsKey(newStateName))
			return;

		if (currState != null)
		{
			currState.Exit();
		}
		currState = states[newStateName];
		currState.Enter();
	}

	public override void _Process(double delta)
	{
		if (currState != null)
		{
			currState.Update(delta);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currState != null)
		{
			currState.PhysicsUpdate(delta);
		}
	}
}
