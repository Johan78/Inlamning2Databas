
CREATE PROCEDURE [dbo].[sp_InsertContact] (@CustomerID  NVARCHAR(5), 
                                 @ContactName nvarchar(15),
                                 @CompanyName nvarchar(15))
                          
AS

INSERT INTO [dbo].[Customers]
           ([CustomerID]
           ,[ContactName]
           ,[CompanyName])
        
     VALUES
           (@CustomerID
           ,@ContactName
           ,@CompanyName)


