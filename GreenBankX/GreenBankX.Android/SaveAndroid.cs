using System;
using System.IO;
//using GettingStarted.Droid;
using Android.Content;
using Java.IO;
using Xamarin.Forms;
using System.Threading.Tasks;


[assembly: Dependency(typeof(SaveAndroid))]

class SaveAndroid: ISave
    {
        //Method to save document as a file in Android and view the saved document
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task SaveAndView(string fileName, String contentType, MemoryStream stream)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            string root = null;
            //Get the root path in android device.
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.OS.Environment.ExternalStorageDirectory.ToString();
            }
            else
                root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string documentsPath = Environment.CurrentDirectory;

        //Create directory and file 
        Application.Current.Properties["savename"] = root + "/GreenBankX";
        Java.IO.File myDir = new Java.IO.File(root + "/GreenBankX");
            myDir.Mkdir();

            Java.IO.File file = new Java.IO.File(myDir, fileName);

            //Remove if the file exists
            if (file.Exists()) file.Delete();

            //Write the stream into the file
            FileOutputStream outs = new FileOutputStream(file);
            outs.Write(stream.ToArray());

            outs.Flush();
            outs.Close();

            //Invoke the created file for viewing
            if (file.Exists())
            {
                Android.Net.Uri path = Android.Net.Uri.FromFile(file);
                string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
                string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(path, mimeType);
                Forms.Context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            }
        }

    public async Task Save(string fileName, String contentType, MemoryStream stream)
    {
        string root = null;
        //Get the root path in android device.
        if (Android.OS.Environment.IsExternalStorageEmulated)
        {
            root = Android.OS.Environment.ExternalStorageDirectory.ToString();
        }
        else
            root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string documentsPath = Environment.CurrentDirectory;

        //Create directory and file 
        Application.Current.Properties["savename"] = root + "/GreenBankX";
        Java.IO.File myDir = new Java.IO.File(root + "/GreenBankX");
        myDir.Mkdir();

        Java.IO.File file = new Java.IO.File(myDir, fileName);

        //Remove if the file exists
        if (file.Exists()) file.Delete();

        //Write the stream into the file
        FileOutputStream outs = new FileOutputStream(file);
        outs.Write(stream.ToArray());

        outs.Flush();
        outs.Close();
    }
    public string GetFileName()
    {
        string root = null;
        //Get the root path in android device.
        if (Android.OS.Environment.IsExternalStorageEmulated)
        {
            root = Android.OS.Environment.ExternalStorageDirectory.ToString();
        }
        else
            root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string documentsPath = Environment.CurrentDirectory;

        //Create directory and file 
        return root + "/GreenBankX";
    }
    }
