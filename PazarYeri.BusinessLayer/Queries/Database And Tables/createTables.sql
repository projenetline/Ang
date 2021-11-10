CREATE TABLE [dbo].[net_EntegrationSettings]
(
  [Id] [INT] IDENTITY(1, 1) NOT NULL
 ,[EntegrationName] [NVARCHAR](50) NULL
 ,[FirmCode] [NVARCHAR](50) NULL
 ,[UserName] [NVARCHAR](50) NULL
 ,[PassWord] [NVARCHAR](250) NULL
 ,[Excel] [BIT] NULL
 ,[WebService] [BIT] NULL
 ,[Specode] [NVARCHAR](50) NULL
 ,[PaymentCode] [NVARCHAR](50) NULL
 ,[ArpCodeShpm] [NVARCHAR](50) NULL
 ,[ArpCode] [NVARCHAR](50) NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[net_LogoSettings]
(
  [Id] [INT] IDENTITY(1, 1) NOT NULL
 ,[LogoServerName] [NVARCHAR](50) NULL
 ,[LogoDatabase] [NVARCHAR](50) NULL
 ,[LogoUserName] [NVARCHAR](50) NULL
 ,[LogoPassword] [NVARCHAR](250) NULL
 ,[FirmNr] [INT] NULL
 ,[PeriodNr] [INT] NULL
 ,[AutoTransfer] [BIT] NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[net_Orders]
(
  [Id] [INT] IDENTITY(1, 1) NOT NULL
 ,[EntegrationName] [NVARCHAR](50) NULL
 ,[OrderNo] [NVARCHAR](50) NULL
 ,[OrderDate] [DATETIME] NULL
 ,[OrderXml] [IMAGE] NULL
 ,[Transfered] [INT] NULL
 ,[ResultMsg] [NVARCHAR](250) NULL
 ,[LineNr] [NVARCHAR](250) NULL
 ,[LogoFicheNo] [NVARCHAR](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[net_ProductPairing]
(
  [Id] [INT] IDENTITY(1, 1) NOT NULL
 ,[EntegrationName] [NVARCHAR](50) NULL
 ,[EntegrationCode] [NVARCHAR](50) NULL
 ,[LogoCode] [NVARCHAR](50) NULL
 ,[Barcode] [NVARCHAR](50) NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[net_ShipInfo]
(
  [Id] [INT] IDENTITY(1, 1) NOT NULL
 ,[OrderId] [INT] NULL
 ,[Name_] [NVARCHAR](50) NULL
 ,[Surname] [NVARCHAR](50) NULL
 ,[Address1] [NVARCHAR](250) NULL
 ,[Address2] [NVARCHAR](250) NULL
 ,[Town] [NVARCHAR](50) NULL
 ,[District] [NVARCHAR](50) NULL
 ,[City] [NVARCHAR](50) NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[net_ClientPairing]
(
  [Id] [INT] IDENTITY(1, 1) NOT NULL
 ,[LogoFirmNo] [INT] NULL
 ,[EntegrationName] [NVARCHAR](50) NULL
 ,[EMail] [NVARCHAR](250) NULL
 ,[ClientCode] [NVARCHAR](50) NULL
 ,CONSTRAINT [PK_net_ClientPairing]
    PRIMARY KEY CLUSTERED ([Id] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON
         ,ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[net_LogoTransferSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientTransferPrefixCode] [nvarchar](50) NULL,
	[OrderTransferDoCode] [nvarchar](50) NULL,
	[OrderTransferProjectCode] [nvarchar](50) NULL,
	[OrderTransferSalesManCode] [nvarchar](50) NULL,
	[OrderTransferTradingGroup] [nvarchar](50) NULL,
	[OrderTransferServiceChargeCode] [nvarchar](50) NULL,
	[OrderTransferWareHouseNr] [int] NULL,
	[OrderTransferDivisionNr] [int] NULL,
	[EntegrationName] [nvarchar](50) NULL,
	[ClientTransferSpeCode1] [nvarchar](50) NULL,
	[ClientTransferSpeCode2] [nvarchar](50) NULL,
	[ClientTransferSpeCode3] [nvarchar](50) NULL,
	[ClientTransferSpeCode4] [nvarchar](50) NULL,
	[ClientTransferSpeCode5] [nvarchar](50) NULL,
	[ClientTransferAccountCode] [nvarchar](50) NULL,
	[ClientTransferPaymentCode] [nvarchar](50) NULL,
	[ClientTransferTradingGroup] [nvarchar](50) NULL,
	[ClientTransferProjectCode] [nvarchar](50) NULL,
	[OrderTransferDiscountCouponCode] [nvarchar](50) NULL,
	[OrderTransferLateChargeCode] [nvarchar](50) NULL,
	[TransferFicheProjectCode] [nvarchar](50) NULL,
	[OrderTransferSpeCode] [nvarchar](50) NULL,
	[OrderTransferPaymentCode] [nvarchar](50) NULL,
	[OrderTransferArpShippmentCode] [nvarchar](50) NULL,
	[OrderTransferArpCode] [nvarchar](50) NULL,
	[OrderTransferRetransferTransferedOrder] [int] NULL,
	[OrderTransferAddRowsIfOrder] [int] NULL,
	[OrderTransferUnitPriceRoundingNumberOfDigits] [int] NULL,
	[OrderTransferStatus] [int] NULL,
	[OrderTransferGroupByOrderNumber] [int] NULL,
	[ClientTransferAuthCode] [nvarchar](50) NULL,
	[OrderTransferAuthCode] [nvarchar](50) NULL,
	[OrderTransferTransferToShippingAddress] [int] NULL
) ON [PRIMARY]

