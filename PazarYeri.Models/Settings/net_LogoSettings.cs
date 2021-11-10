namespace PazarYeri.Models.Settings
{
    public class net_LogoSettings
    {
        public int Id { get; set; } = 0;
        public string LogoServerName { get; set; } = string.Empty;
        public string LogoDatabase { get; set; } = string.Empty;
        public string LogoUserName { get; set; } = string.Empty;
        public string LogoPassword { get; set; } = string.Empty;
        public int FirmNr { get; set; } = 0;
        public int PeriodNr { get; set; } = 0;
        public bool AutoTransfer { get; set; } = false;
        
    }
}
