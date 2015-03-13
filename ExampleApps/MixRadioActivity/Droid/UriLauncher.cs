using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace MixRadioActivity
{
    public class UriLauncher : IUriLauncher
    {
        public void LaunchUri(Uri uri)
        {
            Debug.WriteLine("launch " + uri.ToString());
            try
            {
                Device.OpenUri(uri);
            }
            catch (Exception e)
            {
                Debug.WriteLine("launch failed " + e.ToString());
            }
        }
    }
}

