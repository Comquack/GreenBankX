using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Drive.v3;

namespace GreenBankX
{
    public static class Constants
    {
        public static string AppName = "com.companyname.GreenBankX";

        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string iOSClientId = "263109938909-bts0mgt2859gv9btr2h9ep36fqtk31dh.apps.googleusercontent.com";
        public static string AndroidClientId = "189021053893-9ag4ogqenm1qsliouip4lmqgthr42bnm.apps.googleusercontent.com";

        // These values do not need changing
        public static string scopes = "https://www.googleapis.com/auth/userinfo.email "+DriveService.Scope.Drive+" "+DriveService.Scope.DriveFile+" "+DriveService.Scope.DriveMetadata;
        public static string[] scopearray = new string[] {"https://www.googleapis.com/auth/userinfo.email"  ,DriveService.Scope.Drive,
        DriveService.Scope.DriveFile, DriveService.Scope.DriveMetadata};
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string iOSRedirectUrl = "com.googleusercontent.apps.263109938909-bts0mgt2859gv9btr2h9ep36fqtk31dh:/oauth2redirect";
        public static string AndroidRedirectUrl = "com.googleusercontent.apps.263109938909-v6r1cu813081jujunosjadmhc3nr67kk:/oauth2redirect";
        internal static string iOSClientSecret;
    }
}
