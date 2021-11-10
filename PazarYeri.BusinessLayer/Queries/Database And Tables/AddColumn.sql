IF NOT EXISTS ( SELECT      1
            FROM        sys.objects AS o
            LEFT JOIN   sys.columns AS c WITH (NOLOCK) ON c.object_id = o.object_id
            WHERE       o.name = @TableName
                        AND o.type = 'U'
                        AND c.name = @NewColumnName)
BEGIN

  DECLARE @SQLTEXT NVARCHAR(1000)

  SET @SQLTEXT = 'ALTER TABLE dbo.' + @TableName + ' ADD ' + @NewColumnName + ' ' + @DataType + ' NULL'

  EXEC sys.sp_executesql @SQLTEXT

END