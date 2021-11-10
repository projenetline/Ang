SELECT      ITM.CODE
           ,ITM.NAME
           ,BRC.BARCODE
FROM        dbo.LG_001_ITEMS AS ITM
LEFT JOIN   dbo.LG_001_UNITBARCODE AS BRC ON BRC.ITEMREF = ITM.LOGICALREF
                                             AND BRC.LINENR = 1
WHERE       ITM.CARDTYPE <> 22