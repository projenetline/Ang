IF EXISTS ( SELECT  1
            FROM    dbo.net_EntegrationSettings
            WHERE   Id = @Id)
BEGIN
  UPDATE    dbo.net_EntegrationSettings
  SET       EntegrationName = @EntegrationName
           ,FirmCode = @FirmCode
           ,UserName = @UserName
           ,PassWord = @PassWord
           ,Excel = @Excel
           ,WebService = @WebService
  WHERE     Id = @Id
END
ELSE
BEGIN
  INSERT INTO   dbo.net_EntegrationSettings
  (
    EntegrationName
   ,FirmCode
   ,UserName
   ,PassWord
   ,Excel
   ,WebService
  )
  VALUES
  ( @EntegrationName
   ,@FirmCode
   ,@UserName
   ,@PassWord
   ,@Excel
   ,@WebService)
END