using Godot;

namespace LacieEngine.Animation
{
	public class AudioVolumeFadeOutAnimation : AudioVolumeFadeAnimation
	{
		public AudioVolumeFadeOutAnimation(AudioStreamPlayer node, float duration)
			: base(node, duration, GD.Db2Linear(node.VolumeDb), 0f)
		{
		}
	}
}
