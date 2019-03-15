using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android;
using TK.CustomMap.Droid;

using Google.Apis.Drive.v3;
using Android.Content;
using Xamarin.Auth;
using System.Threading;
using GreenBankX.Resources;
using System.Linq;
using Newtonsoft.Json;

namespace GreenBankX.Droid
{
    [Activity(Label = "GreenBankX", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataSchemes = new[] { "com.googleusercontent.apps.263109938909-v6r1cu813081jujunosjadmhc3nr67kk" },
    DataPath = "/oauth2redirect")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const string TAG = "MainActivity";

        const int RC_SIGN_IN = 9001;
        protected override void OnCreate(Bundle bundle)
        {
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            
            // [END configure_signin]

            // [START build_client]
           
        // [END build_client]
        global::Xamarin.Forms.Forms.Init(this, bundle);
            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);
            TKGoogleMaps.Init(this, bundle);
            
            Xamarin.FormsMaps.Init(this, bundle);
            GoogleInfo.GetInstance(this);
            // Convert Android.Net.Url to Uri
            Uri uri;
            if (Intent.Data != null)
            {
                uri = new Uri(Intent.Data.ToString());

                // Load redirectUrl page
                AuthenticationState.Authenticator.OnPageLoading(uri);
            }
            LoadApplication(new App());
        }
        protected override void OnResume()
        {
            const string permission = Manifest.Permission.AccessFineLocation;
            const string persimmon = Manifest.Permission.WriteExternalStorage;
            base.OnResume();
            if (ContextCompat.CheckSelfPermission(this, persimmon) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.WriteExternalStorage }, 0);
            }

            if (ContextCompat.CheckSelfPermission(this, permission) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, 0);
            }

       
        }
        protected override void OnStart()
        {
            base.OnStart();

        }

     

        public void SignOut()
        {
           //mGoogleApiClient.Disconnect();
            GoogleInfo.GetInstance().Acount = null;
            Xamarin.Forms.Application.Current.Properties["Signed"] = false;
            Xamarin.Forms.Application.Current.Properties["Boff"] = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Welcome1");
        }


        protected override void OnStop()
        {
            base.OnStop();

        }

   



    }
}

