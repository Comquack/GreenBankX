using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Xamarin.Auth;
using Xamarin.Forms;

namespace GreenBankX
{
    class UpDowner
    {
        public TokenResponse token;
        public Account acca;
        public string clientId;
        public string redirectUri;
        public static UpDowner instance;
        DriveService driver { get; set; }
        private UpDowner() {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constants.iOSClientId;
                    redirectUri = Constants.iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = Constants.AndroidClientId;
                    redirectUri = Constants.AndroidRedirectUrl;
                    break;
            }
        }
        public void  Tokenise() {
            acca = (Account)Application.Current.Properties["Account"];

            token = null;
            //  try
            //{
            token = new TokenResponse()
            {
                AccessToken = acca.Properties["access_token"],
                ExpiresInSeconds = Convert.ToInt64(acca.Properties["expires_in"]),
                IdToken = acca.Properties["id_token"],
                RefreshToken = acca.Properties["refresh_token"],
                Scope = acca.Properties["scope"],
                TokenType = acca.Properties["token_type"],
            };
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = clientId
                }
            });
            var cred = new UserCredential(flow, "", token);
            driver = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
                ApplicationName = Constants.AppName
            });
        }
        public static UpDowner GetInstance()
        {
            if (instance == null)
            {
                instance = new UpDowner();
                return instance;
            }
            return instance;
        }
        public void FilesList() {
            Xamarin.Forms.Application.Current.Properties["Boff"] = "";
            FilesResource bung = new FilesResource(driver);
            FileList jonny = bung.List().Execute();
           
           // try{
                foreach (Google.Apis.Drive.v3.Data.File x in jonny.Files)
            {
                if (x.MimeType == "application/vnd.ms-excel")
                { 
                    if (x.Name == "Pricings.xls") {
                        Xamarin.Forms.Application.Current.Properties["Boff"] += "\nDownloading: " + x.Name;
                        StreamtoFile(x);
                    }
                    else if (x.Name == "Plots.xls")
                    {
                        Xamarin.Forms.Application.Current.Properties["Boff"] += "\nDownloading: " + x.Name;
                        StreamtoFile(x);
                    }
                    else if (x.Name == "trees.xls")
                    {
                        Xamarin.Forms.Application.Current.Properties["Boff"] += "\nDownloading: " + x.Name;
                        StreamtoFile(x);
                    
                    }
                }
            }
           // }
          //  catch(Exception e)
            // {
            //   Xamarin.Forms.Application.Current.Properties["Boff"] = e.GetBaseException().ToString();
            // }
        }
        public void StreamtoFile(Google.Apis.Drive.v3.Data.File x) {
            FilesResource bung = new FilesResource(driver);
            MemoryStream output = new MemoryStream();
            var stream = new System.IO.MemoryStream();
            bung.Get(x.Id).Download(stream);
            DependencyService.Get<ISave>().Save(x.Name, "application/msexcel", stream);
            Application.Current.Properties["Boff"] += "Downloaded: " + x.Name;
        }

    }
}
