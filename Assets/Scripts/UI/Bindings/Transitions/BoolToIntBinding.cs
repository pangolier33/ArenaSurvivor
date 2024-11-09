using Bones.UI.Bindings.Transitions.Base;


namespace Bones.UI.Bindings.Transitions
{
	public class BoolToIntBinding : TransBinding<bool, int>
	{
		protected override int Convert(bool from)
		{
			return System.Convert.ToInt32(from);
		}
	}
}