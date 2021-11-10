IF EXISTS ( SELECT  1
            FROM    dbo.net_Orders
            WHERE   EntegrationName = @EntegrationName
                    AND OrderNo = @OrderNo
                    AND ( LineNr = @LineNr
                          OR LineNr = -1))
BEGIN
  UPDATE    dbo.net_Orders
  SET       EntegrationName = @EntegrationName
           ,OrderNo = @OrderNo
           ,OrderDate = @OrderDate
           ,OrderXml = @OrderXml
           ,ResultMsg = @ResultMsg
           ,Transfered = @Transfered
           ,LineNr = @LineNr
           ,LogoFicheNo = @LogoFicheNo
  WHERE     EntegrationName = @EntegrationName
            AND OrderNo = @OrderNo
            AND ( LineNr = @LineNr
                  OR LineNr = -1)
END
ELSE
BEGIN
  INSERT INTO   dbo.net_Orders
  (
    EntegrationName
   ,OrderNo
   ,OrderDate
   ,OrderXml
   ,Transfered
   ,ResultMsg
   ,LineNr
  )
  VALUES
  ( @EntegrationName
   ,@OrderNo
   ,@OrderDate
   ,@OrderXml
   ,@Transfered
   ,@ResultMsg
   ,@LineNr)
END