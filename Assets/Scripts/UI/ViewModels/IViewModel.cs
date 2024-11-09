namespace Bones.UI.Presenters.New
{
	public interface IViewModel<in T>
	{
		void Bind(T model);
	}
}