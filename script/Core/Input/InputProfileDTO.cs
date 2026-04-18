using System.Collections.Generic;
using Godot;

namespace LacieEngine.Core
{
	internal class InputProfileDTO
	{
		public string Caption;

		public string Type;

		public string Order;

		public string CaptionBase;

		public List<InputMappingDTO> Mappings;

		public List<InputCaptionDTO> Captions;

		public InputProfileDTO()
		{
			Mappings = new List<InputMappingDTO>();
			Captions = new List<InputCaptionDTO>();
		}

		public InputProfileDTO(InputProfile profile)
		{
			Caption = profile.Caption;
			Type = ((profile.Type == InputProfile.InputType.Keyboard) ? "keyboard" : "controller");
			Order = profile.Order.ToString();
			CaptionBase = profile.CaptionBase;
			Mappings = new List<InputMappingDTO>();
			foreach (InputProfile.Mapping mapping in profile.Mappings)
			{
				Mappings.Add(MappingToDTO(mapping));
			}
			foreach (uint scancode in profile.KeyCaptions.Keys)
			{
				InputCaptionDTO caption = new InputCaptionDTO();
				caption.Key = new InputMappingKeyDTO();
				caption.Key.Code = scancode;
				caption.Caption = profile.KeyCaptions[scancode];
				Captions.Add(caption);
			}
			foreach (int code in profile.JoystickButtonCaptions.Keys)
			{
				InputCaptionDTO caption2 = new InputCaptionDTO();
				caption2.Button = new InputMappingButtonDTO();
				caption2.Button.Code = code;
				caption2.Caption = profile.JoystickButtonCaptions[code];
				Captions.Add(caption2);
			}
			foreach (int code2 in profile.JoystickAxisMinusCaptions.Keys)
			{
				InputCaptionDTO caption3 = new InputCaptionDTO();
				caption3.Axis = new InputMappingAxisDTO();
				caption3.Axis.Code = code2;
				caption3.Axis.Value = -1;
				caption3.Caption = profile.JoystickAxisMinusCaptions[code2];
				Captions.Add(caption3);
			}
			foreach (int code3 in profile.JoystickAxisPlusCaptions.Keys)
			{
				InputCaptionDTO caption4 = new InputCaptionDTO();
				caption4.Axis = new InputMappingAxisDTO();
				caption4.Axis.Code = code3;
				caption4.Axis.Value = 1;
				caption4.Caption = profile.JoystickAxisPlusCaptions[code3];
				Captions.Add(caption4);
			}
		}

		private InputMappingDTO MappingToDTO(InputProfile.Mapping mapping)
		{
			InputMappingDTO mappingDto = new InputMappingDTO();
			mappingDto.Action = mapping.Action;
			if (mapping.Event is InputEventKey mappingKey)
			{
				mappingDto.Key = new InputMappingKeyDTO();
				mappingDto.Key.Code = mappingKey.Scancode;
			}
			else if (mapping.Event is InputEventJoypadButton mappingButton)
			{
				mappingDto.Button = new InputMappingButtonDTO();
				mappingDto.Button.Code = mappingButton.ButtonIndex;
			}
			else if (mapping.Event is InputEventJoypadMotion mappingMotion)
			{
				mappingDto.Axis = new InputMappingAxisDTO();
				mappingDto.Axis.Code = mappingMotion.Axis;
				mappingDto.Axis.Value = ((mappingMotion.AxisValue > 0f) ? 1 : 0);
			}
			return mappingDto;
		}

		public InputProfile ToInputProfile()
		{
			InputProfile profile = new InputProfile((!(Type == "keyboard")) ? InputProfile.InputType.Controller : InputProfile.InputType.Keyboard);
			profile.Caption = Caption;
			profile.CaptionBase = CaptionBase;
			if (!Order.IsNullOrEmpty())
			{
				profile.Order = int.Parse(Order);
			}
			foreach (InputMappingDTO mapping in Mappings)
			{
				if (mapping.Key != null)
				{
					profile.AddKeyMapping(mapping.Action, mapping.Key.Code);
				}
				else if (mapping.Button != null)
				{
					profile.AddButtonMapping(mapping.Action, mapping.Button.Code);
				}
				else if (mapping.Axis != null)
				{
					profile.AddAxisMapping(mapping.Action, mapping.Axis.Code, mapping.Axis.Value);
				}
			}
			foreach (InputCaptionDTO caption in Captions)
			{
				if (caption.Key != null)
				{
					profile.AddKeyCaption(caption.Key.Code, caption.Caption);
				}
				else if (caption.Button != null)
				{
					profile.AddButtonCaption(caption.Button.Code, caption.Caption);
				}
				else if (caption.Axis != null)
				{
					profile.AddAxisCaption(caption.Axis.Code, caption.Axis.Value, caption.Caption);
				}
			}
			return profile;
		}
	}
}
