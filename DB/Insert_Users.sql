USE [AuthDB]
GO

/*
Original Password : bikash
It has hashed and stored.
*/

INSERT INTO [dbo].[Users]
           (
            [Email]
           ,[PasswordHash]
           ,[Role]
           ,[AuthProvider]
           ,[IsActive]
           ,[CreatedAt])
     VALUES
           (
           'bikash.pattanayak@gmail.com'
           ,'$2a$11$fgc5MvcgorzZSAm/zgR4kOmcXtKBlTwE2QVTM16n66ASIWg2xmnzG'
           ,'admin'
           ,'Local'
           ,1
           ,GETDATE())
GO


