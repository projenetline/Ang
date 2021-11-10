IF EXISTS ( SELECT  1
            FROM    dbo.net_ProductPairing
            WHERE   Id = @Id)
BEGIN
  UPDATE    dbo.net_ProductPairing
  SET       EntegrationName = @EntegrationName
           ,EntegrationCode = @EntegrationCode
           ,LogoCode = @LogoCode
           ,Barcode = @Barcode
  WHERE     Id = @Id
END
ELSE
BEGIN
  INSERT INTO   dbo.net_ProductPairing
  (
    EntegrationName
   ,EntegrationCode
   ,LogoCode
   ,Barcode
  )
  VALUES
  ( @EntegrationName
   ,@EntegrationCode
   ,@LogoCode
   ,@Barcode)
END