SELECT  ISNULL(( SELECT     TOP 1   ITM.VAT
                 FROM       dbo.LG_001_ITEMS AS ITM
                 WHERE      ITM.CODE = @ITEMCODE
                 ORDER BY   ITM.LOGICALREF), 18)