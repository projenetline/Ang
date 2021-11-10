using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using PazarYeri.Models.Settings;

namespace PazarYeri.BusinessLayer.Utility
{
    public class ProjectUtil
    {
        string cryptoKey = "*?netline?*-*";

        public net_ServerSettings getSetting()
        {
            net_ServerSettings app = new net_ServerSettings();

            try
            {
                string fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Settings.xml");
                XmlSerializer xs = new XmlSerializer(typeof(net_ServerSettings));
                bool exist = File.Exists(fileName);
                net_ServerSettings Crypto_app = new net_ServerSettings();
                FileStream read = null;
                if (exist)
                {
                    read = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    Crypto_app = (net_ServerSettings) xs.Deserialize(read);
                }

                read.Close();
                read.Dispose();

                app.DatabaseName = Crypter.Decrypt(Crypto_app.DatabaseName, cryptoKey);
                app.ServerName = Crypter.Decrypt(Crypto_app.ServerName, cryptoKey);
                app.UserName = Crypter.Decrypt(Crypto_app.UserName, cryptoKey);
                app.Password = Crypter.Decrypt(Crypto_app.Password, cryptoKey);
                app.LicenceKey = Crypter.Decrypt(Crypto_app.LicenceKey, cryptoKey);
                app.ControlTime = Crypto_app.ControlTime;
            }
            catch (Exception)
            {
            }

            return app;
        }

        public void SaveSettings(net_ServerSettings appSet)
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Settings.xml");

            var Crypto_app = new net_ServerSettings
            {
                DatabaseName = Crypter.Encrypt(appSet.DatabaseName, cryptoKey),
                ServerName = Crypter.Encrypt(appSet.ServerName, cryptoKey),
                UserName = Crypter.Encrypt(appSet.UserName, cryptoKey),
                Password = Crypter.Encrypt(appSet.Password, cryptoKey),
                LicenceKey = Crypter.Encrypt(appSet.LicenceKey, cryptoKey),
                ControlTime = appSet.ControlTime,
            };

            SaveXML.SaveData(Crypto_app, fileName);

            //  DatabaseLayer databaseLayer = new DatabaseLayer();
        }

        public string LogService(string content)
        {
            string rtrn = string.Empty;
            try
            {
                string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log";

                //  ApplicationSettings appSet = new ApplicationSettings();
                int gun = DateTime.Today.Day;
                int ay = DateTime.Today.Month;
                int yil = DateTime.Today.Year;

                //set up a filestream
                FileStream fs = new FileStream(
                    String.Format("{0}\\{1}-{2}-{3}-Pazaryeri_Log.txt", filePath, gun, ay, yil),
                    FileMode.OpenOrCreate,
                    FileAccess.Write);

                //set up a streamwriter for adding text
                StreamWriter sw = new StreamWriter(fs);

                //find the end of the underlying filestream
                sw.BaseStream.Seek(0, SeekOrigin.End);

                //add the text
                content = string.Format("{0} | {1}", DateTime.Now.ToLongTimeString(), content);
                sw.WriteLine(content);

                //add the text to the underlying filestream
                sw.Flush();

                //close the writer
                sw.Close();
            }
            catch (Exception ex)
            {
                rtrn = ex.Message;
            }

            return rtrn;
        }
    }
}