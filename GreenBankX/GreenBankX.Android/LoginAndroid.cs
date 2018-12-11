using System;
using System.IO;
using Xamarin.Forms;
using System.Threading.Tasks;
using GreenBankX.Droid;
using Android.Gms.Drive;
using Android.Gms.Common.Apis;
using Android.Runtime;
using GreenBankX.Resources;
using System.Threading;

[assembly: Dependency(typeof(LoginAndroid))]

class LoginAndroid : Java.Lang.Object, ILogin, IResultCallback, IDriveApiDriveContentsResult, IDriveFileDownloadProgressListener
{
    FileStream inputStream;
    bool[] work = new bool[] { false, false, false};
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
            throw new Exception();
        }
        return GoogleInfo.GetInstance().Acount.DisplayName;
        
    }
    public string UseDrive(int select)
    {
        string Load = "";
        GoogleInfo.GetInstance().Upload = true;
        switch (select)
        {
            case -1:
                Xamarin.Forms.Application.Current.Properties["Boff"] = "Test";
               
                GoogleInfo.GetInstance().Files.Clear();
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
                GoogleInfo.GetInstance().FileName = "map.kml";
                break;
            case 4:
                GoogleInfo.GetInstance().Count = -1;
                Application.Current.Properties["Boff"] = "Finished";
                return "Finished";
        }
        if (inputStream != null)
        {
            inputStream.Dispose();
        }
        DriveClass.DriveApi.NewDriveContents(GoogleInfo.GetInstance().SignInApi).SetResultCallback(this);
        if (GoogleInfo.GetInstance().SignInApi.HasConnectedApi(DriveClass.API))
        {
            DriveClass.DriveApi.RequestSync(GoogleInfo.GetInstance().SignInApi);
            Application.Current.Properties["Boff"] = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Loading");
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
                if (work[0] && work[1] && work[2])
                {
                    Application.Current.Properties["Boff"] = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LoadingFin");
                }
                else {
                    Application.Current.Properties["Boff"] = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LoadFail") + ": " +(work[0]?"": "Pricings.xls, ") + (work[1] ? "" : "Plots.xls, ") + (work[2] ? "" : "trees.xls, ");
                }
                return "Finished";
                
        }
        if (inputStream != null)
        {
            inputStream.Dispose();
        }
        DriveClass.DriveApi.NewDriveContents(GoogleInfo.GetInstance().SignInApi).SetResultCallback(this);
        if (GoogleInfo.GetInstance().SignInApi.HasConnectedApi(DriveClass.API)) {
            Application.Current.Properties["Boff"] = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Loading");
            Load = "Loading";
        }
        else {
            Application.Current.Properties["Boff"] = "Error";
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
                    var folderMetaData = await DriveClass.DriveApi.GetRootFolder(GoogleInfo.GetInstance().SignInApi).ListChildrenAsync(GoogleInfo.GetInstance().SignInApi);
                    foreach (var driveItem in folderMetaData.MetadataBuffer)
                    {
                        GoogleInfo.GetInstance().Files.Add((driveItem.Title, driveItem.DriveId, driveItem.AlternateLink));
                        GoogleInfo.GetInstance().Trees++;

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
                else
                {
                    Download(GoogleInfo.GetInstance().Count);
                }
            });
        }
        else if (GoogleInfo.GetInstance().Upload)
        {
            bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/" + GoogleInfo.GetInstance().FileName);
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
                    string mime = "application/vnd.ms-excel";
                    if (GoogleInfo.GetInstance().Count == 3)
                    {
                        mime = "text/plain";
                    }
                    writer.Close();
                    MetadataChangeSet changeSet = new MetadataChangeSet.Builder()
                           .SetTitle(GoogleInfo.GetInstance().FileName)
                           .SetMimeType(mime)
                           .Build();
                    DriveClass.DriveApi
                              .GetRootFolder(GoogleInfo.GetInstance().SignInApi)
                              .CreateFile(GoogleInfo.GetInstance().SignInApi, changeSet, contentResults.DriveContents);
                    GoogleInfo.GetInstance().Count = GoogleInfo.GetInstance().Count + 1;
                    Application.Current.Properties["Boff"] = "Uploaded: " + GoogleInfo.GetInstance().FileName;

                    UseDrive(GoogleInfo.GetInstance().Count);
                });
            }
            else
            {
                Application.Current.Properties["Boff"] = "Not Found: " + GoogleInfo.GetInstance().FileName;
                GoogleInfo.GetInstance().Count++;
                UseDrive(GoogleInfo.GetInstance().Count);
            }
        }
        else
        {

            try { Application.Current.Properties["Boff"] = "Downloading: " + GoogleInfo.GetInstance().Files.Find(m => m.Item1 == GoogleInfo.GetInstance().FileName).Item1;
            var floop = GoogleInfo.GetInstance().Files.Find(m => m.Item1 == GoogleInfo.GetInstance().FileName).Item2;
            IDriveFile file = DriveClass.DriveApi.GetFile(GoogleInfo.GetInstance().SignInApi, floop);
            file.GetMetadata(GoogleInfo.GetInstance().SignInApi).SetResultCallback(MetadataRetrievedCallback());
            Task.Run(() =>
            {
                var driveContentsResult = file.Open(GoogleInfo.GetInstance().SignInApi,
                    DriveFile.ModeReadOnly, null).Await();
                IDriveContents driveContents = driveContentsResult.JavaCast<IDriveApiDriveContentsResult>().DriveContents;
                Application.Current.Properties["Boff"] = "Recieved " + GoogleInfo.GetInstance().Files.Find(m => m.Item1 == GoogleInfo.GetInstance().FileName).Item1;
                Stream inputstream = driveContents.InputStream;
                Application.Current.Properties["Boff"] = GoogleInfo.GetInstance().Files.Find(m => m.Item1 == GoogleInfo.GetInstance().FileName).Item1 + " Finished";
                byte[] buffer = new byte[16 * 1024];
                int read;
                MemoryStream output = new MemoryStream();

                while ((read = inputstream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, read);
                }
                Application.Current.Properties["Boff"] = output.Length.ToString();
                DependencyService.Get<ISave>().Save(GoogleInfo.GetInstance().FileName, "application/msexcel", output);

                GoogleInfo.GetInstance().Count = GoogleInfo.GetInstance().Count + 1;
                Application.Current.Properties["Boff"] = "Downloaded: " + GoogleInfo.GetInstance().FileName;

                if (GoogleInfo.GetInstance().Count == 3)
                {
                    Application.Current.Properties["Load"] = true;
                    work[GoogleInfo.GetInstance().Count - 1] = true;
                }
                else {
                    work[GoogleInfo.GetInstance().Count-1] = true;
                }
                Download(GoogleInfo.GetInstance().Count);
            });
            }
            catch
            {
                Task.Run(() =>
                {
                GoogleInfo.GetInstance().Count = GoogleInfo.GetInstance().Count + 1;
                Application.Current.Properties["Boff"] = "Failed to Load: " + GoogleInfo.GetInstance().FileName;   
                if (GoogleInfo.GetInstance().Count == 3)
                {
                    Application.Current.Properties["Load"] = true;
                }
                Download(GoogleInfo.GetInstance().Count);
                });
            }
        }
    }

    public IDriveContents DriveContents
    { get {throw new NotImplementedException();}}

    public Statuses Status
    { get {throw new NotImplementedException();}}

    public GoogleApiClient GapiClient { get; private set; }

    void IDriveFileDownloadProgressListener.OnProgress(long bytesDownloaded, long bytesExpected)
    { }
    private IResultCallback MetadataRetrievedCallback()
    { return null;}
}


