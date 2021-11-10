SELECT  ISNULL(( SELECT TOP 1   CODE
                 FROM   dbo.LG_001_CLCARD
                 WHERE  SPECODE5 = @ID), '')