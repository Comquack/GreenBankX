using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Google.SignIn;
using UIKit;
using TK.CustomMap;
using TK.CustomMap.iOSUnified;

namespace GreenBankX.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.FormsMaps.Init();
            TKCustomMapRenderer.InitMapRenderer();
            global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();
            Rg.Plugins.Popup.Popup.Init();
            LoadApplication(new App());

            var googleServiceDictionary = NSDictionary.FromFile("GoogleService-Info.plist");
            SignIn.SharedInstance.ClientID = googleServiceDictionary["CLIENT_ID"].ToString();

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            // Convert NSUrl to Uri
            var uri = new Uri(url.AbsoluteString);

            // Load redirectUrl page
            AuthenticationState.Authenticator.OnPageLoading(uri);

            return true;
        }
        // For iOS 9 or newer
        //public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        //{
        //    var openUrlOptions = new UIApplicationOpenUrlOptions(options);
        //    return SignIn.SharedInstance.HandleUrl(url, openUrlOptions.SourceApplication, openUrlOptions.Annotation);
        //}

        //// For iOS 8 and older
        //public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        //{
        //    return SignIn.SharedInstance.HandleUrl(url, sourceApplication, annotation);
        //}
        public void DidSignIn(SignIn signIn, GoogleUser user, NSError error)
        {
            if (user != null && error == null) { }
            Xamarin.Forms.Application.Current.Properties["Boff"] = "Is it working";
        }
    }
}
