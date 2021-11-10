using PazarYeri.Models.Settings;
using System;
using UnityObjects;

namespace PazarYeri.BusinessLayer.Utility
{
    public class Global
    {
        public static UnityApplication application = new UnityApplication();
        static DatabaseLayer databaseLayer = new DatabaseLayer();
        public static bool Login(out string result)
        {
            result = "";
            bool loggedin = false;
            net_LogoSettings logoSettings = databaseLayer.GetLogoSettings();    
            if (application.Login(logoSettings.LogoUserName, logoSettings.LogoPassword, logoSettings.FirmNr, logoSettings.PeriodNr))
            {
                result = "Logo ile bağlantı sağlandı.";
                loggedin = true;
            }
            else
            {
                result = "Logo ile bağlantı hatası!!! " + application.GetLastErrorString();
            }

            return loggedin;
        }



    }
}
