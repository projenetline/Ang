SELECT  ISNULL(( SELECT     TOP 1   CODE
                 FROM       dbo.LG_001_CLCARD
                 WHERE      DEFINITION2 = @DEFINITION2
                 ORDER BY   LOGICALREF DESC), '')