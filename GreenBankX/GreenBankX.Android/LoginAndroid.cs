using System;
using System.IO;
//using GettingStarted.Droid;
using Android.Content;
using Java.IO;
using Xamarin.Forms;
using System.Threading.Tasks;
using GreenBankX.Droid;
using Android.Gms.Drive;
using Android.App;
using Android.Gms.Common.Apis;
using Android.Runtime;
using Google.Apis.Drive.v3;
using System.Linq;
using Google.Apis.Download;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.Collections.Generic;

[assembly: Dependency(typeof(LoginAndroid))]

class LoginAndroid : Java.Lang.Object, ILogin, IResultCallback, IDriveApiDriveContentsResult, IDriveFileDownloadProgressListener
// class LoginAndroid : Java.Lang.Object, ILogin
{
    FileStream inputStream;
    public static LoginAndroid instance = new LoginAndroid();
    public ILogin GetInstance()
    {
        if (instance == null)
        {
            return new LoginAndroid();
        }
        return instance;
    }
   public bool SignIn() {
         GoogleInfo.GetInstance().bom.SignIn();
        return true;

    }
    public void SignOut() {
        GoogleInfo.GetInstance().bom.SignOut();
    }
    public string AccountName() {
        if (GoogleInfo.GetInstance().Acount.DisplayName == null) {
            return GoogleInfo.GetInstance().Acount.ToString();
        }
        return GoogleInfo.GetInstance().Acount.DisplayName;
        
    }
    public string UseDrive(int select)
    {
        string Load = "";
        GoogleInfo.GetInstance().Upload = true;
        GoogleInfo.GetInstance().Up = 0;
        switch (select)
        {
            case -1:
                Xamarin.Forms.Application.Current.Properties["Boff"] = "Test";
                GoogleInfo.GetInstance().Count = -1;
                break;
            case 0:
                GoogleInfo.GetInstance().FileName = "Pricings.xls";
                break;
            case 1:
                GoogleInfo.GetInstance().FileName = "Plots.xls";
                break;
            case 2:
                GoogleInfo.GetInstance().FileName = "trees.xls";
                break;
            case 3:
                GoogleInfo.GetInstance().Count = -1;
                return "Finished";
        }
        if (inputStream != null)
        {
            inputStream.Dispose();
        }
        DriveClass.DriveApi.NewDriveContents(GoogleInfo.GetInstance().SignInApi).SetResultCallback(this);
        if (GoogleInfo.GetInstance().SignInApi.HasConnectedApi(DriveClass.API))
        {
            Xamarin.Forms.Application.Current.Properties["Boff"] = "Loading";
            Load = "Loading";
        }
        else
        {
            Load = "Error";
        }
        return Load;
    }
    public string Download(int select)
    {
        GoogleInfo.GetInstance().Upload = false;
        string Load="";
        GoogleInfo.GetInstance().Up = 0;
        switch (select)
        {
            case -1:
                GoogleInfo.GetInstance().Count = -1;
                GoogleInfo.GetInstance().Files.Clear();
                break;
            case 0:
                GoogleInfo.GetInstance().FileName = "Pricings.xls";
                break;
            case 1:
                GoogleInfo.GetInstance().FileName = "Plots.xls";
                break;
            case 2:
                GoogleInfo.GetInstance().FileName = "trees.xls";
                break;
            case 3:
                GoogleInfo.GetInstance().Count = -1;
                Xamarin.Forms.Application.Current.Properties["Boff"]= "Finished";
                return "Finished";
                
        }
        if (inputStream != null)
        {
            inputStream.Dispose();
        }
        DriveClass.DriveApi.NewDriveContents(GoogleInfo.GetInstance().SignInApi).SetResultCallback(this);
        if (GoogleInfo.GetInstance().SignInApi.HasConnectedApi(DriveClass.API)) {
            Xamarin.Forms.Application.Current.Properties["Boff"] = "Loading";
            Load = "Loading";
        }
        else {
            Xamarin.Forms.Application.Current.Properties["Boff"] = "Error";
           Load = "Error";
        }
       return Load;
    }
    void IResultCallback.OnResult(Java.Lang.Object result)
    {
        var contentResults = (result).JavaCast<IDriveApiDriveContentsResult>();
        if (!contentResults.Status.IsSuccess) // handle the error
            return;
        if (GoogleInfo.GetInstance().Count == -1)
        {
            GoogleInfo.GetInstance().Plots = -1;
            GoogleInfo.GetInstance().Pricings = -1;
            GoogleInfo.GetInstance().Trees = -1;
            Task.Run(async () =>
            {
            async Task GetFolderMetaData(IDriveFolder folder, int depth)
                {
                    //DriveClass.DriveApi.RequestSync(GoogleInfo.GetInstance().SignInApi).Await();      
                    var folderMetaData = await DriveClass.DriveApi.GetRootFolder(GoogleInfo.GetInstance().SignInApi).ListChildrenAsync(GoogleInfo.GetInstance().SignInApi);
                    foreach (var driveItem in folderMetaData.MetadataBuffer)
                    {
                      //  if (driveItem.Title == "trees.xls" && GoogleInfo.GetInstance().Trees == -1 && !driveItem.IsTrashed)
                       // {
                       
                            GoogleInfo.GetInstance().Files.Add((driveItem.Title, driveItem.DriveId, driveItem.AlternateLink));
                            GoogleInfo.GetInstance().Trees++;
                       // }
                       // if (driveItem.Title == "Plots.xls" && GoogleInfo.GetInstance().Plots == -1 && !driveItem.IsTrashed)
                       // {
                         //   GoogleInfo.GetInstance().Files.Add((driveItem.Title, driveItem.DriveId));
                          //  GoogleInfo.GetInstance().Plots++;
                       // }
                       // if (driveItem.Title == "Pricings.xls" && GoogleInfo.GetInstance().Pricings == -1 && !driveItem.IsTrashed)
                       // {
                         //   GoogleInfo.GetInstance().Files.Add((driveItem.Title, driveItem.DriveId));
                           // GoogleInfo.GetInstance().Pricings++;
                        //}
                        if (driveItem.IsFolder)
                        await GetFolderMetaData(driveItem.DriveId.AsDriveFolder(), depth + 1);
                }
                }
                await GetFolderMetaData(DriveClass.DriveApi.GetAppFolder(GoogleInfo.GetInstance().SignInApi), 0);
            
            GoogleInfo.GetInstance().Count = 0;
            if (GoogleInfo.GetInstance().Upload)
            {
                UseDrive(GoogleInfo.GetInstance().Count);
            }
            else {
                Download(GoogleInfo.GetInstance().Count);
            }
            });
        }else if (GoogleInfo.GetInstance().Upload)
        {
            bool doesExist = System.IO.File.Exists(DependencyService.Get<ISave>().GetFileName() + "/" + GoogleInfo.GetInstance().FileName);
            if (doesExist)
            {
                inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/" + GoogleInfo.GetInstance().FileName, FileMode.Open);
                Task.Run(() =>
                {
                    try
                    {
                        IDriveFile repeat = GoogleInfo.GetInstance().Files.Find(m => m.Item1 == GoogleInfo.GetInstance().FileName).Item2.AsDriveFile();
                        repeat.Delete(GoogleInfo.GetInstance().SignInApi);
                    }
                    catch { }

                    var writer = new OutputStreamAdapter(contentResults.DriveContents.OutputStream);
                    for (int x = 0; x < inputStream.Length; x++)
                    {
                        writer.Write(inputStream.ReadByte());
                    }
                    writer.Close();
                    MetadataChangeSet changeSet = new MetadataChangeSet.Builder()
                           .SetTitle(GoogleInfo.GetInstance().FileName)
                           .SetMimeType("application/vnd.ms-excel")
                           .Build();
                    DriveClass.DriveApi
                              .GetRootFolder(GoogleInfo.GetInstance().SignInApi)
                              .CreateFile(GoogleInfo.GetInstance().SignInApi, changeSet, contentResults.DriveContents);
                    GoogleInfo.GetInstance().Count = GoogleInfo.GetInstance().Count + 1;
                    Xamarin.Forms.Application.Current.Properties["Boff"] = "Uploaded: " + GoogleInfo.GetInstance().FileName;
                    UseDrive(GoogleInfo.GetInstance().Count);
                });
            }
            else {
                Xamarin.Forms.Application.Current.Properties["Boff"] = "Not Found: " + GoogleInfo.GetInstance().FileName;
                UseDrive(GoogleInfo.GetInstance().Count);
            }
        }
        else {

            try { Xamarin.Forms.Application.Current.Properties["Boff"] = "Downloading: "+ GoogleInfo.GetInstance().Files.Find(m => m.Item1 == GoogleInfo.GetInstance().FileName).Item3.ToString(); }
            catch { Xamarin.Forms.Application.Current.Properties["Boff"] = "Fail"; }
             var floop = GoogleInfo.GetInstance().Files.Find(m => m.Item1 == GoogleInfo.GetInstance().FileName).Item2;
           //IDriveFile bob =  DriveClass.DriveApi.GetFile(GoogleInfo.GetInstance().SignInApi, floop);
            IDriveFile file = DriveClass.DriveApi.GetFile(GoogleInfo.GetInstance().SignInApi, floop);
            file.GetMetadata(GoogleInfo.GetInstance().SignInApi).SetResultCallback(metadataRetrievedCallback());
            Task.Run(() =>
            {
                Xamarin.Forms.Application.Current.Properties["Boff"] = "shoop";
                var driveContentsResult = file.Open(GoogleInfo.GetInstance().SignInApi,
                    DriveFile.ModeReadOnly, null).Await();
                Xamarin.Forms.Application.Current.Properties["Boff"] = driveContentsResult.ToString();
                IDriveContents driveContents = driveContentsResult.JavaCast<IDriveApiDriveContentsResult>().DriveContents;
                Xamarin.Forms.Application.Current.Properties["Boff"] = "Adoop";
                Stream inputstream = driveContents.InputStream;
                Xamarin.Forms.Application.Current.Properties["Boff"] = "Finito";
                byte[] buffer = new byte[16 * 1024];
                int read;
                MemoryStream output = new MemoryStream();
               
                while ((read = inputstream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, read);
                }
                Xamarin.Forms.Application.Current.Properties["Boff"] = output.Length.ToString();
                Xamarin.Forms.DependencyService.Get<ISave>().Save(GoogleInfo.GetInstance().FileName, "application/msexcel", output);
                GoogleInfo.GetInstance().Count = GoogleInfo.GetInstance().Count + 1;
                Xamarin.Forms.Application.Current.Properties["Boff"] = "Downloaded: " + GoogleInfo.GetInstance().FileName;
                Download(GoogleInfo.GetInstance().Count);
            });





    }
    }

    public int count() {
        return GoogleInfo.GetInstance().Count;
    }
    public IDriveContents DriveContents
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public Statuses Status
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public GoogleApiClient GapiClient { get; private set; }

    public bool ListFiles() {
        GoogleInfo.GetInstance().Count = -1;
        return true;
    }

    void IDriveFileDownloadProgressListener.OnProgress(long bytesDownloaded, long bytesExpected)
    {
        //Xamarin.Forms.Application.Current.Properties["Boff"] = bytesDownloaded.ToString() + "out of " + bytesExpected.ToString();
    }
    private IResultCallback metadataRetrievedCallback()
    {
        void onResult(IDriveApiMetadataBufferResult result)
        {
            if (!result.Status.IsSuccess)
            {
                return ;
            }
            //metadata = result.getMetadata();

        }
        return null;
    }
}


