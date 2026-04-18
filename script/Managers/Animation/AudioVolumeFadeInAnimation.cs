using Godot;

namespace LacieEngine.Animation
{
	public class AudioVolumeFadeInAnimation : AudioVolumeFadeAnimation
	{
		public AudioVolumeFadeInAnimation(AudioStreamPlayer node, float finalVolume, float duration)
			: base(node, duration, 0f, finalVolume)
		{
		}
	}
}
