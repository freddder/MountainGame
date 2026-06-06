using Godot;
using System;

[GlobalClass]
public abstract partial class Power : Node3D
{
	public bool isInUse = false;

	public abstract bool UsePower();
}
