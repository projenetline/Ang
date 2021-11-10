SELECT  *
FROM    dbo.net_ProductPairing
WHERE   EntegrationName = @EntegrationName
        AND EntegrationCode = @EntegrationCode