CREATE PROCEDURE spInsertProduct (@ProductName nvarchar(30),
                                 @UnitPrice money)
                          
AS

INSERT INTO [dbo].[Products]
           ([ProductName]
           ,[UnitPrice])
        
     VALUES
           (@ProductName
           ,@UnitPrice)
