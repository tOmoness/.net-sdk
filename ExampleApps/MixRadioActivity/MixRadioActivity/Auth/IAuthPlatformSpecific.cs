using System.Threading.Tasks;

namespace MixRadioActivity
{
	public interface IAuthPlatformSpecific
	{
		Task StoreTokenDetails (string serialised);
		Task<string> LoadTokenDetails();
	}
}

