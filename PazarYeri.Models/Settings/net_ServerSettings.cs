namespace PazarYeri.Models.Settings
{
    public class net_ServerSettings
    {
        public string ServerName { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LicenceKey { get; set; } = "";
        public int ControlTime { get; set; } = 0;
    }
}
