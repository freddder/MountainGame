using Godot;
using System;

public partial class PlayerMenu : Node
{
	private TextureProgressBar staminaWheel;
	private ColorRect buttonsMenu;
	private Button resumeButton;
	private PackedScene mainMenu;

	public override void _Ready()
	{
		staminaWheel = GetNode<TextureProgressBar>("StaminaWheel");
		buttonsMenu = GetNode<ColorRect>("ColorRect");
		resumeButton = GetNode<Button>("ColorRect/VBoxContainer/Resume");

		mainMenu = ResourceLoader.Load<PackedScene>("res://main_menu.tscn");
		staminaWheel.Visible = false;
		buttonsMenu.Visible = false;
	}

	public void OnRestartPressed()
	{
		GetTree().ReloadCurrentScene();
	}

	public void OnMainMenuPressed()
	{
		GetTree().ChangeSceneToPacked(mainMenu);
	}
}
