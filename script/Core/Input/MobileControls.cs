using Godot;
using System;
using LacieEngine.Core; // Hooks into your existing input system

public class MobileButton : Control
{
	[Export] public string ActionName = Inputs.Action; // Defaults to your 'input_action' constant
	[Export] public Texture ButtonTexture;
	
	// Position percentage (0.0 to 1.0). e.g., 0.8, 0.8 puts it in the bottom right corner
	[Export] public Vector2 AnchorPosition = new Vector2(0.8f, 0.8f); 
	[Export] public float BaseAlpha = 0.7f; // Controls transparency, separate from scale
	[Export] public float SizeMultiplier = 1.0f; // Fine-tune individual button sizes

	// Base resolution used to calculate consistent aspect ratios across devices
	private readonly Vector2 BASE_RESOLUTION = new Vector2(1920, 1080);
	private TouchScreenButton _touchButton;

	public override void _Ready()
	{
		_touchButton = new TouchScreenButton();
		_touchButton.Normal = ButtonTexture;
		_touchButton.Modulate = new Color(1, 1, 1, BaseAlpha);

		// Connect touch signals to our custom hardware event injector
		_touchButton.Connect("pressed", this, nameof(OnPressed));
		_touchButton.Connect("released", this, nameof(OnReleased));

		AddChild(_touchButton);

		// Listen for screen rotation or resolution changes
		GetTree().Root.Connect("size_changed", this, nameof(UpdateLayout));
		UpdateLayout();
	}

	public void UpdateLayout()
	{
		Vector2 screenSize = GetViewportRect().Size;

		// 1. Maintain consistent visual size based on screen height ratio
		float scaleFactor = (screenSize.y / BASE_RESOLUTION.y) * SizeMultiplier;
		_touchButton.Scale = new Vector2(scaleFactor, scaleFactor);

		// 2. Center the button exactly on the anchor percentage
		Vector2 textureSize = (ButtonTexture != null) ? ButtonTexture.GetSize() * scaleFactor : Vector2.Zero;
		Vector2 targetPos = (screenSize * AnchorPosition) - (textureSize / 2f);

		_touchButton.Position = targetPos;
	}

	private void OnPressed() => InjectHardwareEvent(ActionName, true);
	private void OnReleased() => InjectHardwareEvent(ActionName, false);

	/// <summary>
	/// Bypasses LacieEngine's strict hardware checks by synthesizing the exact 
	/// physical key mapped to the target action in the InputProfile.
	/// </summary>
	private void InjectHardwareEvent(string action, bool isPressed)
	{
		// Search the InputMap for the physical key assigned to this action
		foreach (InputEvent ev in InputMap.GetActionList(action))
		{
			if (ev is InputEventKey keyEv)
			{
				// Create and fire a synthetic physical keypress
				var fakeKey = new InputEventKey();
				fakeKey.Scancode = keyEv.Scancode;
				fakeKey.Pressed = isPressed;
				Input.ParseInputEvent(fakeKey);
				return; 
			}
		}

		// Fallback for actions strictly mapped to joypads without key fallbacks
		var actionEv = new InputEventAction();
		actionEv.Action = action;
		actionEv.Pressed = isPressed;
		Input.ParseInputEvent(actionEv);
	}
}
