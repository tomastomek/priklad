namespace Zufanci.Server.Helpers
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public string EmailPassword { get; set; } = string.Empty;
    }
}
