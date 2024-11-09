using System.Collections.Generic;

namespace Bones.Analitics
{
	public static class AppmetricaHelper
	{
		public static void SendLevelStartEvent(int level)
		{
			var parameters = new Dictionary<string, object>()
			{
				{ "level", level }
			};
			//AppMetrica.Instance.ReportEvent("level_start", parameters);
			//AppMetrica.Instance.SendEventsBuffer();
		}

		public static void SendLevelCompleteEvent(int level, int timeSpent)
		{
			var parameters = new Dictionary<string, object>()
			{
				{ "level", level },
				{ "time_spent", timeSpent }
			};
			//AppMetrica.Instance.ReportEvent("level_complete", parameters);
			//AppMetrica.Instance.SendEventsBuffer();
		}

		public static void SendLevelFailEvent(int level, int timeSpent)
		{
			var parameters = new Dictionary<string, object>()
			{
				{ "level", level },
				{ "time_spent", timeSpent }
			};
			//AppMetrica.Instance.ReportEvent("level_fail", parameters);
			//AppMetrica.Instance.SendEventsBuffer();
		}
	}
}
