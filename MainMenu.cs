using Godot;
using System;

public partial class MainMenu : Node3D
{
	private PackedScene mainLevel;
	private PackedScene testLevel;
	private ColorRect controlsPanel;
	private VBoxContainer levelButtons;
	private Button mainLevelButton;
	private Button controlsBackButton;

	public override void _Ready()
	{
		mainLevel = ResourceLoader.Load<PackedScene>("res://main_level.tscn");
		testLevel = ResourceLoader.Load<PackedScene>("res://terrain_level.tscn");

		mainLevelButton = GetNode<Button>("Control/VBoxContainer/MainLevelButton");
		mainLevelButton.Pressed += OnMainLevelPressed;
		mainLevelButton.GrabFocus();

		Button testLevelButton = GetNode<Button>("Control/VBoxContainer/TestLevelButton");
		testLevelButton.Pressed += OnTestLevelPressed;
		testLevelButton.GrabFocus();

		controlsPanel = GetNode<ColorRect>("Control/ColorRect");
		controlsPanel.Visible = false;
		levelButtons = GetNode<VBoxContainer>("Control/VBoxContainer");

		controlsBackButton = GetNode<Button>("Control/ColorRect/BackButton");
		controlsBackButton.Pressed += OnBackButtonPressed;

		Button controlsButton = GetNode<Button>("Control/VBoxContainer/ControlsButton");
		controlsButton.Pressed += OnControlsButtonPressed;
	}

	private void OnMainLevelPressed()
	{
		GetTree().ChangeSceneToPacked(mainLevel);
	}

	private void OnTestLevelPressed()
	{
		GetTree().ChangeSceneToPacked(testLevel);
	}

	private void OnControlsButtonPressed()
	{
		controlsPanel.Visible = true;
		levelButtons.Visible = false;
		controlsBackButton.GrabFocus();
	}

	private void OnBackButtonPressed()
	{
		controlsPanel.Visible = false;
		levelButtons.Visible = true;
		mainLevelButton.GrabFocus();
	}
}
