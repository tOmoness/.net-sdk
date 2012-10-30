// -----------------------------------------------------------------------
// <copyright file="LocalizedStrings.cs" company="Nokia">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.TestApp.Resources;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources
        {
            get
            {
                return _localizedResources;
            }
        }
    }
}