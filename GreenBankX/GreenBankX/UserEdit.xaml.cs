using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserEdit : ContentPage
	{
		public UserEdit ()
		{
			InitializeComponent ();
            if (Application.Current.Properties["First"] != null) {
                first.Text = (string)Application.Current.Properties["First"];
            }
            if (Application.Current.Properties["Last"] != null)
            {
                Last.Text = (string)Application.Current.Properties["Last"];
            }
        }
	}
}