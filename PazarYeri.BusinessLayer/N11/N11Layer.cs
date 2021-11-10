using PazarYeri.BusinessLayer.com.n11.api;
using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models;
using PazarYeri.Models.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using PazarYeri.Models.Common;
using UnityObjects;

namespace PazarYeri.BusinessLayer.N11
{
    public class N11Layer
    {
        private readonly OrderServicePortService _detailedOrder;
        private readonly net_EntegrationSettings _entegrationSettings;
        private readonly net_LogoTransferSettings _transferSettings;
        private readonly ProjectUtil _util = new ProjectUtil();

        private readonly DatabaseLayer _databaseLayer;

        private const string _entegrationName = "N11";

        public N11Layer()
        {
            _databaseLayer = new DatabaseLayer();
            _detailedOrder = new OrderServicePortService();
            _entegrationSettings = _databaseLayer.GetEntegrationSettings(_entegrationName);
            _transferSettings = _databaseLayer.GetLogoTransferSettings(_entegrationName);
        }

        public void getOrders(DateTime begdate, DateTime endate)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                string format = "d";
                DetailedOrderListRequest orderListRequest = new DetailedOrderListRequest();
                Authentication authentication = new Authentication();
                authentication.appKey = _entegrationSettings.UserName;
                authentication.appSecret = _entegrationSettings.PassWord;
                orderListRequest.auth = authentication;

                int currentPage = 0;
                int? pageCount = 1;
                while (pageCount >= currentPage)
                {
                    orderListRequest.pagingData = new PagingData()
                    {
                        currentPage = currentPage,
                        pageSize = 100
                    };
                    orderListRequest.searchData = new OrderDataListRequest()
                    {
                    };

                    string bdate = begdate.Day.ToString().PadLeft(2, '0') + "/" + begdate.Month.ToString().PadLeft(2, '0') + "/" + begdate.Year;
                    string edate = endate.Day.ToString().PadLeft(2, '0') + "/" + endate.Month.ToString().PadLeft(2, '0') + "/" + endate.Year;
                    orderListRequest.searchData.period = new OrderSearchPeriod()
                    {
                        startDate = bdate,
                        endDate = edate,
                    };
                    var list = _detailedOrder.DetailedOrderList(orderListRequest);

                    pageCount = list.pagingData.pageCount;
                    List<DetailedOrderData> orderList = new List<DetailedOrderData>();

                    foreach (var item in list.orderList)
                    {
                        OrderDataRequest orderDataRequest = new OrderDataRequest() {id = item.id};

                        OrderDetailRequest request = new OrderDetailRequest()
                        {
                            auth = authentication,
                            orderRequest = new OrderDataRequest() {id = item.id},
                        };
                        var orderDetail = _detailedOrder.OrderDetail(request);
                        format = "g";
                        provider = new CultureInfo("fr-FR");
                        foreach (var detail in orderDetail.orderDetail.itemList)
                        {
                            DateTime dt = DateTime.ParseExact(item.createDate, format, provider);
                            _util.LogService(
                                item.orderNumber +
                                "===>>>" +
                                orderDetail.orderDetail.billingAddress.fullName);

                            if (orderDetail.orderDetail.billingAddress.fullName == "UĞUR KARADEMİR")
                            {
                            }

                            net_N11 n11 = new net_N11()
                            {
                                address = orderDetail.orderDetail.billingAddress.address,
                                city = orderDetail.orderDetail.billingAddress.city,
                                district = orderDetail.orderDetail.billingAddress.district,
                                fullName = orderDetail.orderDetail.billingAddress.fullName,
                                tcId = orderDetail.orderDetail.billingAddress.tcId,
                                gsm = orderDetail.orderDetail.billingAddress.gsm,
                                neighborhood = orderDetail.orderDetail.billingAddress.neighborhood,
                                postalCode = orderDetail.orderDetail.billingAddress.postalCode,
                                userId = orderDetail.orderDetail.buyer.id,
                                email = orderDetail.orderDetail.buyer.email,
                                commission = detail.commission,
                                createDate = DateTime.ParseExact(item.createDate, format, provider),
                                id = item.id,
                                lineId = detail.id,
                                orderNumber = item.orderNumber,
                                price = detail.price,
                                productName = detail.productName,
                                productSellerCode = detail.productSellerCode,
                                quantity = Convert.ToDecimal(detail.quantity),
                                n11Discount = Convert.ToDecimal(detail.sellerInvoiceAmount) - Convert.ToDecimal(detail.dueAmount),
                                vadeFarki = Convert.ToDecimal(detail.installmentChargeWithVAT),
                                Discount = Convert.ToDecimal(detail.sellerDiscount),
                                totalServicePrice = Convert.ToDecimal(orderDetail.orderDetail.billingTemplate.totalServiceItemOriginalPrice),
                            };
                            net_EntegratedOrders entegratedOrder = new net_EntegratedOrders();
                            using (var stringwriter = new System.IO.StringWriter())
                            {
                                var serializer = new XmlSerializer(typeof(net_N11));
                                serializer.Serialize(stringwriter, n11);
                                string returnStr = stringwriter.ToString();
                                entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                            }

                            entegratedOrder.EntegrationName = "N11";
                            entegratedOrder.OrderDate = n11.createDate;
                            entegratedOrder.OrderNo = n11.orderNumber;
                            entegratedOrder.LineNr = n11.lineId.ToString();
                            DatabaseLayer database = new DatabaseLayer();
                            database.saveOrders(entegratedOrder);
                        }
                    }

                    currentPage++;
                }
            }
            catch (Exception e)
            {
                _util.LogService(e.Message + " Konum : " + e.StackTrace);
            }
        }

        public bool OrderTransfer(net_N11 netorder, out string result)
        {
            var order = MapN11(netorder);

            var unityManager = new UnityManager(_entegrationName);

            unityManager.PostOrder(order, out result, out var ficheRef);

            return ficheRef > 0;
        }

        private CommonOrder MapN11(net_N11 n11Order)
        {
            var clientCode = _transferSettings.OrderTransferArpCode == ""
                ? _databaseLayer.GetClientCodeFromPairingTable(n11Order.email, _entegrationName)
                : _transferSettings.OrderTransferArpCode;

            if (string.IsNullOrEmpty(clientCode))
                clientCode = _transferSettings.ClientTransferPrefixCode + _databaseLayer.GetLastClCode(_transferSettings.ClientTransferPrefixCode);

            var commonOrder = new CommonOrder()
            {
                Header = new CommonOrderHeader()
                {
                    Number = "~",
                    Date = n11Order.createDate.Year == 2014 ? n11Order.createDate.AddYears(5) : n11Order.createDate,
                    ArpCode = clientCode,
                    CustOrdNo = n11Order.orderNumber,
                    SourceWh = _transferSettings.OrderTransferWareHouseNr,
                    SourceCostGrp = _transferSettings.OrderTransferWareHouseNr,
                    DocNumber = _transferSettings.OrderTransferDoCode,
                    PaymentCode = _transferSettings.OrderTransferPaymentCode,
                    Division = _transferSettings.OrderTransferDivisionNr,
                    SalesmanCode = _transferSettings.OrderTransferSalesManCode,
                    TradingGrp = _transferSettings.OrderTransferTradingGroup,
                    AuxilCode = _transferSettings.OrderTransferSpeCode,
                    AuthCode = _transferSettings.OrderTransferAuthCode,
                    ArpCodeShpm = _transferSettings.OrderTransferArpShippmentCode,
                    OrderStatus = _transferSettings.OrderTransferStatus,
                    CurrselTotal = 1,
                    AffectRisk = 0,
                    ProjectCode = _transferSettings.OrderTransferProjectCode,
                    CargoNo = "",
                    EArchiveTrInsteadOfDesp = 1,
                    EArchiveTrSendMod = 2,
                    EInvoice = 2,
                    EInvoiceProfileId = 2                    
                }
            };

            var name = "";
            var surName = "";

            var list = n11Order.fullName.Split(' ');

            for (var i = 0; i < list.Length; i++)
            {
                if (n11Order.fullName.Split(' ').Length - 1 == i)
                {
                    surName = n11Order.fullName.Split(' ')[i];
                }
                else
                {
                    name += " " + n11Order.fullName.Split(' ')[i];
                }
            }

            var address1 = n11Order.address.Length > 200 ? n11Order.address.Substring(0, 199) : n11Order.address;

            var address2 = n11Order.address.Length > 200 ? n11Order.address.Substring(200) : "";

            commonOrder.Header.ArAps = new CommonArAps()
            {
                AccountType = 3,
                Code = clientCode,
                Title = n11Order.fullName.ToUpper(),
                City = n11Order.city,
                Town = n11Order.district,
                AuxilCode = _transferSettings.ClientTransferSpeCode1,
                AuxilCode2 = _transferSettings.ClientTransferSpeCode2,
                AuxilCode3 = _transferSettings.ClientTransferSpeCode3,
                AuxilCode4 = _transferSettings.ClientTransferSpeCode4,
                AuxilCode5 = _transferSettings.ClientTransferSpeCode5,
                AuthCode = _transferSettings.ClientTransferAuthCode,
                Country = "TÜRKİYE",
                CountryCode = "TR",
                ProfilId = 2,
                EArchiveSendMode = 2,
                TradingGrp = _transferSettings.ClientTransferTradingGroup,
                PaymentCode = _transferSettings.ClientTransferPaymentCode,
                ProjectCode = _transferSettings.ClientTransferProjectCode,
                GlCode = _transferSettings.ClientTransferAccountCode,
                Telephone1 = n11Order.gsm,
                Telephone2 = "",
                EMail = n11Order.email,
                Contact = n11Order.fullName,
                Address1 = address1,
                Address2 = address2,
                PersCompany = 1,
                TCKNO = n11Order.tcId,
                Name = name.Trim(),
                SurName = surName.Trim(),
                InsteadOfDispatch = 1
            };

            commonOrder.Header.ShippingAddress = new CommonShippingAddress()
            {
                Code = "",
                ArpCode = ""
            };

            var lines = new List<CommonOrderLine>
            {
                new CommonOrderLine()
                {
                    Type = 0,
                    MasterCode = n11Order.productSellerCode,
                    Quantity = (double) n11Order.quantity,
                    Price = Convert.ToDouble(n11Order.price) / 1.18,
                    VatRate = 18,
                    UnitCode = _databaseLayer.GetItemMainUnit(n11Order.productSellerCode),
                    UnitConv1 = 1,
                    UnitConv2 = 1,
                    Total = Convert.ToDouble(n11Order.price) / 1.18 * (double) n11Order.quantity,
                    CalcType = 0,
                    SourceWh = _transferSettings.OrderTransferWareHouseNr,
                    SourceCostGrp = _transferSettings.OrderTransferWareHouseNr,
                    Division = _transferSettings.OrderTransferDivisionNr,
                    SalesManCode = _transferSettings.OrderTransferSalesManCode,
                    ProjectCode = _transferSettings.OrderTransferProjectCode,
                    AuxilCode = _transferSettings.OrderTransferSpeCode,
                    AuthCode = _transferSettings.OrderTransferAuthCode,
                }
            };

            if (n11Order.Discount > 0 && !string.IsNullOrEmpty(_transferSettings.OrderTransferDiscountCouponCode))
            {
                lines.Add(
                    new CommonOrderLine()
                    {
                        Type = 2,
                        MasterCode = "",
                        Quantity = 0,
                        Price = 0,
                        VatRate = 0,
                        UnitCode = "",
                        UnitConv1 = 0,
                        UnitConv2 = 0,
                        CalcType = 1,
                        Total = Convert.ToDouble(n11Order.Discount) * Convert.ToInt16(n11Order.quantity) / 1.18,
                        SourceWh = _transferSettings.OrderTransferWareHouseNr,
                        SourceCostGrp = _transferSettings.OrderTransferWareHouseNr,
                        Division = _transferSettings.OrderTransferDivisionNr,
                        SalesManCode = _transferSettings.OrderTransferSalesManCode,
                        ProjectCode = _transferSettings.OrderTransferProjectCode
                    });
            }

            if (n11Order.n11Discount > 0)
            {
                lines.Add(
                    new CommonOrderLine()
                    {
                        Type = 2,
                        MasterCode = _transferSettings.OrderTransferDiscountCouponCode,
                        Quantity = 0,
                        Price = 0,
                        VatRate = 0,
                        UnitCode = _databaseLayer.GetSrvUnitCode(_transferSettings.OrderTransferDiscountCouponCode),
                        UnitConv1 = 0,
                        UnitConv2 = 0,
                        CalcType = 1,
                        Total = Convert.ToDouble(n11Order.n11Discount) / 1.18,
                        SourceWh = _transferSettings.OrderTransferWareHouseNr,
                        SourceCostGrp = _transferSettings.OrderTransferWareHouseNr,
                        Division = _transferSettings.OrderTransferDivisionNr,
                        SalesManCode = _transferSettings.OrderTransferSalesManCode,
                        ProjectCode = _transferSettings.OrderTransferProjectCode
                    });
            }

            if (n11Order.totalServicePrice > 0)
            {
                lines.Add(
                    new CommonOrderLine()
                    {
                        Type = 4,
                        MasterCode = _transferSettings.OrderTransferServiceChargeCode,
                        Quantity = 1,
                        Price = Convert.ToDouble(n11Order.totalServicePrice) / 1.18,
                        VatRate = 18,
                        UnitCode = _databaseLayer.GetSrvUnitCode(_transferSettings.OrderTransferServiceChargeCode),
                        UnitConv1 = 1,
                        UnitConv2 = 1,
                        CalcType = 1,
                        Total = Convert.ToDouble(n11Order.Discount) * Convert.ToInt16(n11Order.quantity) / 1.18,
                        SourceWh = _transferSettings.OrderTransferWareHouseNr,
                        SourceCostGrp = _transferSettings.OrderTransferWareHouseNr,
                        Division = _transferSettings.OrderTransferDivisionNr,
                        SalesManCode = _transferSettings.OrderTransferSalesManCode,
                        ProjectCode = _transferSettings.OrderTransferProjectCode
                    });
            }

            if (n11Order.vadeFarki > 0)
            {
                lines.Add(
                    new CommonOrderLine()
                    {
                        Type = 4,
                        MasterCode = _transferSettings.OrderTransferLateChargeCode,
                        Quantity = 0,
                        Price = 0,
                        VatRate = 0,
                        UnitCode = _databaseLayer.GetSrvUnitCode(_transferSettings.OrderTransferLateChargeCode),
                        UnitConv1 = 0,
                        UnitConv2 = 0,
                        CalcType = 1,
                        Total = Convert.ToDouble(n11Order.Discount) * Convert.ToInt16(n11Order.quantity) / 1.18,
                        SourceWh = _transferSettings.OrderTransferWareHouseNr,
                        SourceCostGrp = _transferSettings.OrderTransferWareHouseNr,
                        Division = _transferSettings.OrderTransferDivisionNr,
                        SalesManCode = _transferSettings.OrderTransferSalesManCode,
                        ProjectCode = _transferSettings.OrderTransferProjectCode
                    });
            }

            commonOrder.Lines = lines;

            return commonOrder;
        }

        public bool transferOrder(net_N11 netorder, out string result)
        {
            result = "";
            bool transfered = false;

            var logoTransferSettings = _databaseLayer.GetLogoTransferSettings(_entegrationName);

            try
            {
                if (netorder.createDate.Year == 2014)
                {
                    netorder.createDate = netorder.createDate.AddYears(5);
                }

                int OrderRef = _databaseLayer.GetOrderRef(_transferSettings.OrderTransferSpeCode, netorder.orderNumber, "");
                string clCode = _databaseLayer.GetClCodeByUyeId(netorder.userId.ToString());
                Data order = Global.application.NewDataObject(DataObjectType.doSalesOrderSlip);
                if (OrderRef == 0)
                {
                    if (string.IsNullOrEmpty(clCode))
                    {
                        clCode = logoTransferSettings.ClientTransferPrefixCode +
                                 _databaseLayer.GetLastClCode(logoTransferSettings.ClientTransferPrefixCode);
                    }

                    CreateClCard(netorder, clCode);

                    order.New();
                    order.DataFields.FieldByName("NUMBER").Value = "~";

                    //order.DataFields.FieldByName("DOC_TRACK_NR").Value = logoTransf.ToString();
                    order.DataFields.FieldByName("DATE").Value = netorder.createDate.ToString("dd.MM.yyyy");
                    order.DataFields.FieldByName("ARP_CODE").Value = clCode;
                    order.DataFields.FieldByName("CUST_ORD_NO").Value = netorder.orderNumber;
                    order.DataFields.FieldByName("SOURCE_WH").Value = logoTransferSettings.OrderTransferWareHouseNr;
                    order.DataFields.FieldByName("SOURCE_COST_GRP").Value = logoTransferSettings.OrderTransferWareHouseNr;
                    order.DataFields.FieldByName("DOC_NUMBER").Value = logoTransferSettings.OrderTransferDoCode;
                    order.DataFields.FieldByName("PAYMENT_CODE").Value = _transferSettings.OrderTransferPaymentCode;
                    order.DataFields.FieldByName("DIVISION").Value = logoTransferSettings.OrderTransferDivisionNr;
                    order.DataFields.FieldByName("SALESMAN_CODE").Value = logoTransferSettings.OrderTransferSalesManCode;
                    order.DataFields.FieldByName("TRADING_GRP").Value = logoTransferSettings.OrderTransferTradingGroup;

                    if (!string.IsNullOrEmpty(_transferSettings.OrderTransferSpeCode))
                    {
                        order.DataFields.FieldByName("AUXIL_CODE").Value = _transferSettings.OrderTransferSpeCode;
                    }

                    if (!string.IsNullOrEmpty(_transferSettings.OrderTransferArpShippmentCode))
                    {
                        order.DataFields.FieldByName("ARP_CODE_SHPM").Value = _transferSettings.OrderTransferArpShippmentCode;
                    }

                    order.DataFields.FieldByName("ORDER_STATUS").Value = 4;
                    order.DataFields.FieldByName("CURRSEL_TOTAL").Value = 1;

                    order.DataFields.FieldByName("AFFECT_RISK").Value = 0;
                    order.DataFields.FieldByName("PROJECT_CODE").Value = logoTransferSettings.OrderTransferProjectCode;

                    Lines transactions_lines = order.DataFields.FieldByName("TRANSACTIONS").Lines;

                    transactions_lines.AppendLine();
                    transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = netorder.productSellerCode;

                    // transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value = n11Order.commission.ToString();
                    string quantity = netorder.quantity.ToString();
                    transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = quantity;

                    transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Convert.ToDouble(netorder.price) / 1.18;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value =
                        _databaseLayer.GetItemMainUnit(netorder.productSellerCode);
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("SOURCE_WH").Value = logoTransferSettings.OrderTransferWareHouseNr;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("SOURCE_COST_GRP").Value =
                        logoTransferSettings.OrderTransferWareHouseNr;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("DIVISION").Value = logoTransferSettings.OrderTransferDivisionNr;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("SALESMAN_CODE").Value =
                        logoTransferSettings.OrderTransferSalesManCode;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("PROJECT_CODE").Value =
                        logoTransferSettings.OrderTransferProjectCode;

                    if (netorder.Discount > 0)
                    {
                        transactions_lines.AppendLine();
                        transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 2;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = "";
                        transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = "0";
                        transactions_lines[transactions_lines.Count - 1].FieldByName("CALC_TYPE").Value = "1";
                        double discountAmnt = Convert.ToDouble(netorder.Discount) * Convert.ToInt16(quantity);
                        transactions_lines[transactions_lines.Count - 1].FieldByName("TOTAL").Value = discountAmnt / 1.18;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("SALESMAN_CODE").Value =
                            logoTransferSettings.OrderTransferSalesManCode;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("PROJECT_CODE").Value =
                            logoTransferSettings.OrderTransferProjectCode;
                    }

                    #region Barakat

                    // N11 İndirim Kupon Kodu
                    if (netorder.n11Discount > 0)
                    {
                        transactions_lines.AppendLine();
                        transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 2;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value =
                            _transferSettings.OrderTransferDiscountCouponCode;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = "0";
                        transactions_lines[transactions_lines.Count - 1].FieldByName("CALC_TYPE").Value = "1";
                        double discountAmnt = Convert.ToDouble(netorder.n11Discount);
                        transactions_lines[transactions_lines.Count - 1].FieldByName("TOTAL").Value = discountAmnt / 1.18;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("SALESMAN_CODE").Value =
                            logoTransferSettings.OrderTransferSalesManCode;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("PROJECT_CODE").Value =
                            logoTransferSettings.OrderTransferProjectCode;
                    }

                    // Hizmet Bedeli
                    if (netorder.totalServicePrice > 0)
                    {
                        transactions_lines.AppendLine();
                        transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 4;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value =
                            logoTransferSettings.OrderTransferServiceChargeCode;

                        //  transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value = n11Order.commission.ToString();
                        transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = 1;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value =
                            Convert.ToDouble(netorder.totalServicePrice) / 1.18;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value =
                            _databaseLayer.GetSrvUnitCode(logoTransferSettings.OrderTransferServiceChargeCode);
                        transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("SOURCE_WH").Value =
                            logoTransferSettings.OrderTransferWareHouseNr;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("SOURCE_COST_GRP").Value =
                            logoTransferSettings.OrderTransferWareHouseNr;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("DIVISION").Value = logoTransferSettings.OrderTransferDivisionNr;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("SALESMAN_CODE").Value =
                            logoTransferSettings.OrderTransferSalesManCode;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("PROJECT_CODE").Value =
                            logoTransferSettings.OrderTransferProjectCode;
                    }

                    // Vade Farkı
                    if (netorder.vadeFarki > 0)
                    {
                        transactions_lines.AppendLine();
                        transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 4;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = "055";

                        //  transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value = n11Order.commission.ToString();
                        transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = 1;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Convert.ToDouble(netorder.vadeFarki) / 1.18;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = _databaseLayer.GetSrvUnitCode("055");
                        transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("SOURCE_WH").Value =
                            logoTransferSettings.OrderTransferWareHouseNr;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("SOURCE_COST_GRP").Value =
                            logoTransferSettings.OrderTransferWareHouseNr;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("DIVISION").Value = logoTransferSettings.OrderTransferDivisionNr;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("SALESMAN_CODE").Value =
                            logoTransferSettings.OrderTransferSalesManCode;
                        transactions_lines[transactions_lines.Count - 1].FieldByName("PROJECT_CODE").Value =
                            logoTransferSettings.OrderTransferProjectCode;
                    }

                    #endregion
                }
                else
                {
                    order.Read(OrderRef);
                    Lines transactions_lines = order.DataFields.FieldByName("TRANSACTIONS").Lines;
                    transactions_lines.AppendLine();
                    transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = netorder.productSellerCode;

                    //   transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value = n11Order.commission.ToString();
                    string quantity = netorder.quantity.ToString();
                    transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = quantity;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value =
                        Convert.ToDouble(netorder.price) / 1.18;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value =
                        _databaseLayer.GetItemMainUnit(netorder.productSellerCode);
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("SOURCE_WH").Value = logoTransferSettings.OrderTransferWareHouseNr;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("SOURCE_COST_GRP").Value =
                        logoTransferSettings.OrderTransferWareHouseNr;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("DIVISION").Value = logoTransferSettings.OrderTransferDivisionNr;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("SALESMAN_CODE").Value =
                        logoTransferSettings.OrderTransferSalesManCode;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("PROJECT_CODE").Value =
                        logoTransferSettings.OrderTransferProjectCode;
                }

                order.ReCalculate();
                order.ExportToXML(
                    "SALES_ORDERS",
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Xml\{netorder.orderNumber}.xml");

                if (order.Post() == true)
                {
                    int logicalref = order.DataFields.FieldByName("INTERNAL_REFERENCE").Value;
                    double NetTotal = order.DataFields.FieldByName("TOTAL_NET").Value;
                    CreateArpVoucher(netorder, clCode, NetTotal);
                    transfered = true;
                }
                else
                {
                    if (order.ErrorCode != 0)
                    {
                        result = "DBError(" + order.ErrorCode.ToString() + ")-" + order.ErrorDesc + order.DBErrorDesc;
                    }
                    else if (order.ValidateErrors.Count > 0)
                    {
                        result = "XML ErrorList:";
                        for (int i = 0; i < order.ValidateErrors.Count; i++)
                        {
                            result += "(" + order.ValidateErrors[i].ID.ToString() + ") - " + order.ValidateErrors[i].Error;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return transfered;
        }

        private string CreateClCard(net_N11 address, string clcode)
        {
            string result = "";
            Data arps = Global.application.NewDataObject(DataObjectType.doAccountsRP);

            int clientref = _databaseLayer.GetClientrefByCode(clcode);
            if (clientref > 0)
            {
                arps.Read(clientref);
            }
            else
            {
                arps.New();
            }

            arps.DataFields.FieldByName("ACCOUNT_TYPE").Value = 3;
            arps.DataFields.FieldByName("CODE").Value = clcode;
            arps.DataFields.FieldByName("TITLE").Value = address.fullName.ToUpper();
            arps.DataFields.FieldByName("CITY").Value = address.city;
            arps.DataFields.FieldByName("TOWN").Value = address.district;

            arps.DataFields.FieldByName("AUXIL_CODE5").Value = address.userId.ToString();

            arps.DataFields.FieldByName("COUNTRY_CODE").Value = "TR";
            arps.DataFields.FieldByName("COUNTRY").Value = "TÜRKİYE";
            arps.DataFields.FieldByName("CORRESP_LANG").Value = 1;
            arps.DataFields.FieldByName("CL_ORD_FREQ").Value = 1;
            arps.DataFields.FieldByName("INVOICE_PRNT_CNT").Value = 1;
            arps.DataFields.FieldByName("PURCHBRWS").Value = 1;
            arps.DataFields.FieldByName("SALESBRWS").Value = 1;
            arps.DataFields.FieldByName("IMPBRWS").Value = 1;
            arps.DataFields.FieldByName("EXPBRWS").Value = 1;
            arps.DataFields.FieldByName("FINBRWS").Value = 1;
            arps.DataFields.FieldByName("COLLATRLRISK_TYPE").Value = 1;
            arps.DataFields.FieldByName("RISK_TYPE1").Value = 1;
            arps.DataFields.FieldByName("RISK_TYPE2").Value = 1;
            arps.DataFields.FieldByName("RISK_TYPE3").Value = 1;
            arps.DataFields.FieldByName("PROFILE_ID").Value = 2;
            arps.DataFields.FieldByName("EARCHIVE_SEND_MODE").Value = 2;
            arps.DataFields.FieldByName("CREATE_WH_FICHE").Value = 1;

            #region Barakat

            arps.DataFields.FieldByName("TRADING_GRP").Value = "Perakende";
            arps.DataFields.FieldByName("PAYMENT_CODE").Value = "Kredi Kartı";
            arps.DataFields.FieldByName("AUXIL_CODE").Value = "PR";
            arps.DataFields.FieldByName("AUXIL_CODE2").Value = "027";
            arps.DataFields.FieldByName("PROJECT_CODE").Value = "PR";
            arps.DataFields.FieldByName("GL_CODE").Value = "120.10.03";

            #endregion

            arps.DataFields.FieldByName("TELEPHONE1").Value = address.gsm;
            arps.DataFields.FieldByName("E_MAIL").Value = address.email;
            arps.DataFields.FieldByName("CONTACT").Value = address.fullName;
            arps.DataFields.FieldByName("DSP_SEND_EMAIL").Value = address.email;
            arps.DataFields.FieldByName("ORD_SEND_EMAIL").Value = address.email;
            arps.DataFields.FieldByName("INV_SEND_EMAIL").Value = address.email;
            arps.DataFields.FieldByName("REMINDER_EMAIL").Value = address.email;
            arps.DataFields.FieldByName("ITR_SEND_MAIL_ADR").Value = address.email;
            arps.DataFields.FieldByName("FBS_SEND_EMAILADDR").Value = address.email;
            arps.DataFields.FieldByName("FBA_SEND_EMAILADDR").Value = address.email;
            arps.DataFields.FieldByName("OFF_SEND_MAIL_ADDR").Value = address.email;

            if (address.address.Length > 200)
            {
                arps.DataFields.FieldByName("ADDRESS1").Value = address.address.Substring(0, 200);
                arps.DataFields.FieldByName("ADDRESS2").Value = address.address.Substring(200);
            }
            else
            {
                arps.DataFields.FieldByName("ADDRESS1").Value = address.address;
                arps.DataFields.FieldByName("ADDRESS2").Value = "";
            }

            arps.DataFields.FieldByName("PERSCOMPANY").Value = 1;
            arps.DataFields.FieldByName("TCKNO").Value = address.tcId;

            string[] names = address.fullName.Split(' ');
            if (names.Length > 1)
            {
                arps.DataFields.FieldByName("NAME").Value = names[0];
                arps.DataFields.FieldByName("SURNAME").Value = names[1];
            }
            else if (names.Length == 1)
            {
                arps.DataFields.FieldByName("NAME").Value = names[0];
                arps.DataFields.FieldByName("SURNAME").Value = ".";
            }
            else if (names.Length > 2)
            {
                arps.DataFields.FieldByName("NAME").Value = names[0];
                arps.DataFields.FieldByName("SURNAME").Value = names[1] + " " + names[2];
                ;
            }

            if (arps.Post() == true)
            {
            }
            else
            {
                if (arps.ErrorCode != 0)
                {
                    result = "DBError(" + arps.ErrorCode.ToString() + ")-" + arps.ErrorDesc + arps.DBErrorDesc;
                }
                else if (arps.ValidateErrors.Count > 0)
                {
                    result = "XML ErrorList:";
                    for (int i = 0; i < arps.ValidateErrors.Count; i++)
                    {
                        result += "(" + arps.ValidateErrors[i].ID.ToString() + ") - " + arps.ValidateErrors[i].Error;
                    }
                }
            }

            return result;
        }

        private void CreateArpVoucher(net_N11 order, string clCode, double netTotal)
        {
            Data arpvoucher = Global.application.NewDataObject(UnityObjects.DataObjectType.doARAPVoucher);
            arpvoucher.New();
            arpvoucher.DataFields.FieldByName("NUMBER").Value = "~";
            arpvoucher.DataFields.FieldByName("DATE").Value = order.createDate;
            arpvoucher.DataFields.FieldByName("TYPE").Value = 5;
            arpvoucher.DataFields.FieldByName("NOTES1").Value = order.orderNumber;
            arpvoucher.DataFields.FieldByName("TOTAL_DEBIT").Value = netTotal;
            arpvoucher.DataFields.FieldByName("TOTAL_CREDIT").Value = netTotal;

            UnityObjects.Lines transactions_lines = arpvoucher.DataFields.FieldByName("TRANSACTIONS").Lines;
            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ARP_CODE").Value = clCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TRANNO").Value = "~";
            transactions_lines[transactions_lines.Count - 1].FieldByName("DOC_NUMBER").Value = order.orderNumber;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DESCRIPTION").Value = order.orderNumber;
            transactions_lines[transactions_lines.Count - 1].FieldByName("CREDIT").Value = netTotal;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TC_AMOUNT").Value = netTotal;
            transactions_lines[transactions_lines.Count - 1].FieldByName("AFFECT_RISK").Value = 1;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DISTRIBUTION_TYPE_FNO").Value = 0;

            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ARP_CODE").Value = _transferSettings.OrderTransferArpShippmentCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TRANNO").Value = "~";
            transactions_lines[transactions_lines.Count - 1].FieldByName("DOC_NUMBER").Value = order.orderNumber;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DESCRIPTION").Value = order.orderNumber;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DEBIT").Value = netTotal;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TC_AMOUNT").Value = netTotal;

            transactions_lines[transactions_lines.Count - 1].FieldByName("DISTRIBUTION_TYPE_FNO").Value = 0;

            arpvoucher.DataFields.FieldByName("AFFECT_RISK").Value = 0;

            if (arpvoucher.Post() == true)
            {
            }
            else
            {
                if (arpvoucher.ErrorCode != 0)
                {
                    //  MessageBox.Show("DBError(" + arpvoucher.ErrorCode.ToString() + ")-" + arpvoucher.ErrorDesc + arpvoucher.DBErrorDesc);
                }
                else if (arpvoucher.ValidateErrors.Count > 0)
                {
                    string result = "XML ErrorList:";
                    for (int i = 0; i < arpvoucher.ValidateErrors.Count; i++)
                    {
                        result += "(" + arpvoucher.ValidateErrors[i].ID.ToString() + ") - " + arpvoucher.ValidateErrors[i].Error;
                    }

                    // MessageBox.Show(result);
                }
            }
        }
    }
}