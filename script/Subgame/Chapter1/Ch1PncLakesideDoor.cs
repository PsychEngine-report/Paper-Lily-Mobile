using Godot;
using LacieEngine.API;
using LacieEngine.Core;

namespace LacieEngine.Minigames
{
	public class Ch1PncLakesideDoor : PointAndClick, IMinigame
	{
		[Export(PropertyHint.None, "")]
		private Texture texNoDoorknob;

		[Export(PropertyHint.None, "")]
		private Texture texExitDoorknob;

		[Export(PropertyHint.None, "")]
		private Texture texFacilityDoorknob;

		[Export(PropertyHint.None, "")]
		private Texture texGardenDoorknob;

		[Export(PropertyHint.None, "")]
		private Texture texLibraryDoorknob;

		[Export(PropertyHint.None, "")]
		private Texture texGraveDoorknob;

		[Export(PropertyHint.None, "")]
		private Texture texMothsDoorknob;

		[Export(PropertyHint.None, "")]
		private Texture texShopDoorknob;

		[Export(PropertyHint.None, "")]
		private Texture texGoldDoorknob;

		[GetNode("Targets/Slot1")]
		private TextureRect nSlot1;

		[GetNode("Targets/Slot2")]
		private TextureRect nSlot2;

		[GetNode("Targets/Slot3")]
		private TextureRect nSlot3;

		[GetNode("Targets/Slot4")]
		private TextureRect nSlot4;

		[GetNode("Targets/Slot5")]
		private TextureRect nSlot5;

		[GetNode("Targets/Slot6")]
		private TextureRect nSlot6;

		private PVar varDoorknob1 = "ch1.forest_lakeside_doorknob_1";

		private PVar varDoorknob2 = "ch1.forest_lakeside_doorknob_2";

		private PVar varDoorknob3 = "ch1.forest_lakeside_doorknob_3";

		private PVar varDoorknob4 = "ch1.forest_lakeside_doorknob_4";

		private PVar varDoorknob5 = "ch1.forest_lakeside_doorknob_5";

		private PVar varDoorknob6 = "ch1.forest_lakeside_doorknob_6";

		private PVar varDoorknobToPlace = "ch1.temp_doorknob_to_place";

		private PEvent evtNoKnob = "pnc_door_slot";

		private PEvent evtFacilityKnob = "pnc_door_facility";

		private PEvent evtLibraryKnob = "pnc_door_library";

		private PEvent evtGraveKnob = "pnc_door_grave";

		private PEvent evtMothsKnob = "pnc_door_moths";

		private PEvent evtShopKnob = "pnc_door_shop";

		private PEvent evtGoldKnob = "pnc_door_gold";

		private PEvent evtExitKnob = "pnc_door_exit";

		private PEvent evtIntro = "pnc_door_intro";

		private PVar _lastDoorknobInteracted;

		public override void Init()
		{
			base.Init();
			RefreshDoorknobs();
		}

		public void Start()
		{
			if (!Game.Events.SeenEvent(evtIntro.Id))
			{
				evtIntro.Play();
			}
		}

		public void Slot1()
		{
			ExecuteSlot(varDoorknob1);
		}

		public void Slot2()
		{
			ExecuteSlot(varDoorknob2);
		}

		public void Slot3()
		{
			ExecuteSlot(varDoorknob3);
		}

		public void Slot4()
		{
			ExecuteSlot(varDoorknob4);
		}

		public void Slot5()
		{
			ExecuteSlot(varDoorknob5);
		}

		public void Slot6()
		{
			ExecuteSlot(varDoorknob6);
		}

		public void PlaceDoorknob()
		{
			_lastDoorknobInteracted.NewValue = varDoorknobToPlace.Value;
			RefreshDoorknobs();
		}

		private void RefreshDoorknobs()
		{
			UpdateTextureWithDoorknob(nSlot1, varDoorknob1);
			UpdateTextureWithDoorknob(nSlot2, varDoorknob2);
			UpdateTextureWithDoorknob(nSlot3, varDoorknob3);
			UpdateTextureWithDoorknob(nSlot4, varDoorknob4);
			UpdateTextureWithDoorknob(nSlot5, varDoorknob5);
			UpdateTextureWithDoorknob(nSlot6, varDoorknob6);
		}

		private void UpdateTextureWithDoorknob(TextureRect slot, PVar var)
		{
			slot.Texture = (string)var.Value switch
			{
				"exit" => texExitDoorknob, 
				"facility" => texFacilityDoorknob, 
				"garden" => texGardenDoorknob, 
				"library" => texLibraryDoorknob, 
				"grave" => texGraveDoorknob, 
				"moths" => texMothsDoorknob, 
				"shop" => texShopDoorknob, 
				"gold" => texGoldDoorknob, 
				_ => texNoDoorknob, 
			};
		}

		private void ExecuteSlot(PVar var)
		{
			_lastDoorknobInteracted = var;
			((string)var.Value switch
			{
				"exit" => evtExitKnob, 
				"facility" => evtFacilityKnob, 
				"library" => evtLibraryKnob, 
				"grave" => evtGraveKnob, 
				"moths" => evtMothsKnob, 
				"shop" => evtShopKnob, 
				"gold" => evtGoldKnob, 
				_ => evtNoKnob, 
			}).Play();
		}
	}
}
