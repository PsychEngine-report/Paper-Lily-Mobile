using Godot;
using System;

namespace LacieEngine.UI
{
	public class VirtualButton : Control
	{
		[Export] public Color NormalColor = new Color(0.2f, 0.2f, 0.2f, 0.7f);
		[Export] public Color PressedColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);
		[Export] public float Size = 80f;
		[Export] public string Action = "";
		[Export] public string ButtonLabel = "";

		private bool _isPressed = false;
		private bool _editMode = false;
		private int _touchIndex = -1;
		private static Font _defaultFont;

		public override void _Ready()
		{
			AddToGroup("MobileControls");
			RectMinSize = new Vector2(Size, Size);
			MouseFilter = MouseFilterEnum.Ignore; // Default ignore so global Input works

			if (_defaultFont == null)
			{
				using (var temp = new Label()) { _defaultFont = temp.GetFont("font"); }
			}
		}

		// --- STUCK BUTTON FIXES ---
		public override void _ExitTree() { if (_isPressed) ReleaseButton(); }

		public override void _Notification(int what)
		{
			if (what == NotificationVisibilityChanged && !IsVisibleInTree())
				if (_isPressed) ReleaseButton();
		}

		public void UpdateEditState(bool isEditMode)
		{
			_editMode = isEditMode;
			// Allow GuiInput dragging ONLY during edit mode!
			MouseFilter = _editMode ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
			if (_isPressed) ReleaseButton();
			Update();
		}

		public override void _Input(InputEvent @event)
		{
			if (_editMode || !IsVisibleInTree()) return;

			Rect2 hitBox = new Rect2(RectGlobalPosition, RectSize * RectScale);

			if (@event is InputEventScreenTouch touch)
			{
				if (touch.Pressed && _touchIndex == -1 && hitBox.HasPoint(touch.Position))
				{
					_touchIndex = touch.Index;
					PressButton();
				}
				else if (!touch.Pressed && touch.Index == _touchIndex)
				{
					ReleaseButton();
				}
			}
			else if (@event is InputEventScreenDrag drag)
			{
				// SLIDE TO PRESS FIX: Let players slide their thumb smoothly across the D-Pad!
				if (_touchIndex == -1 && hitBox.HasPoint(drag.Position))
				{
					_touchIndex = drag.Index;
					PressButton();
				}
				else if (drag.Index == _touchIndex && !hitBox.HasPoint(drag.Position))
				{
					ReleaseButton();
				}
			}
		}

		public override void _GuiInput(InputEvent @event)
		{
			if (_editMode && @event is InputEventScreenDrag editDrag)
				RectGlobalPosition += editDrag.Relative;
		}

		private void PressButton()
		{
			_isPressed = true;
			SetActionState(Action, true);
			Update();
		}

		private void ReleaseButton()
		{
			_touchIndex = -1;
			_isPressed = false;
			SetActionState(Action, false);
			Update();
		}

		private void SetActionState(string action, bool state)
		{
			if (string.IsNullOrEmpty(action)) return;

			if (state) Input.ActionPress(action);
			else Input.ActionRelease(action);

			foreach (InputEvent ev in InputMap.GetActionList(action))
			{
				if (ev is InputEventJoypadButton joyBtn)
				{
					var spoofJoy = new InputEventJoypadButton();
					spoofJoy.Device = 0;
					spoofJoy.ButtonIndex = joyBtn.ButtonIndex;
					spoofJoy.Pressed = state;
					Input.ParseInputEvent(spoofJoy);
				}
			}
		}

		public override void _Draw()
		{
			Vector2 center = RectSize / 2;
			float radius = Size / 2.2f;
			Color drawColor = _editMode ? new Color(1f, 0.2f, 0.2f, 0.5f) : (_isPressed ? PressedColor : NormalColor);

			DrawCircle(center, radius, drawColor);
			DrawArc(center, radius, 0, Mathf.Tau, 32, new Color(0, 0, 0, 0.5f), 3f);

			if (!string.IsNullOrEmpty(ButtonLabel) && _defaultFont != null)
			{
				Vector2 stringSize = _defaultFont.GetStringSize(ButtonLabel);
				Vector2 textPos = center + new Vector2(-stringSize.x / 2, stringSize.y / 3);

				Color textColor = new Color(1, 1, 1, 1);
				if (ButtonLabel == "A") textColor = new Color(0.2f, 0.8f, 0.2f);
				if (ButtonLabel == "B") textColor = new Color(0.8f, 0.2f, 0.2f);
				if (ButtonLabel == "X") textColor = new Color(0.2f, 0.4f, 0.8f);
				if (ButtonLabel == "Y") textColor = new Color(0.8f, 0.8f, 0.2f);

				DrawString(_defaultFont, textPos, ButtonLabel, textColor);
			}
		}
	}
}
