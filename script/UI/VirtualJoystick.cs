using Godot;
using System;

namespace LacieEngine.UI
{
	public class VirtualJoystick : Control
	{
		[Export] public float BaseRadius = 80f;
		[Export] public float StickRadius = 30f;

		private Vector2 _stickPos = Vector2.Zero;
		private int _touchIndex = -1;
		private bool _editMode = false;

		public override void _Ready()
		{
			AddToGroup("MobileControls");
			RectMinSize = new Vector2(BaseRadius * 2, BaseRadius * 2);
			MouseFilter = MouseFilterEnum.Ignore;
		}

		public override void _ExitTree() { if (_touchIndex != -1) ReleaseJoystick(); }
		public override void _Notification(int what)
		{
			if (what == NotificationVisibilityChanged && !IsVisibleInTree())
				if (_touchIndex != -1) ReleaseJoystick();
		}

		public void UpdateEditState(bool isEditMode)
		{
			_editMode = isEditMode;
			MouseFilter = _editMode ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
			if (_editMode) ReleaseJoystick();
			Update();
		}

		public override void _Input(InputEvent @event)
		{
			if (_editMode || !IsVisibleInTree()) return;

			if (@event is InputEventScreenTouch touch)
			{
				if (touch.Pressed && _touchIndex == -1)
				{
					Rect2 hitBox = new Rect2(RectGlobalPosition, RectSize * RectScale);
					if (hitBox.HasPoint(touch.Position))
					{
						_touchIndex = touch.Index;
						UpdateStickFromGlobal(touch.Position);
					}
				}
				else if (!touch.Pressed && touch.Index == _touchIndex)
				{
					ReleaseJoystick();
				}
			}
			else if (@event is InputEventScreenDrag drag && drag.Index == _touchIndex)
			{
				UpdateStickFromGlobal(drag.Position);
			}
		}

		public override void _GuiInput(InputEvent @event)
		{
			if (_editMode && @event is InputEventScreenDrag editDrag)
				RectGlobalPosition += editDrag.Relative;
		}

		private void ReleaseJoystick()
		{
			_touchIndex = -1;
			UpdateStick(Vector2.Zero);
		}

		private void UpdateStickFromGlobal(Vector2 globalTouchPos)
		{
			Vector2 localPos = (globalTouchPos - RectGlobalPosition) / RectScale;
			Vector2 center = RectSize / 2;
			UpdateStick(localPos - center);
		}

		private void UpdateStick(Vector2 offset)
		{
			if (offset.Length() > BaseRadius) offset = offset.Normalized() * BaseRadius;
			_stickPos = offset;
			Update();

			Vector2 analogValues = offset / BaseRadius;
			InjectJoypadAxis(0, analogValues.x);
			InjectJoypadAxis(1, analogValues.y);
		}

		private void InjectJoypadAxis(int axisIndex, float axisValue)
		{
			var spoofAxis = new InputEventJoypadMotion();
			spoofAxis.Device = 0;
			spoofAxis.Axis = axisIndex;
			spoofAxis.AxisValue = axisValue;
			Input.ParseInputEvent(spoofAxis);
		}

		public override void _Draw()
		{
			Vector2 center = RectSize / 2;
			Color baseC = _editMode ? new Color(1f, 0.2f, 0.2f, 0.4f) : new Color(0.1f, 0.1f, 0.1f, 0.5f);

			DrawCircle(center, BaseRadius, baseC);
			DrawArc(center, BaseRadius, 0, Mathf.Tau, 32, new Color(0, 0, 0, 0.6f), 4f);

			DrawCircle(center + _stickPos, StickRadius, new Color(0.7f, 0.7f, 0.7f, 0.9f));
			DrawArc(center + _stickPos, StickRadius, 0, Mathf.Tau, 32, new Color(0, 0, 0, 0.8f), 2f);
		}
	}
}
