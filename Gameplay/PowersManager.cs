using Godot;
using System;
using System.Collections.Generic;

public partial class PowersManager : Node
{
	private Power lastUsedPower = null;
	private List<Power> powers;
	private int currSelectedPowerIndex = 0;

	public override void _Ready()
	{
		powers = new List<Power>();
		foreach (Node child in GetChildren())
		{
			if (child is Power power)
			{
				powers.Add(power);
			}
		}
	}

	public override void _Process(double delta)
	{
		if (powers.Count == 0)
			return;

		if (Input.IsActionJustPressed("change_power_left") && lastUsedPower != null && !lastUsedPower.isInUse)
		{
			currSelectedPowerIndex--;
			if (currSelectedPowerIndex < 0)
				currSelectedPowerIndex = powers.Count - 1;
		}

		if (Input.IsActionJustPressed("change_power_right") && lastUsedPower != null && !lastUsedPower.isInUse)
		{
			currSelectedPowerIndex++;
			if (currSelectedPowerIndex >= powers.Count)
				currSelectedPowerIndex = 0;
		}

		if (Input.IsActionJustPressed("use_power") && currSelectedPowerIndex < powers.Count)
		{
			if (lastUsedPower == null || !lastUsedPower.isInUse)
			{
				powers[currSelectedPowerIndex].UsePower();
			}
		}
	}
}
