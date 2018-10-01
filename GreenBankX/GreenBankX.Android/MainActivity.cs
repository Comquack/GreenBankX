using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android;
using TK.CustomMap.Droid;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using Android.Gms.Common;
using Android.Content;
using Android.Gms.Drive;
using Android.Content.Res;
using System.Threading;
using Google.Apis.Util.Store;
using System.IO;

namespace GreenBankX.Droid
{
    [Activity(Label = "GreenBankX", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const string TAG = "MainActivity";

        const int RC_SIGN_IN = 9001;
        GoogleApiClient mGoogleApiClient;
        protected override void OnCreate(Bundle bundle)
        {
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            // [START configure_signin]
            // Configure sign-in to request the user's ID, email address, and basic
            // profile. ID and basic profile are included in DEFAULT_SIGN_IN.
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)    
                .RequestEmail()
                .RequestScopes(new Scope(Constants.scopes))
                .RequestScopes(DriveClass.ScopeFile)
                .RequestScopes(DriveClass.ScopeAppfolder)
                 .Build();
            
            // [END configure_signin]

            // [START build_client]
            // Build a GoogleApiClient with access to the Google Sign-In API and the
            // options specified by gso.
            mGoogleApiClient = new GoogleApiClient.Builder(this)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API,gso)
                    .AddApi(DriveClass.API)
                  .AddOnConnectionFailedListener(OnConnectionFailed)
                    .Build();
            if (!mGoogleApiClient.IsConnected) { 
                mGoogleApiClient.Connect(GoogleApiClient.SignInModeOptional);
                //mGoogleApiClient.Connect(GoogleApiClient.SignInModeRequired);
            }
        // [END build_client]
        global::Xamarin.Forms.Forms.Init(this, bundle);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);
            TKGoogleMaps.Init(this, bundle);
            GoogleInfo.GetInstance(this);
            GoogleInfo.GetInstance().SignInApi = mGoogleApiClient;
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

           var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
            if (opr.IsDone)
            {
           //      If the user's cached credentials are valid, the OptionalPendingResult will be "done"
           //      and the GoogleSignInResult will be available instantly.
                var result = opr.Get() as GoogleSignInResult;
                HandleSignInResult(result);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == RC_SIGN_IN)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                HandleSignInResult(result);
            }
        }

        public void HandleSignInResult(GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                var acct = result.SignInAccount;
                GoogleInfo.GetInstance().Acount = acct;
                GoogleInfo.GetInstance().SignInApi = mGoogleApiClient;
                if (!mGoogleApiClient.IsConnected)
                {
                    mGoogleApiClient.Connect(GoogleApiClient.SignInModeOptional);
                    Xamarin.Forms.Application.Current.Properties["Boff"] = "Hello " + acct.DisplayName + "! \nWelcome to GreenBank";
                    Xamarin.Forms.Application.Current.Properties["First"] = acct.GivenName;
                    Xamarin.Forms.Application.Current.Properties["Last"] = acct.FamilyName;
                    Xamarin.Forms.Application.Current.Properties["Signed"] = true;



                }
                else {
                    Xamarin.Forms.Application.Current.Properties["Boff"] = "Hello " + acct.DisplayName + "! \nWelcome to GreenBank";
                    Xamarin.Forms.Application.Current.Properties["First"] = acct.GivenName;
                    Xamarin.Forms.Application.Current.Properties["Last"] = acct.FamilyName;
                    Xamarin.Forms.Application.Current.Properties["Signed"] = true;
                }
            }
            else {
                GoogleInfo.GetInstance().Result = result.Status.ToString();
                Xamarin.Forms.Application.Current.Properties["Boff"] = result.Status.ToString();
            }
        }

        public void SignIn()
        {
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(signInIntent, RC_SIGN_IN);
        }

        public void SignOut()
        {
           mGoogleApiClient.Disconnect();
            GoogleInfo.GetInstance().Acount = null;
            Xamarin.Forms.Application.Current.Properties["Signed"] = false;
            Xamarin.Forms.Application.Current.Properties["Boff"] = "Hello! Welcome to GreenBank\n Please log In with a Google Account";
        }

        void RevokeAccess()
        {
            Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {

        }

        protected override void OnStop()
        {
            base.OnStop();
            mGoogleApiClient.Disconnect();
        }

   

        public void HideProgressDialog()
        {
            //if (mProgressDialog != null && mProgressDialog.IsShowing)
           // {
              //  mProgressDialog.Hide();
            //}
        }

    }
}

