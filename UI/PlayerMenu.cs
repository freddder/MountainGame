using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PlayerMenu : Control
{
	private TextureProgressBar staminaWheel;
	private ColorRect buttonsMenu;
	private Button resumeButton;
	private Button restartButton;
	private Button mainMenuButton;
	private PackedScene mainMenu;

	private bool isPaused = false;

	public override void _Ready()
	{
		staminaWheel = GetNode<TextureProgressBar>("StaminaWheel");
		buttonsMenu = GetNode<ColorRect>("ColorRect");
		resumeButton = GetNode<Button>("ColorRect/VBoxContainer/Resume");
		restartButton = GetNode<Button>("ColorRect/VBoxContainer/Restart");
		mainMenuButton = GetNode<Button>("ColorRect/VBoxContainer/MainMenu");

		resumeButton.Pressed += OnResumePressed;
		restartButton.Pressed += OnRestartPressed;
		mainMenuButton.Pressed += OnMainMenuPressed;

		mainMenu = ResourceLoader.Load<PackedScene>("res://main_menu.tscn");

		staminaWheel.Visible = false;
		buttonsMenu.Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			PauseToggle();
		}
	}

	public void OnResumePressed()
	{
		PauseToggle();
	}

	public void OnRestartPressed()
	{
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	public void OnMainMenuPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToPacked(mainMenu);
	}

	private void PauseToggle()
	{
		isPaused = !isPaused;
		GetTree().Paused = isPaused;
		buttonsMenu.Visible = isPaused;
		Input.MouseMode = isPaused ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;;
	}
}
