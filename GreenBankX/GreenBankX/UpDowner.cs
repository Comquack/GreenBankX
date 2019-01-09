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
using static Google.Apis.Drive.v3.FilesResource;

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
            Xamarin.Forms.Application.Current.Properties["Boff"] = "Downloading Files, Please Wait.";
            FilesResource bung = new FilesResource(driver);
            FileList jonny = bung.List().Execute();
            bool pr = false;
            bool tr = false;
            bool pl = false;

            // try{
            foreach (Google.Apis.Drive.v3.Data.File x in jonny.Files)
            {
                if (x.MimeType == "application/vnd.ms-excel")
                { 
                    if (x.Name == "Pricings.xls") {
                        Xamarin.Forms.Application.Current.Properties["Boff"] += "\nDownloading: " + x.Name;
                        pr = true;
                        StreamtoFile(x);
                    }
                    else if (x.Name == "Plots.xls")
                    {
                        Xamarin.Forms.Application.Current.Properties["Boff"] += "\nDownloading: " + x.Name;
                        tr = true;
                        StreamtoFile(x);
                    }
                    else if (x.Name == "trees.xls")
                    {
                        Xamarin.Forms.Application.Current.Properties["Boff"] += "\nDownloading: " + x.Name;
                        pl = true;
                        StreamtoFile(x);
                    
                    }
                }
            }
            if (!pl || !pr || !tr)
            {
                Xamarin.Forms.Application.Current.Properties["Boff"] = (pr ? "" : "Pricings.xls, ") + (pl ? "" : "Plots.xls, ") + (tr ? "" : "trees.xls, ") + "not found.";
            }
            // }
            //  catch(Exception e)
            // {
            //   Xamarin.Forms.Application.Current.Properties["Boff"] = e.GetBaseException().ToString();
            // }
        }
        public void UpList()
        {
            Xamarin.Forms.Application.Current.Properties["Boff"] = "Uploading Files, Please Wait.";
            FilesResource bung = new FilesResource(driver);
            ListRequest req = bung.List();
            req.Q = "'root' in parents and trashed=false";
            FileList jonny = req.Execute();

            // try{
            (bool, bool, bool) set = (false, false, false); 
            foreach (Google.Apis.Drive.v3.Data.File x in jonny.Files)
            { 
                if (x.MimeType == "application/vnd.ms-excel")
                {
                    if (x.Name == "Pricings.xls")
                    {
                        Xamarin.Forms.Application.Current.Properties["Boff"] += "\nUploading: " + x.Name;
                        set.Item1 = true;
                        FiletoStream(x, "Pricings.xls");
                    }
                    else if (x.Name == "Plots.xls")
                    {
                        Xamarin.Forms.Application.Current.Properties["Boff"] += "\nUploading: " + x.Name;
                        set.Item2 = true;
                        FiletoStream(x, "Plots.xls");
                    }
                    else if (x.Name == "trees.xls")
                    {
                        Xamarin.Forms.Application.Current.Properties["Boff"] += "\nUploading: " + x.Name;
                        set.Item3 = true;
                        FiletoStream(x, "trees.xls");
                    }
                }
            }
           CreateFiles(set);
        }
            public void StreamtoFile(Google.Apis.Drive.v3.Data.File x) {
            FilesResource bung = new FilesResource(driver);
            MemoryStream output = new MemoryStream();
            var stream = new System.IO.MemoryStream();
            bung.Get(x.Id).Download(stream);
            DependencyService.Get<ISave>().Save(x.Name, "application/vnd.ms-excel", stream);
            Application.Current.Properties["Boff"] += "\nDownloaded: " + x.Name;
        }

        public void FiletoStream(Google.Apis.Drive.v3.Data.File x, string name)
        { bool doesExist = System.IO.File.Exists(DependencyService.Get<ISave>().GetFileName() + "/" + name);
            if (doesExist)
            {
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/" + name, FileMode.Open);
                FilesResource bung = new FilesResource(driver);
                bung.Update(x, x.Id, inputStream, "application/vnd.ms-excel").Upload();
                //bung.Create(x,inputStream, "application/msexcel").Upload();
                Application.Current.Properties["Boff"] += "Uploaded: " + x.Name;
                inputStream.Close();
          
            }
        }

        public void CreateFiles((bool, bool, bool) set)
        {
            (string, string, string) names = ("Pricings.xls", "Plots.xls", "trees.xls");

            bool doesExist = System.IO.File.Exists(DependencyService.Get<ISave>().GetFileName() + "/" + names.Item1)&&!set.Item1;
            if (doesExist)
            {
                FilesResource bung = new FilesResource(driver);
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/" + names.Item1, FileMode.Open);
                Google.Apis.Drive.v3.Data.File up = new Google.Apis.Drive.v3.Data.File();
                up.MimeType = "application/vnd.ms-excel";
                up.Name = names.Item1;
                bung.Create(up, inputStream, "application/vnd.ms-excel").Upload();
                Application.Current.Properties["Boff"] += "\nUploaded: " + names.Item1;
                inputStream.Close();
                
            }
            doesExist = System.IO.File.Exists(DependencyService.Get<ISave>().GetFileName() + "/" + names.Item2) && !set.Item2;
            if (doesExist)
            {
                FilesResource bung = new FilesResource(driver);
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/" + names.Item2, FileMode.Open);
                Google.Apis.Drive.v3.Data.File up = new Google.Apis.Drive.v3.Data.File();
                up.MimeType = "application/vnd.ms-excel";
                up.Name = names.Item2;
                bung.Create(up, inputStream, "application/vnd.ms-excel").Upload();
                Application.Current.Properties["Boff"] += "\nUploaded: " + names.Item2;
                inputStream.Close();

            }
            doesExist = System.IO.File.Exists(DependencyService.Get<ISave>().GetFileName() + "/" + names.Item3) && !set.Item3;
            if (doesExist)
            {
                FilesResource bung = new FilesResource(driver);
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/" + names.Item3, FileMode.Open);
                Google.Apis.Drive.v3.Data.File up = new Google.Apis.Drive.v3.Data.File();
                up.MimeType = "application/vnd.ms-excel";
                up.Name = names.Item3;
                bung.Create(up, inputStream, "application/vnd.ms-excel").Upload();
                Application.Current.Properties["Boff"] += "\nUploaded: " + names.Item3;
                inputStream.Close();

            }
        }

    }
}
