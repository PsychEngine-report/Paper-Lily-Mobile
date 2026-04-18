using System;
using System.Collections.Generic;
using Godot;

namespace LacieEngine.Core
{
	public class InputProfile : IComparable<InputProfile>
	{
		public enum InputType
		{
			Keyboard,
			Controller
		}

		public class Mapping
		{
			public string Action;

			public InputEvent Event;
		}

		public string Name;

		public string Caption;

		public int Order;

		public InputType Type;

		public List<Mapping> Mappings = new List<Mapping>();

		public string CaptionBase;

		public Dictionary<uint, string> KeyCaptions;

		public Dictionary<int, string> JoystickButtonCaptions;

		public Dictionary<int, string> JoystickAxisPlusCaptions;

		public Dictionary<int, string> JoystickAxisMinusCaptions;

		public InputProfile(InputType type)
		{
			Type = type;
			Mappings = new List<Mapping>();
			KeyCaptions = new Dictionary<uint, string>();
			JoystickButtonCaptions = new Dictionary<int, string>();
			JoystickAxisPlusCaptions = new Dictionary<int, string>();
			JoystickAxisMinusCaptions = new Dictionary<int, string>();
		}

		public InputProfile(InputProfile baseProfile)
		{
			Type = baseProfile.Type;
			Mappings = new List<Mapping>(baseProfile.Mappings);
			CaptionBase = baseProfile.CaptionBase;
			KeyCaptions = new Dictionary<uint, string>(baseProfile.KeyCaptions);
			JoystickButtonCaptions = new Dictionary<int, string>(baseProfile.JoystickButtonCaptions);
			JoystickAxisPlusCaptions = new Dictionary<int, string>(baseProfile.JoystickAxisPlusCaptions);
			JoystickAxisMinusCaptions = new Dictionary<int, string>(baseProfile.JoystickAxisMinusCaptions);
		}

		public void AddKeyMapping(string action, uint key, bool shift = false, bool ctrl = false, bool alt = false)
		{
			InputEventKey evt = new InputEventKey();
			evt.Scancode = key;
			evt.Shift = shift;
			evt.Control = ctrl;
			evt.Alt = alt;
			Mapping mapping = new Mapping();
			mapping.Action = action;
			mapping.Event = evt;
			Mappings.Add(mapping);
		}

		public void AddButtonMapping(string action, int button)
		{
			InputEventJoypadButton evt = new InputEventJoypadButton();
			evt.ButtonIndex = button;
			Mapping mapping = new Mapping();
			mapping.Action = action;
			mapping.Event = evt;
			Mappings.Add(mapping);
		}

		public void AddAxisMapping(string action, int axis, int value)
		{
			InputEventJoypadMotion evt = new InputEventJoypadMotion();
			evt.Axis = axis;
			evt.AxisValue = ((value > 0) ? 1f : (-1f));
			Mapping mapping = new Mapping();
			mapping.Action = action;
			mapping.Event = evt;
			Mappings.Add(mapping);
		}

		public bool IsComplete()
		{
			string[] allGame = Inputs.AllGame;
			foreach (string action in allGame)
			{
				if (!IsActionMapped(action))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsActionMapped(string action)
		{
			foreach (Mapping mapping in Mappings)
			{
				if (mapping.Action == action)
				{
					return true;
				}
			}
			return false;
		}

		public void UnassignMapping(string action)
		{
			Mappings.RemoveAll((Mapping mapping) => mapping.Action == action);
		}

		public void UnassignEvent(InputEvent @event)
		{
			List<Mapping> toRemove = new List<Mapping>();
			foreach (Mapping mapping in Mappings)
			{
				if (mapping.Event is InputEventKey mappingKey && @event is InputEventKey eventKey)
				{
					if (mappingKey.Scancode == eventKey.Scancode)
					{
						toRemove.Add(mapping);
					}
				}
				else if (mapping.Event is InputEventJoypadButton mappingButton && @event is InputEventJoypadButton eventButton)
				{
					if (mappingButton.ButtonIndex == eventButton.ButtonIndex)
					{
						toRemove.Add(mapping);
					}
				}
				else if (mapping.Event is InputEventJoypadMotion mappingMotion && @event is InputEventJoypadMotion eventMotion && mappingMotion.Axis == eventMotion.Axis && mappingMotion.AxisValue == eventMotion.AxisValue)
				{
					toRemove.Add(mapping);
				}
			}
			Mappings.RemoveAll((Mapping item) => toRemove.Contains(item));
		}

		public void AddKeyCaption(uint key, string caption)
		{
			KeyCaptions[key] = caption;
		}

		public void AddButtonCaption(int button, string caption)
		{
			JoystickButtonCaptions[button] = caption;
		}

		public void AddAxisCaption(int axis, int value, string caption)
		{
			if (value > 0)
			{
				JoystickAxisPlusCaptions[axis] = caption;
			}
			else
			{
				JoystickAxisMinusCaptions[axis] = caption;
			}
		}

		public string GetCaptionForAction(string action)
		{
			foreach (Mapping mapping in Mappings)
			{
				if (mapping.Action == action)
				{
					return GetCaptionForEvent(mapping.Event);
				}
			}
			return "-";
		}

		public string GetAllCaptionsForAction(string action)
		{
			List<string> captions = new List<string>();
			foreach (Mapping mapping in Mappings)
			{
				if (mapping.Action == action)
				{
					captions.Add(GetCaptionForEvent(mapping.Event));
				}
			}
			if (captions.Count > 0)
			{
				return string.Join("/", captions);
			}
			return "-";
		}

		public string GetCaptionForEvent(InputEvent evt)
		{
			if (CaptionBase != null && Inputs.Profiles.ContainsKey(CaptionBase))
			{
				return Inputs.Profiles[CaptionBase].GetCaptionForEvent(evt);
			}
			if (evt is InputEventKey { Scancode: var code })
			{
				if (KeyCaptions.ContainsKey(code))
				{
					return ProcessCaption(KeyCaptions[code]);
				}
				return OS.GetScancodeString(code);
			}
			if (evt is InputEventJoypadButton { ButtonIndex: var code2 })
			{
				if (JoystickButtonCaptions.ContainsKey(code2))
				{
					return ProcessCaption(JoystickButtonCaptions[code2]);
				}
				return Game.Language.GetCaption("system.common.button") + " " + (code2 + 1);
			}
			if (evt is InputEventJoypadMotion { Axis: var code3, AxisValue: var value })
			{
				if (value > 0f && JoystickAxisPlusCaptions.ContainsKey(code3))
				{
					return ProcessCaption(JoystickAxisPlusCaptions[code3]);
				}
				if (JoystickAxisMinusCaptions.ContainsKey(code3))
				{
					return ProcessCaption(JoystickAxisMinusCaptions[code3]);
				}
				return Game.Language.GetCaption("system.common.axis") + " " + (code3 + 1) + ((value > 0f) ? " +" : " -");
			}
			return "-";
		}

		private string ProcessCaption(string caption)
		{
			if (caption.StartsWith("img:"))
			{
				string[] obj = new string[5] { "[img]", "res://assets/img/ui/input/", null, null, null };
				obj[2] = caption.Substring(4, caption.Length - 4);
				obj[3] = ".png";
				obj[4] = "[/img]";
				return string.Concat(obj);
			}
			return Game.Language.GetCaption(caption);
		}

		public int CompareTo(InputProfile profile)
		{
			return Order.CompareTo(profile.Order);
		}
	}
}
