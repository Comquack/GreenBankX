using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GreenBankX.Droid
{
    class Login
    {
    }
    class GoogleInfo
    {
        public MainActivity bom;
        public static GoogleInfo instance;
        public GoogleSignInAccount Acount{ get; set;}
        public static GoogleInfo GetInstance(MainActivity bob)
        {
            if (instance == null)
            {
                instance = new GoogleInfo(bob);
                return instance;
            }
            return instance;
        }
        public static GoogleInfo GetInstance()
        {
            if (instance == null)
            {
            }
            return instance;
        }
        private GoogleInfo(MainActivity bob) {
            bom = bob;
        }

    }
}