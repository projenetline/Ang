IF EXISTS ( SELECT  1
            FROM    dbo.net_LogoSettings)
BEGIN
  UPDATE    dbo.net_LogoSettings
  SET       LogoServerName = @LogoServerName
           ,LogoDatabase = @LogoDatabase
           ,LogoUserName = @LogoUserName
           ,LogoPassword = @LogoPassword
           ,FirmNr = @FirmNr
           ,PeriodNr = @PeriodNr
           ,AutoTransfer = @AutoTransfer
END
ELSE
BEGIN
  INSERT INTO   dbo.net_LogoSettings
  (
    LogoServerName
   ,LogoDatabase
   ,LogoUserName
   ,LogoPassword
   ,FirmNr
   ,PeriodNr
   ,AutoTransfer
  )
  VALUES
  ( @LogoServerName
   ,@LogoDatabase
   ,@LogoUserName
   ,@LogoPassword
   ,@FirmNr
   ,@PeriodNr
   ,@AutoTransfer)
END