using System;

namespace Bones.Web
{
	[Serializable]
	public class TonrareUser
	{
		public string id;
		public string username;
		public string created_at;
		public string referral_by_id;
	}
}
