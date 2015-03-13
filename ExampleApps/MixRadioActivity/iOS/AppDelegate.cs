using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.Threading.Tasks;
using System.Reflection;

namespace MixRadioActivity.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			NSObject ver = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];

			LoadApplication (new App (new AuthPlatformSpecific (), ver.ToString(), new UriLauncher()));

			return base.FinishedLaunching (app, options);
		}
	}
}

