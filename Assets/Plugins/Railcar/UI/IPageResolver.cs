namespace Railcar.UI
{
	public interface IPageResolver<in T>
	{
		IMenu Resolve(T key);
	}
}