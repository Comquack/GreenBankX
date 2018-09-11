using System;
using System.IO;
//using GettingStarted.Droid;
using Android.Content;
using Java.IO;
using Xamarin.Forms;
using System.Threading.Tasks;
using GreenBankX.Droid;

[assembly: Dependency(typeof(LoginAndroid))]

class LoginAndroid: ILogin
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
    public string AccountName() {
        return GoogleInfo.GetInstance().Acount.DisplayName;
        
    }
    public void UseDrive()
    {


    }

}
