IF EXISTS ( SELECT  TOP 1   ClientCode
            FROM    dbo.net_ClientPairing
            WHERE   EntegrationName = @EntegrationName
                    AND EMail = @EMail)
BEGIN

  UPDATE    dbo.net_ClientPairing
  SET       ClientCode = @ClientCode
  WHERE     EntegrationName = @EntegrationName
            AND EMail = @EMail

END
ELSE
BEGIN

  INSERT INTO   dbo.net_ClientPairing
  (
    LogoFirmNo
   ,EntegrationName
   ,EMail
   ,ClientCode
  )
  VALUES
  ( @LogoFirmNo
   ,@EntegrationName
   ,@EMail
   ,@ClientCode)

END