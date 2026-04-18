using System;
using System.Collections.Generic;
using Godot;
using LacieEngine.API;
using LacieEngine.Core;

namespace LacieEngine.Characters
{
	[Injectable]
	public class CharacterManager : ICharacterManager, IModule, ITranslatable, IStateOverridable
	{
		private const double AssetReloadPeriod = 5.0;

		private const string CAPTION_GROUP = "characters";

		private Dictionary<string, Character> characters;

		private Dictionary<string, DateTime> lastLoadDate;

		private Dictionary<string, IList<string>> characterResources;

		public ICharacter Get(string id)
		{
			if (characters.ContainsKey(id))
			{
				return characters[id];
			}
			Log.Error("Character ", id, " not found!");
			return null;
		}

		public bool Exists(string id)
		{
			return characters.ContainsKey(id);
		}

		public bool IsMoodValid(string id, string mood)
		{
			if (!characters.ContainsKey(id))
			{
				return false;
			}
			return characters[id].Moods.Contains(mood);
		}

		public void Init()
		{
			Log.Info("Loading characters...");
			characters = new Dictionary<string, Character>();
			lastLoadDate = new Dictionary<string, DateTime>();
			characterResources = new Dictionary<string, IList<string>>();
			foreach (string filename in GDUtil.ListFilesInPath("res://definitions/characters/", null, ".json", fullPath: false))
			{
				string id = filename.Substring(0, filename.LastIndexOf(".json"));
				CharacterDTO characterDto = GDUtil.ReadJsonFile<CharacterDTO>("res://definitions/characters/" + filename);
				Character character = new Character();
				character.Id = id;
				string name = (character.OriginalName = characterDto.Name);
				character.Name = name;
				character.Costumes.AddRange(characterDto.Costumes);
				character.Moods.AddRange(FindMoodsForCharacter(id));
				GatherResourcesForCharacter(character.Id);
				LoadCharacterSpriteData(character, characterDto);
				characters.Add(id, character);
			}
		}

		public IList<string> GetDependencies(string characterId)
		{
			if (characterResources.ContainsKey(characterId))
			{
				return characterResources[characterId];
			}
			return Array.Empty<string>();
		}

		public void LoadResourcesForCharacter(string characterId)
		{
			if (!characters.ContainsKey(characterId) || !characterResources.ContainsKey(characterId) || (lastLoadDate.ContainsKey(characterId) && lastLoadDate[characterId].AddSeconds(5.0) > DateTime.Now))
			{
				return;
			}
			lastLoadDate[characterId] = DateTime.Now;
			Log.Trace("Loading character resources for ", characterId);
			foreach (string path in characterResources[characterId])
			{
				if (!path.EndsWith(".json"))
				{
					Game.Memory.Cache(path);
				}
			}
		}

		public void OverrideCharacterName(string characterId, string newName)
		{
			Game.State.OverrideCharacterNames[characterId] = newName;
			characters[characterId].Name = newName;
		}

		public void RemoveOverrideCharacterName(string characterId)
		{
			Game.State.OverrideCharacterNames.Remove(characterId);
			if (Game.Language.TranslationEnabled)
			{
				characters[characterId].Name = Game.Language.GetCaption("characters", characters[characterId].OriginalName, CharacterNameContext(characters[characterId]));
			}
			else
			{
				characters[characterId].Name = characters[characterId].OriginalName;
			}
		}

		public void OverrideMood(string characterId, string mood, string newMood)
		{
			Game.State.Overrides["chara." + characterId + ".busts." + mood] = newMood;
		}

		public void RemoveOverrideMoods(string characterId)
		{
			Game.State.Overrides.RemoveAll((string k, string v) => k.StartsWith("chara." + characterId + ".busts."));
		}

		public Texture GetMoodTexture(string characterId, string mood)
		{
			if (Game.State.Overrides.ContainsKey("chara." + characterId + ".busts." + mood))
			{
				mood = Game.State.Overrides["chara." + characterId + ".busts." + mood];
			}
			else if (Game.State.Overrides.ContainsKey("chara." + characterId + ".busts.all"))
			{
				mood = Game.State.Overrides["chara." + characterId + ".busts.all"];
			}
			return GD.Load<Texture>("res://assets/img/bust/" + characterId + "/" + mood + ".png");
		}

		private void GatherResourcesForCharacter(string characterId)
		{
			List<string> resources = new List<string>();
			resources.Add("res://definitions/characters/" + characterId + ".json");
			if (GDUtil.FolderExists("res://assets/sprite/common/character/" + characterId))
			{
				resources.AddRange(GDUtil.ListFilesInPath("res://assets/sprite/common/character/" + characterId + "/", ".png"));
			}
			if (GDUtil.FolderExists("res://assets/img/bust/" + characterId))
			{
				resources.AddRange(GDUtil.ListFilesInPath("res://assets/img/bust/" + characterId + "/", ".png"));
			}
			characterResources[characterId] = resources;
		}

		private IList<string> FindMoodsForCharacter(string characterId)
		{
			if (GDUtil.FolderExists("res://assets/img/bust/" + characterId))
			{
				List<string> moods = new List<string>();
				{
					foreach (string bustFile in GDUtil.ListFilesInPath("res://assets/img/bust/" + characterId + "/", null, ".png", fullPath: false))
					{
						moods.Add(bustFile.StripSuffix(".png"));
					}
					return moods;
				}
			}
			return Array.Empty<string>();
		}

		private void LoadCharacterSpriteData(Character character, CharacterDTO characterDto)
		{
			if (!characterDto.NoDefaultSprites)
			{
				character.StateSprites["stand"] = new Character.CharacterStateSprites(Character.DefaultStand);
				character.StateSprites["walk"] = new Character.CharacterStateSprites(Character.DefaultWalk);
				character.StateSprites["run"] = new Character.CharacterStateSprites(Character.DefaultRun);
			}
			foreach (CharacterDTO.CharacterSpriteDTO spriteDto in characterDto.Sprites)
			{
				if (spriteDto.State == "idle")
				{
					character.IdleAnimation = true;
				}
				Character.CharacterStateSprites stateSprite = new Character.CharacterStateSprites();
				stateSprite.TextureVariation = (spriteDto.TextureSuffix.IsNullOrEmpty() ? "" : ("_" + spriteDto.TextureSuffix));
				stateSprite.Hframes = spriteDto.Hframes;
				stateSprite.Vframes = spriteDto.Vframes;
				if (!spriteDto.Offset.IsNullOrEmpty())
				{
					stateSprite.OffsetOverride = GDUtil.StringToVector2(spriteDto.Offset);
				}
				if (IsAnimatedSprite(spriteDto))
				{
					stateSprite.Animated = true;
					stateSprite.AnimationFps = spriteDto.Fps;
					stateSprite.AnimationLeftFrames = spriteDto.LeftFrames;
					stateSprite.AnimationUpFrames = spriteDto.UpFrames;
					stateSprite.AnimationRightFrames = spriteDto.RightFrames;
					stateSprite.AnimationDownFrames = spriteDto.DownFrames;
					stateSprite.Loop = spriteDto.Loop;
				}
				else
				{
					int.TryParse(spriteDto.LeftFrames, out stateSprite.LeftFrame);
					int.TryParse(spriteDto.UpFrames, out stateSprite.UpFrame);
					int.TryParse(spriteDto.RightFrames, out stateSprite.RightFrame);
					int.TryParse(spriteDto.DownFrames, out stateSprite.DownFrame);
				}
				character.StateSprites[spriteDto.State] = stateSprite;
			}
			string p = "res://assets/sprite/common/character/" + character.Id + "/" + character.Id;
			foreach (Character.CharacterStateSprites stateSprite2 in character.StateSprites.Values)
			{
				string s = stateSprite2.TextureVariation + ".png";
				string n = stateSprite2.TextureVariation + "_n.png";
				stateSprite2.Texture = (GDUtil.FileExists(p + s) ? GD.Load<Texture>(p + s) : null);
				stateSprite2.NormalMap = (GDUtil.FileExists(p + n) ? GD.Load<Texture>(p + n) : null);
				if (stateSprite2.Texture == null)
				{
					Log.Error("[CharacterManager] [", character.Id, "] No texture variation found ", stateSprite2.TextureVariation);
				}
			}
			static bool IsAnimatedFrames(string framesStr)
			{
				if (!framesStr.IsNullOrEmpty())
				{
					return framesStr.Split(",").Length > 1;
				}
				return false;
			}
			static bool IsAnimatedSprite(CharacterDTO.CharacterSpriteDTO characterSpriteDTO)
			{
				if (!IsAnimatedFrames(characterSpriteDTO.LeftFrames) && !IsAnimatedFrames(characterSpriteDTO.UpFrames) && !IsAnimatedFrames(characterSpriteDTO.RightFrames))
				{
					return IsAnimatedFrames(characterSpriteDTO.DownFrames);
				}
				return true;
			}
		}

		public void ApplyTranslationOverrides()
		{
			foreach (Character character in characters.Values)
			{
				character.Name = Game.Language.GetCaption("characters", character.OriginalName, CharacterNameContext(character));
			}
		}

		public void ApplyStateOverrides()
		{
			foreach (Character character in characters.Values)
			{
				if (Game.State.OverrideCharacterNames.ContainsKey(character.Id))
				{
					character.Name = Game.State.OverrideCharacterNames[character.Id];
				}
			}
		}

		public void Clean()
		{
			foreach (Character value in characters.Values)
			{
				value.Name = value.OriginalName;
			}
		}

		private string CharacterNameContext(ICharacter character)
		{
			return "characters.name." + character.Id;
		}
	}
}
