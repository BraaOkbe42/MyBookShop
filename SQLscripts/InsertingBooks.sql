SET IDENTITY_INSERT [dbo].[Books] ON

INSERT INTO [dbo].[Books] ([BookID], [Title], [Author], [Price], [StockQuantity], [ImgUrl], [description]) VALUES (27, 'Game OF thrones', N'Harper Lee', CAST(12.99 AS Decimal(18, 2)), 15, N'Images\Novels\To Kill a Mockingbird.jpg', N'A novel dealing with serious issues of race and injustice.')
SET IDENTITY_INSERT [dbo].[Books] OFF
