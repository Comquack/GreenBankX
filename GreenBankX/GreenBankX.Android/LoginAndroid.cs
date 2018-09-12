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

[assembly: Dependency(typeof(LoginAndroid))]

class LoginAndroid : Java.Lang.Object, ILogin, IResultCallback, IDriveApiDriveContentsResult
// class LoginAndroid : Java.Lang.Object, ILogin
{
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
    public bool UseDrive()
    {    
        DriveClass.DriveApi.NewDriveContents(GoogleInfo.GetInstance().SignInApi).SetResultCallback(this);
       return GoogleInfo.GetInstance().SignInApi.HasConnectedApi(DriveClass.API);
    }
    void IResultCallback.OnResult(Java.Lang.Object result)
    {
        var contentResults = (result).JavaCast<IDriveApiDriveContentsResult>();
        if (!contentResults.Status.IsSuccess) // handle the error
            return;
 
        Task.Run(() =>
        {
            var writer = new OutputStreamWriter(contentResults.DriveContents.OutputStream);
            writer.Write("St00ck Overflow");
            writer.Close();
            MetadataChangeSet changeSet = new MetadataChangeSet.Builder()
                   .SetTitle("New Text File")
                   .SetMimeType("text/plain")
                   .Build();
            DriveClass.DriveApi
                      .GetRootFolder(GoogleInfo.GetInstance().SignInApi)
                      .CreateFile(GoogleInfo.GetInstance().SignInApi, changeSet, contentResults.DriveContents);
        });
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


}
