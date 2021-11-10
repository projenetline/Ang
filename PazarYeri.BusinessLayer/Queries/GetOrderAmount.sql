SELECT  ISNULL(( SELECT     TOP 1   NETTOTAL
                 FROM       dbo.LG_001_01_ORFICHE
                 WHERE      LOGICALREF = @FICHEREF
                 ORDER BY   LOGICALREF), 0)