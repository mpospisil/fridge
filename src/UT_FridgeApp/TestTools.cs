using System.Threading.Tasks;

namespace UT_FridgeApp
{
	public static class TestTools
	{
		public static Task<T> ToTask<T>(this T value)
		{
			return Task.FromResult(value);
		}
	}
}
