IF EXISTS ( SELECT  1
            FROM    dbo.net_Orders
            WHERE   EntegrationName = @EntegrationName
                    AND OrderNo = @OrderNo
                    AND LineNr = @LineNr)
BEGIN
  UPDATE    dbo.net_Orders
  SET       EntegrationName = @EntegrationName
           ,OrderNo = @OrderNo
           ,OrderDate = @OrderDate
           ,OrderXml = @OrderXml
           ,ResultMsg = @ResultMsg
           ,LineNr = @LineNr
  WHERE     EntegrationName = @EntegrationName
            AND OrderNo = @OrderNo
            AND LineNr = @LineNr
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