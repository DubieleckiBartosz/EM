--Password -> SuperUserTest$123

IF NOT EXISTS(SELECT *FROM ApplicationUsers WHERE Email = 'SuperUser@test.com')
BEGIN
	BEGIN TRANSACTION
	INSERT INTO ApplicationUsers(FirstName,LastName
	      ,UserName ,Email ,PasswordHash ,PhoneNumber)
	VALUES('SuperUser_FirstName_Test', 'SuperUser_LastName_Test', 'SuperUserTest', 'SuperUser@test.com',
	'AQAAAAEAACcQAAAAEHGkBawWyter77Zcdt95O+92us+fKqLXy3u0fQhvHaX3WIK44tpj2WCo8gE+Dm2NeA==', '111-222-333')

	DECLARE @userId INT = SCOPE_IDENTITY();
	INSERT INTO UserRoles (UserId, RoleId) VALUES (@userId, 1), (@userId, 2), (@userId, 3), (@userId, 4)
	COMMIT TRANSACTION;
END


