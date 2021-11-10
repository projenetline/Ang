namespace PazarYeri.Models.Settings
{
    public class net_EntegrationSettings
    {
        public int Id { get; set; } = 0;
        public string EntegrationName { get; set; } = string.Empty;
        public string FirmCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PassWord { get; set; } = string.Empty;
        public bool Excel { get; set; } = false;
        public bool WebService { get; set; } = false;
        
    }
}