using Godot;
using System;

namespace LacieEngine.UI
{
	public class MobileManager : CanvasLayer
	{
		public bool IsEditMode = false;
		private string SavePath = "user://mobile_ui.cfg";

			private Button _editToggle;
			private Control _editUI;
			private Button _opacityBtn;
			private Button _moveBtn;

			private float _opacity = 1.0f;
			private bool _useJoystick = true;

			public override void _Ready()
			{
				_editToggle = GetNode<Button>("SafeArea/EditToggle");
				_editUI = GetNode<Control>("SafeArea/EditUI");
				_opacityBtn = GetNode<Button>("SafeArea/EditUI/OpacityBtn");
				_moveBtn = GetNode<Button>("SafeArea/EditUI/MovementBtn");

				_editUI.RectPivotOffset = _editUI.RectSize / 2; // Keeps scaling centered
				LoadSettings();
			}

			public void ToggleEditMode()
			{
				IsEditMode = !IsEditMode;

				if (IsEditMode)
				{
					_editToggle.Text = "SAVE & EXIT EDIT MODE";
					_editToggle.Modulate = new Color(1f, 0.2f, 0.2f);
					_editUI.Visible = true;
				}
				else
				{
					_editToggle.Text = "Edit Controls";
					_editToggle.Modulate = new Color(1f, 1f, 1f);
					_editUI.Visible = false;
					SaveSettings();
				}

				foreach (Node node in GetTree().GetNodesInGroup("MobileControls"))
				{
					if (node is VirtualButton vb) vb.UpdateEditState(IsEditMode);
					else if (node is VirtualJoystick vj) vj.UpdateEditState(IsEditMode);
				}
			}

			public void ToggleMovement()
			{
				_useJoystick = !_useJoystick;
				ApplyMovementType();
			}

			public void CycleOpacity()
			{
				_opacity += 0.1f;
				if (_opacity > 1.05f) _opacity = 0.0f;
				_opacity = (float)Math.Round(_opacity, 1);
				ApplyAppearance();
			}

			public void IncreaseSize() { ChangeSize(0.1f); }
			public void DecreaseSize() { ChangeSize(-0.1f); }

			private void ChangeSize(float amount)
			{
				foreach (Control node in GetTree().GetNodesInGroup("MobileControls"))
				{
					Vector2 newScale = node.RectScale + new Vector2(amount, amount);
					if (newScale.x > 0.4f && newScale.x < 3.0f) node.RectScale = newScale;
				}

				// Scale the Edit UI Toolbar as well
				Vector2 uiScale = _editUI.RectScale + new Vector2(amount, amount);
				if (uiScale.x > 0.4f && uiScale.x < 2.0f) _editUI.RectScale = uiScale;
			}

			private void ApplyAppearance()
			{
				_opacityBtn.Text = $"Opacity: {_opacity * 100}%";
				_editUI.Modulate = new Color(1f, 1f, 1f, Math.Max(_opacity, 0.3f)); // Keep UI slightly visible minimum

				foreach (Control node in GetTree().GetNodesInGroup("MobileControls"))
					node.Modulate = new Color(1f, 1f, 1f, _opacity);
			}

			private void ApplyMovementType()
			{
				_moveBtn.Text = _useJoystick ? "Type: Joy" : "Type: DPad";
				GetNode<Control>("SafeArea/VirtualJoystick").Visible = _useJoystick;
				GetNode<Control>("SafeArea/DPad").Visible = !_useJoystick;
			}

			private void SaveSettings()
			{
				var config = new ConfigFile();
				config.SetValue("Settings", "Opacity", _opacity);
				config.SetValue("Settings", "UseJoystick", _useJoystick);
				config.SetValue("Settings", "EditUIScale", _editUI.RectScale);

				foreach (Control node in GetTree().GetNodesInGroup("MobileControls"))
				{
					config.SetValue("Positions", node.Name, node.RectGlobalPosition);
					config.SetValue("Scales", node.Name, node.RectScale);
				}
				config.Save(SavePath);
			}

			private void LoadSettings()
			{
				var config = new ConfigFile();
				if (config.Load(SavePath) == Error.Ok)
				{
					_opacity = Convert.ToSingle(config.GetValue("Settings", "Opacity", 1.0f));
					_useJoystick = (bool)config.GetValue("Settings", "UseJoystick", true);
					_editUI.RectScale = (Vector2)config.GetValue("Settings", "EditUIScale", Vector2.One);

					ApplyAppearance();
					ApplyMovementType();

					foreach (Control node in GetTree().GetNodesInGroup("MobileControls"))
					{
						if (config.HasSectionKey("Positions", node.Name))
							node.RectGlobalPosition = (Vector2)config.GetValue("Positions", node.Name);

						if (config.HasSectionKey("Scales", node.Name))
							node.RectScale = (Vector2)config.GetValue("Scales", node.Name);
					}
				}
			}
	}
}
