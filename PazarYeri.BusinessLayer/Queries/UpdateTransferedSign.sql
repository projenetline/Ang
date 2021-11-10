UPDATE  dbo.net_Orders
SET     Transfered = @IsTransfered
WHERE   EntegrationName = @EntegrationName
        AND OrderNo = @OrderNo