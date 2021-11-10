IF EXISTS ( SELECT      1
            FROM        sys.objects AS o
            LEFT JOIN   sys.columns AS c WITH (NOLOCK) ON c.object_id = o.object_id
            WHERE       o.name = @TableName
                        AND o.type = 'U'
                        AND c.name = @OldColumnName)
BEGIN

  DECLARE @ObjectName NVARCHAR(100)
  SET @ObjectName = @TableName + '.' + @OldColumnName

  EXEC sys.sp_rename @objname = @ObjectName
                    ,@newname = @NewColumnName
                    ,@objtype = 'COLUMN'

END