using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MixRadioActivity
{
	public partial class ActivityPage : TabbedPage
	{
		public ActivityPage ()
		{
			InitializeComponent ();

            if (Device.OS == TargetPlatform.WinPhone)
            {
                this.Title = this.Title.ToUpperInvariant();
            }
		}
	}
}

