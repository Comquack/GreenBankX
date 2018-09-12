using System.IO;
using System.Threading.Tasks;

public interface ILogin
{
    //Method to save document as a file and view the saved document
    ILogin GetInstance();
    bool SignIn();
    string AccountName();
    bool UseDrive();
    void SignOut();
}

