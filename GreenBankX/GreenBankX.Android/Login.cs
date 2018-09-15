using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Drive;
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
        public Android.Content.Res.AssetManager assets;
        public int Trees { get;  set; }
        public int Plots { get;  set; }
        public int Pricings { get; set; }
        public List<(string, DriveId, string)> Files { get; set; }
        public int Count { get; set; }
        public MainActivity bom;
        public static GoogleInfo instance;
        public string FileName { get; set; }
        public GoogleSignInAccount Acount{ get; set;}
        public string Result { get; set; }
        public GoogleApiClient SignInApi { get; set; }
        public bool Upload { get; set; }
        public int Up { get; set; }


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
            Count = -1;
            Files = new List<(string,DriveId,string)>();
            Trees = -1;
            Plots = -1;
            Pricings = -1;
        }

    }
}