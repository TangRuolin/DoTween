namespace DG.Tweening.Core
{
	internal class SequenceCallback : ABSSequentiable
	{
		public SequenceCallback(float sequencedPosition, TweenCallback callback)
		{
			base.tweenType = TweenType.Callback;
			base.sequencedPosition = sequencedPosition;
			base.onStart = callback;
		}
	}
}
