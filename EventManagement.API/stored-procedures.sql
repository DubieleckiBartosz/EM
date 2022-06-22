--Application

CREATE PROCEDURE [dbo].[application_createNewEventApplication_I]
@eventId INT,
@performerId INT,
@performerName VARCHAR(50),
@typePerformance INT,
@durationInMinutes INT,
@currentStatus INT
AS
BEGIN
	INSERT INTO EventApplications(EventId, PerformerId, PerformerName,
				TypePerformance, DurationInMinutes, CurrentStatus, LastModifiedByApplicant)
	VALUES (@eventId, @performerId, @performerName, 
			@typePerformance, @durationInMinutes, @currentStatus, 1)
END
GO



CREATE PROCEDURE [dbo].[application_getApplicationById_S]
	@applicationId INT,
	@userId INT,
	@isAdminOrOwner BIT
AS
BEGIN
	SELECT 
		ea.Id,
		ea.EventId,
		ea.PerformerId,
		ea.TypePerformance,
		ea.DurationInMinutes,
		ea.CurrentStatus,
		ea.LastModifiedByApplicant
	FROM EventApplications AS ea
	INNER JOIN Performers AS p ON p.Id = ea.PerformerId
	WHERE ea.Id = @applicationId 
	AND @isAdminOrOwner = 1 OR p.UserId = @userId
END
GO


CREATE PROCEDURE [dbo].[application_getApplicationsBySearch_S]
	@hasSuperAccess BIT,
	@userId INT,
    @sortName VARCHAR(50) = NULL,
    @sortType VARCHAR(10),
	@status INT = NULL,
	@eventName VARCHAR(30) = NULL,
	@from DATETIME = NULL,
	@to DATETIME = NULL,
	@performanceType INT = NULL,
	@durationInMinutesMin INT = NULL,
	@durationInMinutesMax INT = NULL,
	@lastModifiedByApplicant BIT = NULL
AS
BEGIN
	SELECT 
		ea.Id,
		ea.PerformerId,
		ea.TypePerformance,
		ea.DurationInMinutes,
		ea.CurrentStatus,
		ea.LastModifiedByApplicant,
		ea.EventId 
	FROM EventApplications AS ea
	INNER JOIN Performers AS p ON p.Id = ea.PerformerId
	INNER JOIN [Events] AS e ON e.Id = ea.EventId
	WHERE (@status IS NULL OR ea.CurrentStatus = @status)
	AND (@eventName IS NULL OR e.EventName = @eventName)
	AND (@from IS NULL OR ea.Created > @from)
	AND (@to IS NULL OR ea.Created < @to)
	AND (@performanceType IS NULL OR ea.TypePerformance = @performanceType)
	AND (@durationInMinutesMin IS NULL OR ea.DurationInMinutes > @durationInMinutesMin)
	AND (@durationInMinutesMax IS NULL OR ea.DurationInMinutes < @durationInMinutesMax)
	AND (@lastModifiedByApplicant IS NULL OR ea.LastModifiedByApplicant = @lastModifiedByApplicant)
	AND (@hasSuperAccess = 1 OR p.UserId = @userId)
	ORDER BY
	
	CASE WHEN @sortName = 'Id' AND @sortType = 'asc' THEN ea.Id END ASC,  
	CASE WHEN @sortName = 'Id' AND @sortType = 'desc' THEN ea.Id END DESC,

	CASE WHEN @sortName = 'Created' AND @sortType = 'asc' THEN ea.Created END ASC,  
	CASE WHEN @sortName = 'Created' AND @sortType = 'desc' THEN ea.Created END DESC,

	CASE WHEN @sortName = 'Status' AND @sortType = 'asc' THEN ea.CurrentStatus END ASC,  
	CASE WHEN @sortName = 'Status' AND @sortType = 'desc' THEN ea.CurrentStatus END DESC,

	CASE WHEN @sortName = 'Duration' AND @sortType = 'asc' THEN ea.DurationInMinutes END ASC,  
	CASE WHEN @sortName = 'Duration' AND @sortType = 'desc' THEN ea.DurationInMinutes END DESC,

	CASE WHEN @sortName = 'EventName' AND @sortType = 'asc' THEN e.EventName END ASC,  
	CASE WHEN @sortName = 'EventName' AND @sortType = 'desc' THEN e.EventName END DESC
END
GO



CREATE PROCEDURE [dbo].[application_getApplicationsWithDetails_S]
@eventId INT,
@statusApplication INT = NULL
AS
BEGIN
	SELECT
		 ea.Id ,
         ea.EventId ,
         ea.PerformerId ,
         ea.TypePerformance ,
         ea.DurationInMinutes ,
         ea.CurrentStatus ,
         ea.LastModifiedByApplicant ,
		 p.Id ,
		 p.NumberOfPeople ,
         p.PerformerMail,
		 p.PerformerName,
		 p.UserId
	FROM  EventApplications AS ea 
	LEFT JOIN Performers AS p ON p.Id = ea.PerformerId
	WHERE ea.EventId = @eventId  
	AND ea.LastModifiedByApplicant = 0
	AND @statusApplication IS NULL OR ea.CurrentStatus = @statusApplication
	AND ea.CurrentStatus != 3
END
GO




CREATE PROCEDURE [dbo].[application_updateEventApplication_U]
	@applicationId INT,
	@typePerformance VARCHAR(50) = NULL,
	@durationInMinutes INT = NULL,
	@currentStatus INT = NULL,
	@isApplicant BIT
AS
BEGIN
	UPDATE EventApplications 
	SET LastModifiedByApplicant = @isApplicant,
		TypePerformance = COALESCE(@typePerformance, TypePerformance),
		DurationInMinutes = COALESCE(@durationInMinutes, DurationInMinutes),
		CurrentStatus = COALESCE(@currentStatus, CurrentStatus)
	WHERE Id = @applicationId
END
GO


--Event


CREATE PROCEDURE [dbo].[event_createNewEvent_I]
    @eventName VARCHAR(30),
    @eventDescription VARCHAR(MAX),
    @startDate DATETIME,
    @endDate DATETIME,
    @recurringEvent BIT,
    @placeType INT,
    @city VARCHAR(50),
    @street VARCHAR(50),
    @numberStreet VARCHAR(10),
    @postalCode VARCHAR(10),
    @eventCategory INT,
    @eventType INT,
	@newIdentifier INT OUTPUT
AS
BEGIN
    INSERT INTO [Events] (EventName,  EventDescription, StartDate, EndDate,
        RecurringEvent, PlaceType, City, Street, NumberStreet,
        PostalCode, EventCategory, EventType, CurrentStatus)
    VALUES (@eventName, @eventDescription, @startDate, @endDate,
		 @recurringEvent, @placeType, @city,  @street,
		 @numberStreet, @postalCode, @eventCategory, @eventType, 1)

	SELECT @newIdentifier = CAST(SCOPE_IDENTITY() AS INT)
END
GO


CREATE PROCEDURE [dbo].[event_getEventBaseDataById_S]
	@eventId INT
AS
BEGIN
	SELECT 
		Id,
		EventName,
		EventDescription,
		StartDate,
		EndDate,
		RecurringEvent,
		PlaceType,
		City,
		Street,
		NumberStreet,
		PostalCode,
		EventCategory,
		EventType,
		CurrentStatus
	FROM [Events]
	WHERE Id = @eventId
END
GO




CREATE PROCEDURE [dbo].[event_getEventDetails_S]
	@eventId INT,
	@isAdmin BIT,
	@isOwner BIT
AS
BEGIN
	SELECT 
		e.Id,
		e.EventName,
		e.EventDescription,
		e.StartDate,
		e.EndDate,
		e.RecurringEvent,
		e.PlaceType,
		e.City,
		e.Street,
		e.NumberStreet,
		e.PostalCode,
		e.EventCategory,
		e.EventType,
		e.CurrentStatus,
		ei.Id,
		ei.IsMain,
		ei.ImagePath,
		ei.ImageTitle,
		ei.ImageDescription,
		o.Id,
		o.UserId,
		o.Comment,
		o.Stars,
		au.UserName
	FROM [Events] AS e 
	LEFT JOIN EventImages AS ei ON ei.EventId = e.Id
	LEFT JOIN Opinions AS o ON o.EventId = e.Id
	LEFT JOIN ApplicationUsers AS au ON au.Id = o.UserId
	WHERE e.Id = @eventId
	--AND  (((@isAdmin = 1 OR @isOwner = 1) AND e.CurrentStatus = 2) OR e.CurrentStatus = 1) 
END
GO




CREATE PROCEDURE [dbo].[event_getEventsBySearch_S]
    @eventId INT = NULL,
    @eventName VARCHAR(30) = NULL,
    @from DATETIME = NULL,
    @to DATETIME = NULL,
    @city VARCHAR(50) = NULL,
    @placeType INT = NULL,
    @recurringEvent INT = NULL,
    @category INT = NULL,
    @eventType INT = NULL,
	@eventCurrentStatus INT = NULL,
    @sortName VARCHAR(30),
    @sortType VARCHAR(5), 
    @pageNumber INT, 
    @pageSize INT
AS
BEGIN
	SELECT Id,
	       EventName,
	       EventDescription,
	       StartDate,
	       EndDate,
	       RecurringEvent,
	       PlaceType,
	       City,
	       Street,
	       NumberStreet,
	       PostalCode,
	       EventCategory,
	       EventType,
	       CurrentStatus,
	       COUNT(*) OVER () AS [Count]
	FROM [Events]
	WHERE (@eventId IS NULL OR Id = @eventId)
	AND (@from IS NULL OR StartDate >= @from)
	AND (@to IS NULL OR EndDate <= @to)
	AND (@city IS NULL OR City LIKE '%' + City + '%')
	AND (@eventType IS NULL OR EventType = @eventType)
	AND (@category IS NULL OR EventCategory = @category)
	AND (@recurringEvent IS NULL OR RecurringEvent = @recurringEvent)
	AND (@placeType IS NULL OR PlaceType = @placeType)
	AND (@eventCurrentStatus IS NULL OR CurrentStatus = @eventCurrentStatus)
	AND (@eventName IS NULL OR EventName LIKE '%' + @eventName + '%')
	ORDER BY 
	CASE WHEN @sortName = 'EventId' AND @sortType = 'asc' THEN Id END ASC,  
	CASE WHEN @sortName = 'EventId' AND @sortType = 'desc' THEN Id END DESC,
	
	CASE WHEN @sortName = 'EventName' AND @sortType = 'asc' THEN EventName END ASC,  
	CASE WHEN @sortName = 'EventName' AND @sortType = 'desc' THEN EventName END DESC,
	 
	CASE WHEN @sortName = 'StartDate' AND @sortType = 'asc' THEN StartDate END ASC,  
	CASE WHEN @sortName = 'StartDate' AND @sortType = 'desc' THEN StartDate END DESC, 

	CASE WHEN @sortName = 'EndDate' AND @sortType = 'asc' THEN EndDate END ASC,  
	CASE WHEN @sortName = 'EndDate' AND @sortType = 'desc' THEN EndDate END DESC, 

	CASE WHEN @sortName = 'RecurringEvent' AND @sortType = 'asc' THEN RecurringEvent END ASC,  
	CASE WHEN @sortName = 'RecurringEvent' AND @sortType = 'desc' THEN RecurringEvent END DESC, 
	
	CASE WHEN @sortName = 'City' AND @sortType = 'asc' THEN City END ASC,  
	CASE WHEN @sortName = 'City' AND @sortType = 'desc' THEN City END DESC,

	CASE WHEN @sortName = 'EventCategory' AND @sortType = 'asc' THEN EventCategory END ASC,  
	CASE WHEN @sortName = 'EventCategory' AND @sortType = 'desc' THEN EventCategory END DESC,

	CASE WHEN @sortName = 'EventType' AND @sortType = 'asc' THEN EventType END ASC,  
	CASE WHEN @sortName = 'EventType' AND @sortType = 'desc' THEN EventType END DESC,

	CASE WHEN @sortName = 'CurrentStatus' AND @sortType = 'asc' THEN CurrentStatus END ASC,  
	CASE WHEN @sortName = 'CurrentStatus' AND @sortType = 'desc' THEN CurrentStatus END DESC
	
	/*
	Na chwile obecn¹ niestety nie mo¿emy skorzystaæ z funkcji OFFSET, z dokumentacji:
	https://docs.microsoft.com/en-us/sql/t-sql/statements/alter-database-transact-sql-compatibility-level?redirectedfrom=MSDN&view=sql-server-ver15,
	stackoverflow: https://stackoverflow.com/questions/42263984/incorrect-syntax-near-offset-command
	SELECT compatibility_level  
    FROM sys.databases WHERE name = 'db207858';
	*/  

	--OFFSET (@pageNumber - 1)* @pageSize ROWS FETCH NEXT @pageSize ROWS READONLY
END
GO










CREATE PROCEDURE [dbo].[event_getEventWithApplicationDetails_S]
@eventId INT,
@statusApplication INT = NULL
AS
BEGIN
	SELECT 
	     e.Id ,
         e.EventName ,
         e.EventDescription ,
         e.StartDate ,
         e.EndDate ,
         e.PlaceType ,
         e.RecurringEvent ,
         e.City ,
         e.Street ,
         e.NumberStreet ,
         e.PostalCode ,
         e.EventCategory ,
         e.EventType ,
		 ea.Id ,
		 ea.EventId ,
         ea.PerformerId ,
         ea.TypePerformance ,
         ea.DurationInMinutes ,
         ea.CurrentStatus ,
         ea.LastModifiedByApplicant ,
		 p.NumberOfPeople ,
         p.PerformerMail 
	FROM [Events] AS e
	LEFT JOIN EventApplications AS ea ON ea.EventId = e.Id
	LEFT JOIN Performers AS p ON p.Id = ea.PerformerId
	WHERE e.Id = @eventId  
	AND ea.LastModifiedByApplicant = 0
	AND @statusApplication IS NULL OR ea.CurrentStatus = @statusApplication
END
GO



CREATE PROCEDURE [dbo].[event_getEventWithApplications_S]
	@eventId INT,
	@statusApplication INT = NULL
AS
BEGIN
	SELECT 
		e.Id,
		e.EventName,
		e.EventDescription,
		e.StartDate,
		e.EndDate,
		e.RecurringEvent,
		e.PlaceType,
		e.City,
		e.Street,
		e.NumberStreet,
		e.PostalCode,
		e.EventCategory,
		e.EventType,
		e.CurrentStatus,
		ea.Id,
		ea.PerformerId,
		ea.TypePerformance,
		ea.DurationInMinutes,
		ea.CurrentStatus,
		ea.LastModifiedByApplicant,
		ea.EventId 
	FROM [Events] AS e
	LEFT JOIN EventApplications AS ea ON ea.EventId = e.Id
	WHERE e.Id = @eventId 
	AND @statusApplication IS NULL OR ea.CurrentStatus = @statusApplication
END
GO




CREATE PROCEDURE [dbo].[event_getEventWithImages_S]
	@eventId INT
AS
BEGIN
	SELECT 
		e.Id,
		e.EventName,
		e.EventDescription,
		e.StartDate,
		e.EndDate,
		e.RecurringEvent,
		e.PlaceType,
		e.City,
		e.Street,
		e.NumberStreet,
		e.PostalCode,
		e.EventCategory,
		e.EventType,
		e.CurrentStatus,
		ei.Id,
		ei.EventId,
		ei.ImagePath,
		ei.ImageTitle,
		ei.IsMain,
		ei.ImageDescription
	FROM [Events] AS e
	LEFT JOIN EventImages AS ei ON ei.EventId = e.Id
	WHERE e.Id = @eventId
END
GO





CREATE PROCEDURE [dbo].[event_getEventWithOpinions_S]
	@eventId INT,
	@sortName VARCHAR(30) = NULL,
    @sortType VARCHAR(5) = NULL
AS 
BEGIN
	SELECT  
		e.Id,
		e.EventName,
		e.EventDescription,
		e.StartDate,
		e.EndDate,
		e.RecurringEvent,
		e.PlaceType,
		e.City,
		e.Street,
		e.NumberStreet,
		e.PostalCode,
		e.EventCategory,
		e.EventType,
		e.CurrentStatus,
		o.Id,
		o.EventId,
		o.Comment,
		o.Stars,
		o.UserId,
		au.UserName
	FROM [Events] AS e
	LEFT JOIN Opinions AS o ON o.EventId = e.Id
	LEFT JOIN ApplicationUsers AS au ON au.Id = o.UserId
	WHERE e.Id = @eventId 
	
	ORDER BY 

	CASE WHEN @sortName = 'OpinionId' AND @sortType = 'asc' THEN o.Id END ASC,  
	CASE WHEN @sortName = 'OpinionId' AND @sortType = 'desc' THEN o.Id END DESC,
	
	CASE WHEN @sortName = 'New' AND @sortType = 'asc' THEN o.Created END ASC,  
	CASE WHEN @sortName = 'New' AND @sortType = 'desc' THEN o.Created END DESC,
	 
	CASE WHEN @sortName = 'Stars' AND @sortType = 'asc' THEN o.Stars END ASC,  
	CASE WHEN @sortName = 'Stars' AND @sortType = 'desc' THEN o.Stars END DESC
END
GO




CREATE PROCEDURE [dbo].[event_getPerformersDataToNotifiedAboutStatusChange_S]
	@eventId INT
AS
BEGIN
	SELECT  
		ea.CurrentStatus,
		p.PerformerMail 
	FROM [Events] AS e
	INNER JOIN EventApplications AS ea ON ea.EventId = e.Id
	INNER JOIN Performers AS p ON p.Id = ea.PerformerId
	WHERE e.Id = @eventId 
	AND e.CurrentStatus != 3
	AND ea.CurrentStatus IN (1, 2, 4)
	AND ea.LastModifiedByApplicant = 0
END
GO






CREATE PROCEDURE [dbo].[event_updateEvent_U]
    @eventId INT,
    @eventDescription VARCHAR(MAX) = NULL,
    @startDate DATETIME = NULL,
    @endDate DATETIME = NULL,
    @recurringEvent BIT = NULL,
    @placeType INT = NULL,
    @eventCategory INT = NULL,
    @eventType INT = NULL,
    @currentStatus INT = NULL
AS
BEGIN
      UPDATE [Events]
        SET EventDescription = COALESCE(@eventDescription, EventDescription),
            StartDate = COALESCE(@startDate, StartDate),
            EndDate = COALESCE(@endDate, EndDate),
            RecurringEvent = COALESCE(@recurringEvent, RecurringEvent),
            EventCategory = COALESCE(@eventCategory, EventCategory),
            EventType = COALESCE(@eventType, EventType),
            CurrentStatus = COALESCE(@currentStatus, CurrentStatus),
			Modified = GETDATE()
        WHERE Id = @eventId	
END
GO




CREATE PROCEDURE [dbo].[image_createNewImage_I]
@eventId INT,
@isMain BIT,
@imagePath VARCHAR(MAX),
@imageTitle VARCHAR(50),
@description VARCHAR(50)
AS
BEGIN
	INSERT INTO EventImages(EventId, isMain, ImagePath, ImageTitle, ImageDescription)
	VALUES(@eventId, @isMain, @imagePath, @imageTitle, @description)
END
GO



CREATE PROCEDURE [dbo].[image_getImageById_S]
	@imageId INT
AS
BEGIN
	SELECT * FROM EventImages
	WHERE Id = @imageId
END
GO

CREATE PROCEDURE [dbo].[image_updateMainStatus_I]
	@imageId INT,
	@isMain BIT
AS
BEGIN
	--SET TRANSACTION ISOLATION LEVEL READ COMMITTED
	UPDATE EventImages 
	SET IsMain = @isMain,
		Modified = GETDATE()
	WHERE Id = @imageId
END
GO


CREATE PROCEDURE image_removeImage_D
@imageId INT
AS
BEGIN
	DELETE FROM EventImages
	WHERE Id = @imageId
END
GO

CREATE PROCEDURE [dbo].[logic_application_createNewEventApplication_I]
	@eventId INT,
	@performerId INT,
	@performerName VARCHAR(50),
	@typePerformance VARCHAR(50),
	@durationInMinutes INT,
	@currentStatus INT
AS
BEGIN
	BEGIN TRY
		/* -- dane mog¹ ulec usunieciu, wiêc mo¿liwe, ze idki nie bêd¹ siê zgadzaæ ;)
		EXEC logic_application_createNewEventApplication_I
		@eventId = 14,
		@performerId = 2,
		@performerName = 'Testowi_1',
		@typePerformance = 2,
		@durationInMinutes = 30,
		@currentStatus = 1
		*/

		IF(@durationInMinutes < 15 OR @durationInMinutes > 120)
		BEGIN 
			 RAISERROR('Column DurationInMinutes must be between 15 and 120', 11, 1)
		END

		DECLARE @eventStart DATETIME;
		DECLARE @eventStatus INT;
		DECLARE @performerIdentifier INT;
		DECLARE @name VARCHAR(30); 

		SELECT TOP(1) @performerIdentifier = Id, @name = PerformerName FROM Performers WHERE Id = @performerId
		IF(@@rowcount = 0)
			BEGIN
				RAISERROR('Performer not found.', 11, 1)
			END
	
		IF(@performerName != @name)
			BEGIN
				RAISERROR('Incorrect name %s', 11, 1, @performerName)
			END

		SELECT TOP(1) @eventStart = StartDate, @eventStatus = CurrentStatus FROM [Events] WHERE Id = @eventId
		 
		 IF (@@rowcount != 0)
			BEGIN
				IF(@eventStatus = 3)
					BEGIN
						RAISERROR('If the event status is "canceled" then the application cannot be added.', 11, 1)
					END
				
				IF(DATEADD(DAY, -7, @eventStart) < GETDATE())
					BEGIN
						RAISERROR('Too late to add a new application.', 11, 1)
					END

				DECLARE @created INT;

				MERGE EventApplications AS target
				USING (	SELECT Id FROM Performers WHERE Id = @performerId) AS source
				ON (source.Id = target.PerformerId AND target.EventId = @eventId)
				WHEN NOT MATCHED THEN INSERT 
					(EventId, PerformerId, PerformerName, TypePerformance, 
					DurationInMinutes, CurrentStatus, LastModifiedByApplicant)
				VALUES (@eventId, @performerId, @performerName, @typePerformance, @durationInMinutes, @currentStatus, 1);
			END
		  ELSE
			BEGIN
				RAISERROR('Event does not exist.', 11, 1)
			END
	END TRY
	BEGIN CATCH
	 -- Insert into db errors
	 DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
			 @Severity int = ERROR_SEVERITY(),
			 @State smallint = ERROR_STATE()
   RAISERROR (@Message, @Severity, @State)

  END CATCH
END
GO









CREATE PROCEDURE [dbo].[opinion_createNewOpinion_I]
@eventId INT,
@userId INT = NULL,
@comment VARCHAR(MAX),
@stars INT
AS
BEGIN
	INSERT INTO Opinions (EventId, UserId,Comment, Stars)
	VALUES (@eventId, @userId, @comment, @stars)
END
GO


CREATE PROCEDURE [dbo].[opinion_getById_S]
	@opinionId INT
AS
BEGIN
	SELECT * FROM Opinions
	WHERE Id = @opinionId
END
GO



CREATE PROCEDURE [dbo].[opinion_removeOpinion_D]
    @opinionId INT
AS
BEGIN
	DELETE Opinions
	WHERE Id = @opinionId 
END
GO

CREATE PROCEDURE [dbo].[opinion_updateOpinion_U]
    @opinionId INT,
    @userId INT,
    @comment VARCHAR(MAX) = NULL,
    @stars INT = NULL
AS
BEGIN
    UPDATE Opinions
    SET Comment = COALESCE(@comment, Comment),
        Stars = COALESCE(@stars, Stars),
		Modified = GETDATE()
    WHERE Id = @opinionId AND @userId = UserId 
END
GO

CREATE PROCEDURE [dbo].[performer_createNewPerformer_I]
	@userId INT,
	@performerName VARCHAR(50),
	@numberOfPeople INT,
	@performerMail VARCHAR(50),
	@vip BIT
AS
BEGIN
	MERGE Performers AS target
	USING(SELECT @userId, @performerName, @numberOfPeople, @performerMail, @vip) AS source(UserId, PerformerName, NumberOfPeople, PerformerMail, VIP)
	ON (target.UserId = source.UserId AND target.PerformerName = source.PerformerName)
	WHEN NOT MATCHED THEN
	INSERT (UserId, PerformerName, VIP, NumberOfPeople, PerformerMail) 
	VALUES (source.UserId, source.PerformerName, source.VIP, source.NumberOfPeople, source.PerformerMail);
END
GO




CREATE PROCEDURE [dbo].[performer_getAllPerformers_S]
AS
BEGIN
	SELECT * FROM Performers
END
GO


CREATE PROCEDURE [dbo].[performer_getByUserId_S]
	@userId INT
AS
BEGIN
	SELECT
		p.Id,
		p.UserId,
		p.PerformerName,
		p.VIP,
		p.NumberOfPeople,
		p.PerformerMail
	FROM ApplicationUsers AS au
	INNER JOIN Performers AS p ON p.UserId = au.Id
	WHERE au.Id = @userId
END
GO



CREATE PROCEDURE [dbo].[performer_getPerformersByEventId_S]
	@eventId INT
AS
BEGIN
	SELECT
		p.Id,
		p.UserId,
		p.PerformerName,
		p.PerformerMail,
		p.VIP,
		p.NumberOfPeople 
	FROM EventApplications AS ea
	INNER JOIN Performers AS p ON p.Id = ea.PerformerId
	WHERE ea.EventId = @eventId AND ea.CurrentStatus != 3
END
GO



CREATE PROCEDURE [dbo].[performer_getPerformersWithNumberPerformancesBySearch_S]
	@sortName VARCHAR(50) = NULL,
	@sortType VARCHAR(10),
	@performerName VARCHAR(50),
	@vip BIT = NULL
AS
BEGIN
	SELECT 
		p.Id,
		p.NumberOfPeople,
		p.VIP,
		p.PerformerName,
		p.PerformerMail,
		COUNT(ea.PerformerId) OVER(PARTITION BY ea.PerformerId) AS NumberPerformance
	FROM Performers AS p
	LEFT JOIN EventApplications AS ea ON ea.PerformerId = p.Id
	LEFT JOIN [Events] AS e ON e.Id = ea.EventId
	WHERE e.EndDate IS NULL OR e.EndDate < GETDATE() 
	AND ea.CurrentStatus = 4
	AND @performerName IS NULL OR p.PerformerName = @performerName
	AND @vip IS NULL OR p.VIP = @vip
	AND e.CurrentStatus != 3
	ORDER BY  
	
	CASE WHEN @sortName = 'PerformerName' AND @sortType = 'asc' THEN p.PerformerName END ASC,  
	CASE WHEN @sortName = 'PerformerName' AND @sortType = 'desc' THEN p.PerformerName END DESC,
	
	CASE WHEN @sortName = 'NumberOfPeople' AND @sortType = 'asc' THEN p.NumberOfPeople END ASC,  
	CASE WHEN @sortName = 'NumberOfPeople' AND @sortType = 'desc' THEN p.NumberOfPeople END DESC,
	 
	CASE WHEN @sortName = 'Vip' AND @sortType = 'asc' THEN p.VIP END ASC,  
	CASE WHEN @sortName = 'Vip' AND @sortType = 'desc' THEN p.VIP END DESC
END
GO


  CREATE PROCEDURE [dbo].[performer_getPerformerWithProposals_S]
	@performerId INT
  AS
  BEGIN
	SELECT * FROM Performers AS p
	LEFT JOIN PerformanceProposals AS pp ON pp.PerformerId = p.Id
	WHERE pp.Deleted IS NULL OR pp.Deleted != 1
  END
  GO

  CREATE PROCEDURE [dbo].[performer_updatePerformer_U]
	@performerId INT,
	@numberOfPeople INT = NULL,
	@performerMail VARCHAR(50) = NULL
AS
BEGIN
	UPDATE Performers
	SET NumberOfPeople = COALESCE(@numberOfPeople, NumberOfPeople),
		PerformerMail = COALESCE(@performerMail, PerformerMail)
	WHERE Id = @performerId
END
GO 


 CREATE PROCEDURE [dbo].[proposal_createProposal_I]
	@performerId INT,
	@eventId INT,
	@message VARCHAR(MAX),
	@activeTo DATETIME
  AS
  BEGIN
	INSERT INTO PerformanceProposals (PerformerId, EventId, [Message], ActiveTo)
	VALUES (@performerId, @eventId, @message, @activeTo)
  END
  GO


  CREATE PROCEDURE [dbo].[proposal_getPerformanceProposalsByPerformer_S]
	@performerId INT
AS
BEGIN
	SELECT * FROM PerformanceProposals 
	WHERE PerformerId = @performerId
	AND Deleted != 1
	ORDER BY Created DESC
END
GO

CREATE PROCEDURE [dbo].[proposal_getPerformanceProposalsByUser_S]
	@hasSuperAccess BIT,
	@userId INT
AS
BEGIN
	SELECT * FROM Performers AS p
	INNER JOIN PerformanceProposals AS pp ON pp.PerformerId = p.Id
	WHERE (@hasSuperAccess = 1 OR p.UserId = @userId)
	AND pp.Deleted != 1
	ORDER BY pp.Created DESC
END
GO


CREATE PROCEDURE [dbo].[proposal_removeExpiredProposals_D]
AS
BEGIN
	DELETE FROM PerformanceProposals
	WHERE ActiveTo < GETDATE();
END
GO

  CREATE PROCEDURE [dbo].[proposal_removeProposal_D]
	@proposalId INT
  AS
  BEGIN
	DELETE FROM PerformanceProposals
	WHERE Id = @proposalId 
  END
  GO

  CREATE PROCEDURE [dbo].[user_addToRole_I] 
	@userId INT, 
	@role INT 
AS 
BEGIN 
	INSERT INTO UserRoles(UserId, RoleId) 
	VALUES (@userId, @role) 
END
GO

CREATE PROCEDURE proposal_removeProposals_D
	@eventId INT
AS
BEGIN
	DELETE FROM PerformanceProposals
	WHERE EventId = @eventId
END
GO

  CREATE PROCEDURE [dbo].[user_clearRevokedTokens_D]
  AS
  BEGIN
	DELETE FROM RefreshTokens
	WHERE Revoked IS NOT NULL
  END
  GO


  
CREATE PROCEDURE [dbo].[user_createNewUser_I]
    --@roleId INT,
	@firstName VARCHAR(50),
	@lastName VARCHAR(50),
	@userName VARCHAR(50),
	@email VARCHAR(50),
	@phoneNumber VARCHAR(50),
	@passwordHash VARCHAR(MAX),
	@new_identity INT OUTPUT
AS
BEGIN 

	INSERT INTO ApplicationUsers(FirstName, LastName,
		UserName, Email, PhoneNumber, PasswordHash) 
		VALUES (@firstName, @lastName, @userName, 
		@email, @phoneNumber, @passwordHash) 
		
    SELECT @new_identity = CAST(SCOPE_IDENTITY() AS INT)

		-- Wczeœniejsza logika
	--IF EXISTS (SELECT* FROM Roles WHERE Id = @roleId) 
	--BEGIN
	--	DECLARE @userId INT;
	--	INSERT INTO ApplicationUsers(FirstName, LastName,
	--	UserName, Email, PhoneNumber, PasswordHash) 
	--	VALUES (@firstName, @lastName, @userName, 
	--	@email, @phoneNumber, @passwordHash) 
	--	SELECT @userId = CAST(SCOPE_IDENTITY() AS INT)
	--	INSERT INTO UserRoles(RoleId, UserId) VALUES(@roleId, @userId)
	--END
END
GO

CREATE PROCEDURE [dbo].[user_getUserByEmail_S]
	@email VARCHAR(50)
AS
BEGIN 
	SELECT 
	au.Id AS Id,
	au.FirstName,
	au.LastName,
	au.UserName,
	au.Email,
	au.PhoneNumber,
	au.PasswordHash,
	rt.Id,
	rt.Token,
	rt.Expires,
	rt.Created,
	rt.Revoked
	FROM ApplicationUsers AS au 
	LEFT JOIN RefreshTokens AS rt ON rt.UserId = au.Id 
	WHERE au.Email = @email
END
GO


CREATE PROCEDURE [dbo].[user_getUserByToken_S]
	@tokenKey VARCHAR(MAX)
AS
BEGIN 
	SELECT
	au.Id,
	au.FirstName,
	au.LastName,
	au.UserName,
	au.Email,
	au.PhoneNumber,
	au.PasswordHash,
	rt.Id,
	rt.Token,
	rt.Expires,
	rt.Created,
	rt.Revoked
	FROM RefreshTokens AS rt
	INNER JOIN ApplicationUsers AS au ON au.Id = rt.UserId
	WHERE rt.Token = @tokenKey
END
GO


CREATE PROCEDURE [dbo].[user_getUserRoles_S]
@userId INT
AS
BEGIN
	SELECT r.RoleName FROM UserRoles AS ur
	INNER JOIN Roles AS r ON r.Id = ur.RoleId
	WHERE ur.UserId = @userId
END
GO


CREATE PROCEDURE [dbo].[user_updateUserData_U]
	@userId INT,
	@email VARCHAR(50) = NULL,
	@phoneNumber VARCHAR(50) = NULL,
	@refreshTokens UserRefreshTokensTableType READONLY 
AS
BEGIN 
	BEGIN TRANSACTION

	MERGE RefreshTokens AS target
	USING (SELECT Token, Expires, Created, ReplacedByToken, Revoked FROM @refreshTokens) AS source
	ON (target.UserId = @userId AND target.Token = source.Token)
	WHEN MATCHED THEN 
		UPDATE SET Token = COALESCE(source.Token, target.Token),
		           Expires = CONVERT(DATETIME, source.Expires, 120), 
		           Created = CONVERT(DATETIME, source.Created, 120),
		           ReplacedByToken = COALESCE(source.ReplacedByToken, target.ReplacedByToken),
		           Revoked = COALESCE(source.Revoked, target.Revoked)
	WHEN NOT MATCHED THEN 
		INSERT(UserId, Token, Expires, Created, ReplacedByToken, Revoked) 
		VALUES(@userId, source.Token, source.Expires, source.Created,
		source.ReplacedByToken, source.Revoked);

	UPDATE ApplicationUsers 
	SET Email = COALESCE(@email, Email) , 
	PhoneNumber = COALESCE(@phoneNumber, PhoneNumber),
	Modified = GETDATE()
	WHERE Id = @userId

	COMMIT
END
GO
