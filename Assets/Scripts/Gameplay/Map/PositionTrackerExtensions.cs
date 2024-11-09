using System;
using Bones.Gameplay.Utils;
using UnityEngine;

namespace Bones.Gameplay.Map
{
	public static class PositionTrackerExtensions
	{
		public static IObservable<Vector2Int> TrackShifting(this IPositionTracker tracker, Vector2 step)
			=> tracker.Property.SnapDerivative(step);
	}
}