IF NOT EXISTS ( SELECT  1
                FROM    dbo.net_Orders
                WHERE   EntegrationName = @EntegrationName
                        AND OrderNo = @OrderNo
                        AND LineNr = @LineNr)
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