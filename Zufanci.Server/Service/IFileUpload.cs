namespace Zufanci.Server.Service
{
    public interface IFileUpload
    {
        string UploadFile(string imageBase64, string location);
        bool DeleteFile(string filePath, string location);
    }
}
