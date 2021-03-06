﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GreenBankX.Droid
{
        [Activity(Label = "CustomUrlSchemeInterceptorActivity")]
    //    [IntentFilter(
    //new[] { Intent.ActionView },
    //Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    //DataSchemes = new[] { "com.googleusercontent.apps.263109938909-v6r1cu813081jujunosjadmhc3nr67kk" },
    //DataPath = "/oauth2redirect")]
        public class CustomUrlSchemeInterceptorActivity : Activity
        {
            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);

                // Convert Android.Net.Url to Uri
                var uri = new Uri(Intent.Data.ToString());

                // Load redirectUrl page
                AuthenticationState.Authenticator.OnPageLoading(uri);
                
                Finish();
            }
        }
    }
