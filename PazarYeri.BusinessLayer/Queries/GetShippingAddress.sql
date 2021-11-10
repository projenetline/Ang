SELECT      SHP.CODE AS Code
           ,CLC.CODE AS ArpCode
           ,SHP.NAME AS Description
           ,SHP.ADDR1 AS Address1
           ,SHP.ADDR2 AS Address2
           ,SHP.CITY AS City
           ,SHP.TOWN AS Town
FROM        dbo.LG_001_SHIPINFO AS SHP
LEFT JOIN   dbo.LG_001_CLCARD AS CLC WITH (NOLOCK) ON CLC.LOGICALREF = SHP.CLIENTREF
WHERE       SHP.CODE = @SHIPCODE
            AND CLC.CODE = @CLIENTCODE