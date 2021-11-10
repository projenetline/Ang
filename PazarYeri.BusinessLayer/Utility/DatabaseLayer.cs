using Dapper;
using PazarYeri.BusinessLayer.Helpers;
using PazarYeri.Models;
using PazarYeri.Models.Settings;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using PazarYeri.Models.Common;

namespace PazarYeri.BusinessLayer.Utility
{
    public class DatabaseLayer
    {
        private readonly ProjectUtil _util = new ProjectUtil();
        private readonly SqlConnectionStringBuilder _sqlStr;
        private readonly SqlConnectionStringBuilder _logoSqlStr;

        private readonly string _cryptoKey = "*?netline?*-*";
        private readonly string _logoFirmNo = "";
        private readonly string _logoPeriodNr = "";

        public DatabaseLayer()
        {
            var settings = _util.getSetting();

            _sqlStr = new SqlConnectionStringBuilder
            {
                DataSource = settings.ServerName, InitialCatalog = settings.DatabaseName, UserID = settings.UserName,
                Password = settings.Password
            };

            var logoSettings = GetLogoSettings();

            if (logoSettings != null)
            {
                _logoSqlStr = new SqlConnectionStringBuilder
                {
                    DataSource = logoSettings.LogoServerName,
                    InitialCatalog = logoSettings.LogoDatabase,
                    UserID = settings.UserName,
                    Password = settings.Password
                };
                _logoFirmNo = logoSettings.FirmNr.ToString().PadLeft(3, '0');
                _logoPeriodNr = logoSettings.PeriodNr.ToString().PadLeft(2, '0');
            }

            ReNameColumnNames();

            AddColumns();

            DropColumns();
        }

        private void DropColumns()
        {
            var list = new Dictionary<string, string>()
            {
                {"Specode", "net_EntegrationSettings"},
                {"PaymentCode", "net_EntegrationSettings"},
                {"ArpCodeShpm", "net_EntegrationSettings"},
                {"ArpCode", "net_EntegrationSettings"},
                {"AddRowsIfOrder", "net_LogoTransferSettings"},
                {"RetransferTransferedOrder", "net_LogoTransferSettings"},
            };

            var query = Properties.Resources.DropColumn;

            using (var sqlConnection = new SqlConnection(_sqlStr.ToString()))
            {
                foreach (var item in list)
                {
                    try
                    {
                        sqlConnection.Execute(
                            query,
                            new
                            {
                                @TableName = item.Value,
                                @ColumnName = item.Key
                            });
                    }
                    catch (Exception e)
                    {
                        _util.LogService("[ERROR] (DropColumns)" + Environment.NewLine + e.Message);
                    }
                }
            }
        }

        private void AddColumns()
        {
            var tableName = "net_LogoTransferSettings";

            var addColumnList = new Dictionary<string, string>()
            {
                {"ClientTransferSpeCode1", "NVARCHAR(50)"},
                {"ClientTransferSpeCode2", "NVARCHAR(50)"},
                {"ClientTransferSpeCode3", "NVARCHAR(50)"},
                {"ClientTransferSpeCode4", "NVARCHAR(50)"},
                {"ClientTransferSpeCode5", "NVARCHAR(50)"},
                {"ClientTransferAccountCode", "NVARCHAR(50)"},
                {"ClientTransferPaymentCode", "NVARCHAR(50)"},
                {"ClientTransferTradingGroup", "NVARCHAR(50)"},
                {"ClientTransferProjectCode", "NVARCHAR(50)"},
                {"ClientTransferAuthCode", "NVARCHAR(50)"},
                {"TransferFicheProjectCode", "NVARCHAR(50)"},
                {"OrderTransferDiscountCouponCode", "NVARCHAR(50)"},
                {"OrderTransferLateChargeCode", "NVARCHAR(50)"},
                {"OrderTransferSpeCode", "NVARCHAR(50)"},
                {"OrderTransferPaymentCode", "NVARCHAR(50)"},
                {"OrderTransferArpShippmentCode", "NVARCHAR(50)"},
                {"OrderTransferAuthCode", "NVARCHAR(50)"},
                {"OrderTransferArpCode", "NVARCHAR(50)"},
                {"OrderTransferRetransferTransferedOrder", "INT"},
                {"OrderTransferAddRowsIfOrder", "INT"},
                {"OrderTransferGroupByOrderNumber", "INT"},
                {"OrderTransferUnitPriceRoundingNumberOfDigits", "INT"},
                {"OrderTransferStatus", "INT"},
                {"OrderTransferTransferToShippingAddress", "INT"},
            };

            var query = Properties.Resources.AddColumn;

            using (var sqlConnection = new SqlConnection(_sqlStr.ToString()))
            {
                foreach (var column in addColumnList)
                {
                    try
                    {
                        sqlConnection.Execute(
                            query,
                            new
                            {
                                @TableName = tableName,
                                @NewColumnName = column.Key,
                                @DataType = column.Value
                            });
                    }
                    catch (Exception e)
                    {
                        _util.LogService("[ERROR] (AddColumns)" + Environment.NewLine + e.Message);
                    }
                }
            }

            tableName = "net_LogoTransferSettings";

            addColumnList = new Dictionary<string, string>()
            {
                {"ClientTransferSpeCode1", "NVARCHAR(50)"},
            };

            query = Properties.Resources.AddColumn;

            using (var sqlConnection = new SqlConnection(_sqlStr.ToString()))
            {
                foreach (var column in addColumnList)
                {
                    try
                    {
                        sqlConnection.Execute(
                            query,
                            new
                            {
                                @TableName = tableName,
                                @NewColumnName = column.Key,
                                @DataType = column.Value
                            });
                    }
                    catch (Exception e)
                    {
                        _util.LogService("[ERROR] (AddColumns)" + Environment.NewLine + e.Message);
                    }
                }
            }
        }

        private void ReNameColumnNames()
        {
            var columnsForNetLogoTransferSettings = new Dictionary<string, string>()
            {
                {"ClPrefix", "ClientTransferPrefixCode"},
                {"OrfDocode", "OrderTransferDoCode"},
                {"PrjCode", "OrderTransferProjectCode"},
                {"SlsCode", "OrderTransferSalesManCode"},
                {"ClTradingGroup", "OrderTransferTradingGroup"},
                {"SrvCode", "OrderTransferServiceChargeCode"},
                {"WhouseNr", "OrderTransferWareHouseNr"},
                {"DivisionNr", "OrderTransferDivisionNr"},
                {"AddRowsIfOrder", "OrderTransferAddRowsIfOrder"},
                {"RetransferTransferedOrder", "OrderTransferRetransferTransferedOrder"},
            };

            var query = Properties.Resources.UpdateColumns;

            const string tableName = "net_LogoTransferSettings";

            using (var sqlConnection = new SqlConnection(_sqlStr.ToString()))
            {
                foreach (var column in columnsForNetLogoTransferSettings)
                {
                    try
                    {
                        sqlConnection.Execute(
                            query,
                            new
                            {
                                @TableName = tableName,
                                @OldColumnName = column.Key,
                                @NewColumnName = column.Value
                            });
                    }
                    catch (Exception e)
                    {
                        _util.LogService("[ERROR] (ReNameColumnNames)" + Environment.NewLine + e.Message);
                    }
                }
            }
        }

        public bool saveEntegrationSettings(net_EntegrationSettings entegrationSettings)
        {
            var saved = false;
            entegrationSettings.PassWord = Crypter.Encrypt(entegrationSettings.PassWord, _cryptoKey);

            var query = Properties.Resources.saveEntegrationSettings;

            var sql = new SqlConnection(_sqlStr.ToString());
            var effectedRow = sql.Execute(
                query,
                new
                {
                    @Id = entegrationSettings.Id,
                    @EntegrationName = entegrationSettings.EntegrationName,
                    @FirmCode = entegrationSettings.FirmCode,
                    @UserName = entegrationSettings.UserName,
                    @PassWord = entegrationSettings.PassWord,
                    @Excel = entegrationSettings.Excel,
                    @WebService = entegrationSettings.WebService,
                });

            if (effectedRow > 0)
                saved = true;

            return saved;
        }

        public bool saveLogoSettings(net_LogoSettings netLogoSettings)
        {
            var saved = false;
            netLogoSettings.LogoPassword = Crypter.Encrypt(netLogoSettings.LogoPassword, _cryptoKey);

            var query = Properties.Resources.saveLogoSettings;

            var sql = new SqlConnection(_sqlStr.ToString());
            var effectedRow = sql.Execute(
                query,
                new
                {
                    @LogoServerName = netLogoSettings.LogoServerName,
                    @LogoDatabase = netLogoSettings.LogoDatabase,
                    @LogoUserName = netLogoSettings.LogoUserName,
                    @LogoPassword = netLogoSettings.LogoPassword,
                    @FirmNr = netLogoSettings.FirmNr,
                    @PeriodNr = netLogoSettings.PeriodNr,
                    @AutoTransfer = netLogoSettings.AutoTransfer
                });

            if (effectedRow > 0)
                saved = true;

            return saved;
        }

        public bool saveLogoTransferSettings(net_LogoTransferSettings net_LogoSettings)
        {
            var saved = false;
            var query = Properties.Resources.saveLogoTransferSettings;

            var sql = new SqlConnection(_sqlStr.ToString());

            var effectedRow = sql.Execute(query, net_LogoSettings);

            if (effectedRow > 0)
                saved = true;

            return saved;
        }

        public net_EntegrationSettings GetEntegrationSettings(string entegrationName)
        {
            var settings = new net_EntegrationSettings();
            try
            {
                if (!string.IsNullOrEmpty(_sqlStr.DataSource))
                {
                    var sql = new SqlConnection(_sqlStr.ToString());

                    var query = Properties.Resources.GetEntegrationSettings;

                    settings = sql.Query<net_EntegrationSettings>(
                        query,
                        new {@entegrationName = entegrationName}).FirstOrDefault();
                    if (settings != null)
                    {
                        settings.PassWord = Crypter.Decrypt(settings.PassWord, _cryptoKey);
                    }
                }
            }
            catch (Exception)
            {
                settings = new net_EntegrationSettings();
            }

            return settings;
        }

        public net_LogoTransferSettings GetLogoTransferSettings(string entegrationName)
        {
            var settings = new net_LogoTransferSettings();
            try
            {
                if (!string.IsNullOrEmpty(_sqlStr.DataSource))
                {
                    var sql = new SqlConnection(_sqlStr.ToString());

                    var query = Properties.Resources.GetLogoTransferSettings;

                    settings = sql.Query<net_LogoTransferSettings>(
                        query,
                        new {entegrationName}).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                settings = new net_LogoTransferSettings();
            }

            return settings;
        }

        public net_LogoSettings GetLogoSettings()
        {
            var settings = new net_LogoSettings();
            try
            {
                if (!string.IsNullOrEmpty(_sqlStr.DataSource))
                {
                    var sql = new SqlConnection(_sqlStr.ToString());

                    var query = Properties.Resources.GetLogoSettings;

                    settings = sql.Query<net_LogoSettings>(query).FirstOrDefault();

                    if (settings != null)
                    {
                        settings.LogoPassword = Crypter.Decrypt(settings.LogoPassword, _cryptoKey);
                    }
                }
            }
            catch (Exception)
            {
            }

            return settings;
        }

        public net_LogoSettings GetItemList()
        {
            var settings = new net_LogoSettings();
            var sql = new SqlConnection(_logoSqlStr.ToString());

            var query = Properties.Resources.GetItemList.ToSqlString(_logoFirmNo, _logoPeriodNr);

            if (settings != null)
            {
                settings.LogoPassword = Crypter.Decrypt(settings.LogoPassword, _cryptoKey);
            }

            return settings;
        }

        public bool saveProductPairing(net_ProductPairing productPairing)
        {
            var query = Properties.Resources.saveProductPairing;

            var sql = new SqlConnection(_sqlStr.ToString());

            var effectedRow = sql.Execute(query, productPairing);
            if (effectedRow > 0)
                return true;

            return false;
        }

        public net_ProductPairing GetProductPairing(net_ProductPairing pairing)
        {
            var sql = new SqlConnection(_sqlStr.ToString());

            return sql.Query<net_ProductPairing>(
                Properties.Resources.GetProductPairing,
                pairing).FirstOrDefault();
        }

        public string GetClCodeByUyeId(string id)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());

            var query = Properties.Resources.GetClCodeByUyeId.ToSqlString(_logoFirmNo, _logoPeriodNr);

            return sql.ExecuteScalar<string>(query, new {@ID = id});
        }
        public string GetClCodeByTCKN(string id)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());

            var query = Properties.Resources.GetCLCodeByTCKN.ToSqlString(_logoFirmNo, _logoPeriodNr);

            return sql.ExecuteScalar<string>(query, new { @ID = id });
        }

        public string GetClCodeByUserName(string username)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());

            var query = Properties.Resources.GetClCodeByUserName.ToSqlString(_logoFirmNo, _logoPeriodNr);

            return sql.ExecuteScalar<string>(query, new {@DEFINITION2 = username});
        }

        public int GetClientrefByCode(string code)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());
            return sql.ExecuteScalar<int>(
                $@" select LOGICALREF from LG_{_logoFirmNo}_CLCARD where CODE ='" +
                code +
                "' ");
        }

        public string GetClientCodeFromPairingTable(string eMail, string entegrationName)
        {
            var query = Properties.Resources.GetClientCodeFromPairingTable;

            using (var sqlConnection = new SqlConnection(_sqlStr.ToString()))
            {
                return sqlConnection.Query<string>(
                    query,
                    new
                    {
                        @EntegrationName = entegrationName,
                        @EMail = eMail
                    }).First();
            }
        }

        public string GetLastClCode(string clPrefix)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());

            var query = Properties.Resources.GetLastClCode.ToSqlString(_logoFirmNo, _logoPeriodNr);

            var newCode = sql.ExecuteScalar<int>(query, new {@PREFIX = clPrefix});

            return newCode.ToString().PadLeft(16 - clPrefix.Length, '0');
        }

        public string GetItemCode(string barcode)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());
            return sql.ExecuteScalar<string>(
                $"SELECT CODE FROM LG_{_logoFirmNo}_ITEMS WHERE LOGICALREF IN ( SELECT ITEMREF FROM LG_{_logoFirmNo}_UNITBARCODE WHERE BARCODE='" +
                barcode +
                "') ");
        }

        public string GetItemCodeByProducerCode(string producerCode)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());
            return sql.ExecuteScalar<string>(
                $"SELECT CODE FROM LG_{_logoFirmNo}_ITEMS WHERE PRODUCERCODE = '" +
                producerCode +
                "') ");
        }

        public string GetItemMainUnit(string itemCode)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());
            return sql.ExecuteScalar<string>(
                $@" 
            SELECT UNIT.CODE FROM LG_{_logoFirmNo}_ITEMS ITM
            LEFT JOIN LG_{_logoFirmNo}_UNITSETL UNIT ON UNIT.UNITSETREF = ITM.UNITSETREF
            AND MAINUNIT = 1 WHERE ITM.CODE = '{itemCode}' ");
        }

        public string GetSrvUnitCode(string srvCode)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());
            return sql.ExecuteScalar<string>(
                $@" 
            SELECT UNIT.CODE FROM LG_{_logoFirmNo}_SRVCARD SRV
            LEFT  JOIN LG_{_logoFirmNo}_SRVUNITA SRU ON SRU.SRVREF= SRV.LOGICALREF AND LINENR = 1
            LEFT JOIN LG_{_logoFirmNo}_UNITSETL UNIT ON UNIT.LOGICALREF = SRU.UNITLINEREF
            WHERE SRV.CODE= '{srvCode}'");
        }

        public int GetOrderRef(string specode, string orderNr, string cargoNo)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());
            return sql.ExecuteScalar<int>(
                $" SELECT top 1 LOGICALREF  FROM LG_{_logoFirmNo}_01_ORFICHE WHERE SPECODE =@specode AND CUSTORDNO =@orderNr AND DOCTRACKINGNR =@cargoNo order by LOGICALREF desc ",
                new {specode, orderNr, cargoNo});
        }

        public int GetOrderLineCount(int ficheRef)
        {
            var sql = new SqlConnection(_logoSqlStr.ToString());
            return sql.ExecuteScalar<int>(
                $"  SELECT count(*)  FROM LG_{_logoFirmNo}_01_ORFLINE  WHERE ORDFICHEREF =@ficheRef ",
                new {ficheRef});
        }

        public net_EntegratedOrders GetOrder(string EntegrationName, string OrderNo, int LineNr)
        {
            var sql = new SqlConnection(_sqlStr.ToString());
            return sql.Query<net_EntegratedOrders>(
                " select * from net_Orders  where EntegrationName =@EntegrationName and OrderNo=@OrderNo and LineNr=@LineNr ",
                new {EntegrationName, OrderNo, LineNr}).FirstOrDefault();
        }

        public List<net_EntegratedOrders> GetTransferedOrders(
            string EntegrationName,
            DateTime begdate,
            DateTime enddate)
        {
            var sql = new SqlConnection(_sqlStr.ToString());
            var query = $@" SELECT *   FROM net_Orders
                                where Transfered=1 and EntegrationName=@EntegrationName and OrderDate between @begdate and @enddate  ";
            return sql.Query<net_EntegratedOrders>(query, new {EntegrationName, begdate, enddate}).ToList();
        }

        public bool GetOrderStatus(string EntegrationName, string orderNo, string orderLineNr)
        {
            var sql = new SqlConnection(_sqlStr.ToString());
            var query = $@" SELECT isnull(Transfered ,0)  FROM net_Orders 
                                where  EntegrationName=@EntegrationName and  OrderNo=@orderNo and LineNr=@orderLineNr  ";
            return sql.ExecuteScalar<bool>(query, new {EntegrationName, orderNo, orderLineNr});
        }

        public List<net_EntegratedOrders> GetWaitingOrders(string EntegrationName, DateTime begdate, DateTime enddate)
        {
            var sql = new SqlConnection(_sqlStr.ToString());
            var query = @"
SELECT  *
FROM    dbo.net_Orders
WHERE   Transfered = 0
        AND EntegrationName = @EntegrationName
                   AND OrderDate
            BETWEEN @begdate AND @enddate
";

            var liste = sql.Query<net_EntegratedOrders>(query, new {EntegrationName, begdate, enddate }).ToList();

            return liste;
        }

        public List<string> GetKoctasOrders(string EntegrationName)
        {
            var sql = new SqlConnection(_sqlStr.ToString());
            var query = $@" SELECT OrderNo FROM net_Orders
                                where Transfered=0 and EntegrationName=@EntegrationName  and LineNr=-1  ";
            return sql.Query<string>(query, new {EntegrationName}).ToList();
        }

        public bool saveOrders(net_EntegratedOrders entegratedOrder)
        {
            var query = Properties.Resources.saveOrders;
            var sql = new SqlConnection(_sqlStr.ToString());
            var effectedRow = sql.Execute(query, entegratedOrder);

            return effectedRow > 0;
        }

        public bool saveKoctasOrders(net_EntegratedOrders entegratedOrder)
        {
            var query = Properties.Resources.saveKoctasOrders;
            var sql = new SqlConnection(_sqlStr.ToString());
            var effectedRow = sql.Execute(query, entegratedOrder);

            return effectedRow > 0;
        }

        public bool UpdateOrders(net_EntegratedOrders entegratedOrder)
        {
            var query = Properties.Resources.UpdateOrders;
            var sql = new SqlConnection(_sqlStr.ToString());
            var effectedRow = sql.Execute(query, entegratedOrder);
            return effectedRow > 0;
        }

        public bool UpdateOrderStatus(string Id, int Transfered)
        {
            var saved = false;
            var query = $@"UPDATE net_Orders  SET  Transfered=@Transfered     where Id=@Id";

            var sql = new SqlConnection(_sqlStr.ToString());
            var effectedRow = sql.Execute(query, new {Id, Transfered});
            if (effectedRow > 0)
            {
                saved = true;
            }

            return saved;
        }

        public int getShipInforef(string code, string arpcode)
        {
            var clCref = 0;

            var query =
                $@"SELECT LOGICALREF FROM LG_{_logoFirmNo}_SHIPINFO WHERE CODE='{code}' and CLIENTREF= (SELECT LOGICALREF FROM LG_{_logoFirmNo}_CLCARD WHERE CODE='{arpcode}')  ";

            var cn = new SqlConnection(_logoSqlStr.ToString());
            clCref = cn.ExecuteScalar<int>(query);

            return clCref;
        }

        public bool createTables()
        {
            var saved = false;

            var query = Properties.Resources.createTables;

            var sql = new SqlConnection(_sqlStr.ToString());
            var effectedRow = sql.Execute(query);
            if (effectedRow > 0)
            {
                saved = true;
            }

            return saved;
        }

        public bool checkDatabase(net_ServerSettings settings)
        {
            var builder = new SqlConnectionStringBuilder();

            builder.DataSource = settings.ServerName;
            builder.InitialCatalog = "master";
            builder.UserID = settings.UserName;
            builder.Password = settings.Password;

            var query = Properties.Resources.checkDatabase;

            var sql = new SqlConnection(builder.ToString());

            return sql.ExecuteScalar<bool>(query, new {@DatabaseName = settings.DatabaseName});
        }

        public bool checkTable()
        {
            var query =
                $@" IF EXISTS(SELECT 1 FROM sys.Objects WHERE  Object_id = OBJECT_ID(N'net_EntegrationSettings') AND Type = N'U')
                                select 1
                                else
                                select 0";
            var sql = new SqlConnection(_sqlStr.ToString());
            return sql.ExecuteScalar<bool>(query);
        }

        public void createDataBase(net_ServerSettings settings)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = settings.ServerName;
            builder.InitialCatalog = "master";
            builder.UserID = settings.UserName;
            builder.Password = settings.Password;

            var sql = new SqlConnection(builder.ToString());
            sql.ExecuteScalar<bool>("create database " + settings.DatabaseName);
        }

        public string GetLogoItemCodeByBarcode(string barcode)
        {
            var query = Properties.Resources.GetLogoItemCodeByBarcode.ToSqlString(_logoFirmNo, _logoPeriodNr);

            using (var sqlConnection = new SqlConnection(_sqlStr.ToString()))
            {
                return sqlConnection.Query<string>(query, new {@BARCODE = barcode}).First();
            }
        }

        public void UpdateTransferedSign(string entegrationName, string orderNo, bool isTransfered)
        {
            var query = Properties.Resources.UpdateTransferedSign;

            using (var sqlConnection = new SqlConnection(_sqlStr.ToString()))
            {
                sqlConnection.Execute(query, new {@EntegrationName = entegrationName, @OrderNo = orderNo, @IsTransfered = isTransfered});
            }
        }

        public double GetTaxRate(string logoCode)
        {
            var query = Properties.Resources.GetTaxRate.ToSqlString(_logoFirmNo, _logoPeriodNr);

            using (var sqlConnection = new SqlConnection(_logoSqlStr.ToString()))
            {
                return sqlConnection.Query<double>(query, new {ITEMCODE = logoCode}).FirstOrDefault();
            }
        }

        public void InsertOrUpdateClientPairing(string entegrationName, string clientCode, string eMail)
        {
            var query = Properties.Resources.InsertOrUpdateClientPairing;

            using (var sqlConnection = new SqlConnection(_sqlStr.ToString()))
            {
                sqlConnection.Execute(
                    query,
                    new
                    {
                        @EntegrationName = entegrationName,
                        @EMail = eMail,
                        @ClientCode = clientCode,
                        @LogoFirmNo = _logoFirmNo
                    });
            }
        }

        public CommonShippingAddress GetShippingAddress(string shipCode, string clientCode)
        {
            var query = Properties.Resources.GetShippingAddress.ToSqlString(_logoFirmNo, _logoPeriodNr);

            using (var sqlConnection = new SqlConnection(_logoSqlStr.ToString()))
            {
                return sqlConnection.Query<CommonShippingAddress>(
                    query,
                    new
                    {
                        @SHIPCODE = shipCode,
                        @CLIENTCODE = clientCode
                    }).FirstOrDefault();
            }
        }

        public double GetOrderAmount(int ficheRef)
        {
            var query = Properties.Resources.GetOrderAmount.ToSqlString(_logoFirmNo, _logoPeriodNr);

            using (var sqlConnection = new SqlConnection(_logoSqlStr.ToString()))
            {
                return sqlConnection.Query<double>(query, new { @FICHEREF = ficheRef }).First();
            }
        }
    }
}