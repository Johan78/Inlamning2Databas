CREATE TRIGGER [tr_CustomerNameUpdate]
ON DATABASE
FOR DDL_TABLE_VIEW_EVENTS
AS
BEGIN
	SET NOCOUNT ON
END
-------------------------------------------------------

CREATE TABLE [dbo].[CustomerNameUpdate](
    [CustomerNameUpdateID] [int] IDENTITY(1,1) NOT NULL,
    [CustomerID] [nchar](5) NOT NULL,
    [OldName] [varchar](30) NOT NULL,
    [NewName] [varchar](30) NOT NULL,
    [ChangedDate] [datetime] NOT NULL,
    PRIMARY KEY CLUSTERED 
    (
        [CustomerNameUpdateID] ASC
    )
)

GO
---------------------------------------------------

CREATE TRIGGER [CustomerNameUpdateTrigger]
ON [dbo].[Customers]
after UPDATE
AS

BEGIN
    SET NOCOUNT ON
    INSERT INTO [dbo].[CustomerNameUpdate]
        ([CustomerID], [OldName], [NewName],[ChangedDate])
        SELECT i.[CustomerID], d.[ContactName],i.[ContactName], GETDATE()
            FROM inserted i
              INNER JOIN deleted d ON i.[CustomerID]=d.[CustomerID]
            WHERE d.[ContactName] <> i.[ContactName]
END

GO
