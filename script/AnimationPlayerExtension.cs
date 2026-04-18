using System.Threading.Tasks;
using Godot;

namespace LacieEngine.Core
{
	public static class AnimationPlayerExtension
	{
		public static void ClearAnimations(this AnimationPlayer animationPlayer)
		{
			string[] animationList = animationPlayer.GetAnimationList();
			foreach (string animation in animationList)
			{
				animationPlayer.RemoveAnimation(animation);
			}
		}

		public static void PlayFirstAnimation(this AnimationPlayer animationPlayer)
		{
			animationPlayer.Play(animationPlayer.GetAnimationList()[(animationPlayer.GetAnimationList()[0] == "RESET") ? 1u : 0u]);
		}

		public static async Task WaitUntilFinished(this AnimationPlayer animationPlayer)
		{
			await animationPlayer.ToSignal(animationPlayer, "animation_finished");
		}
	}
}
