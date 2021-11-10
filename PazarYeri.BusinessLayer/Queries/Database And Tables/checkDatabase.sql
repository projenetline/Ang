IF EXISTS (SELECT   ISNULL(( SELECT     TOP 1   name
                             FROM       master.sys.databases
                             WHERE      name = @DatabaseName
                             ORDER BY   database_id), ''))
  SELECT    1
ELSE
  SELECT    0