
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ApplicationUsers' and xtype='U')
BEGIN

	CREATE TABLE ApplicationUsers
		(
		    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL, 
		    FirstName VARCHAR(50) NOT NULL,
			LastName VARCHAR(50) NOT NULL,
			UserName VARCHAR(50) NOT NULL,
		    Email VARCHAR(50) NULL,
		    PasswordHash VARCHAR(MAX) NULL,
		    PhoneNumber VARCHAR(50) NULL,
			Created DATETIME NOT NULL DEFAULT GETDATE(),
			Modified DATETIME NOT NULL DEFAULT GETDATE()
		)
END


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Performers' and xtype='U')
BEGIN

	CREATE TABLE Performers
			(
				Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL, 
				UserId INT NOT NULL,
				PerformerName VARCHAR(50) NOT NULL,
				PerformerMail VARCHAR(50) NULL,
				VIP BIT NOT NULL,
				NumberOfPeople INT NOT NULL DEFAULT 1,
				Created DATETIME NOT NULL DEFAULT GETDATE(),
				Modified DATETIME NOT NULL DEFAULT GETDATE()
			)	
	
			ALTER TABLE [dbo].[Performers] ADD CONSTRAINT [FK_Performers_Users] FOREIGN KEY([UserId])
			REFERENCES [dbo].[ApplicationUsers] ([Id])
END


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Events' and xtype='U')
BEGIN

		CREATE TABLE [Events]
			(
				Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
				EventName VARCHAR(30) NOT NULL,
				EventDescription VARCHAR(MAX) NULL,
				StartDate DATETIME NOT NULL,
				EndDate DATETIME NULL,
				RecurringEvent BIT NOT NULL,
				PlaceType INT NOT NULL,
				City VARCHAR(50) NOT NULL,
				Street VARCHAR(50) NOT NULL,
				NumberStreet VARCHAR(10) NOT NULL,
				PostalCode VARCHAR(10) NOT NULL,
				EventCategory INT NOT NULL,
				EventType INT NOT NULL,
				CurrentStatus INT NOT NULL DEFAULT 1,
				Created DATETIME NOT NULL DEFAULT GETDATE(),
				Modified DATETIME NOT NULL DEFAULT GETDATE()
			)
END



IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Opinions' and xtype='U')
BEGIN

		CREATE TABLE [dbo].[Opinions]
			(
				Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
				EventId INT NOT NULL,
				UserId INT NULL,
				Comment VARCHAR(MAX) NULL,
				Stars INT NOT NULL,
				Created DATETIME NOT NULL DEFAULT GETDATE(),
				Modified DATETIME NOT NULL DEFAULT GETDATE()
			)
			
			ALTER TABLE [dbo].[Opinions] ADD CONSTRAINT [FK_Event_Opinion] FOREIGN KEY([EventId])
			REFERENCES [dbo].[Events] ([Id])
	
			ALTER TABLE [dbo].[Opinions] ADD  CONSTRAINT [FK_OpinionUser] FOREIGN KEY([UserId])
			REFERENCES [dbo].[ApplicationUsers] ([Id])
END


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EventImages' and xtype='U')
BEGIN

		CREATE TABLE [dbo].[EventImages]
			(
				Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
				EventId INT NOT NULL,
				IsMain BIT NOT NULL,
				ImagePath VARCHAR(MAX) NOT NULL,
				ImageTitle VARCHAR(50) NULL,
				ImageDescription VARCHAR(MAX) NOT NULL,
				Created DATETIME NOT NULL DEFAULT GETDATE(),
				Modified DATETIME NOT NULL DEFAULT GETDATE()
			)		
	
			ALTER TABLE [dbo].[EventImages] ADD  CONSTRAINT [FK_Event_EventImage] FOREIGN KEY([EventId])
			REFERENCES [dbo].[Events] ([Id])
END


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EventApplications' and xtype='U')
BEGIN

	CREATE TABLE [dbo].[EventApplications]
			(
				Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
				EventId INT NOT NULL,
				PerformerName VARCHAR(50) NOT NULL,
				TypePerformance INT NOT NULL,
				DurationInMinutes INT NOT NULL,
				CurrentStatus INT NOT NULL,
				LastModifiedByApplicant BIT NOT NULL DEFAULT 1,
				PerformerId INT NOT NULL,
				Created DATETIME NOT NULL DEFAULT GETDATE(),
				Modified DATETIME NOT NULL DEFAULT GETDATE()
			)
			
			ALTER TABLE [dbo].[EventApplications] ADD CONSTRAINT [FK_EventApplication_Event] FOREIGN KEY([EventId])
			REFERENCES [dbo].[Events] ([Id])
			
			ALTER TABLE [dbo].[EventApplications] ADD CONSTRAINT [FK_EventApplications_Performer] FOREIGN KEY([PerformerId])
			REFERENCES [dbo].[Performers] ([Id])
END


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PerformanceProposals' and xtype='U')
BEGIN
			
		CREATE TABLE [dbo].[PerformanceProposals]
			(
				Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
				PerformerId INT NOT NULL,
				[Message] VARCHAR(MAX) NOT NULL,
				ActiveTo DATETIME NOT NULL,
				EventId INT NOT NULL,
				Created DATETIME NOT NULL DEFAULT GETDATE(),
				Modified DATETIME NOT NULL DEFAULT GETDATE(),
				DateDeletion DATETIME NULL,
				Deleted BIT NOT NULL DEFAULT 0,
			)
			
			ALTER TABLE [dbo].[PerformanceProposals] ADD CONSTRAINT [FK_PerformanceProposal_Event] FOREIGN KEY([EventId])
			REFERENCES [dbo].[Events] ([Id])
			
			ALTER TABLE [dbo].[PerformanceProposals] ADD CONSTRAINT [FK_PerformanceProposal_Performer] FOREIGN KEY([PerformerId])
			REFERENCES [dbo].[Performers] ([Id])

END



IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RefreshTokens' and xtype='U')
BEGIN
		CREATE TABLE [dbo].[RefreshTokens]
		(
			Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
			UserId INT NOT NULL,
			Token VARCHAR(MAX) NULL,
			Expires DATETIME NOT NULL,
			Created DATETIME NOT NULL,
			ReplacedByToken VARCHAR(MAX) NULL,
			Revoked VARCHAR(250) NULL
		)
		
		ALTER TABLE [dbo].[RefreshTokens] ADD  CONSTRAINT [FK_RefreshToken_ApplicationUsers] FOREIGN KEY([UserId])
		REFERENCES [dbo].[ApplicationUsers] ([Id]) 

		
--TYPE
		
		CREATE TYPE [dbo].[UserRefreshTokensTableType] AS TABLE
		(
			Token VARCHAR(max) NULL,
			Expires DATETIME NOT NULL,
			Created DATETIME NOT NULL,
			ReplacedByToken VARCHAR(max) NULL,
			Revoked VARCHAR(250) NULL	
		)

END


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Roles' and xtype='U')
BEGIN
		CREATE TABLE [dbo].[Roles]
			(
				Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
				RoleName VARCHAR(25) NULL,
			)

		INSERT INTO Roles VALUES('User'), ('Performer'), 
								('Admin'), ('Owner')
END


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' and xtype='U')
BEGIN
		CREATE TABLE [dbo].[UserRoles]
		(
			Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
			UserId INT NOT NULL,
			RoleId INT NOT NULL,
		)
		
		ALTER TABLE [dbo].[UserRoles] ADD  CONSTRAINT [FK_ApplicationUsers_UserRoles] FOREIGN KEY([UserId])
		REFERENCES [dbo].[ApplicationUsers] ([Id])
		ON DELETE CASCADE
		
		ALTER TABLE [dbo].[UserRoles] ADD  CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY([RoleId])
		REFERENCES [dbo].[Roles] ([Id])
		ON DELETE CASCADE
END



