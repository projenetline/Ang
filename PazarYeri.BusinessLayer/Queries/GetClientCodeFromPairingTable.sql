SELECT  ISNULL(( SELECT     TOP 1   ClientCode
                 FROM       dbo.net_ClientPairing
                 WHERE      EntegrationName = @EntegrationName
                            AND EMail = @EMail
                 ORDER BY   Id), '')