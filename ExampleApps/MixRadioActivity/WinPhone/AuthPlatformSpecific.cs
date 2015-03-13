using System;
using System.Threading.Tasks;
using PCLStorage;

namespace MixRadioActivity
{
	public class AuthPlatformSpecific: IAuthPlatformSpecific
	{
		private const string TokenName = "token.txt";

		public async Task StoreTokenDetails(string serialised)
		{
			var rootFolder = FileSystem.Current.LocalStorage;
			var file = await rootFolder.CreateFileAsync(TokenName, CreationCollisionOption.ReplaceExisting);
			await file.WriteAllTextAsync(serialised);
		}

		public async Task<string> LoadTokenDetails()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			if (await rootFolder.CheckExistsAsync (TokenName) == ExistenceCheckResult.FileExists) {
				var file = await rootFolder.GetFileAsync (TokenName);
				return await file.ReadAllTextAsync ();
			} else {
				return null;
			}
		}
	}
}

