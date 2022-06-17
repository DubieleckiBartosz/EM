namespace EM.IntegrationTests.Setup.Constants
{
    public class Clear
    {
        public const string Delete_Tokens_UserRoles_AppUsers = @"BEGIN TRANSACTION 
																 DELETE FROM UserRoles 
																 WHERE UserId != 1 
																 DELETE FROM RefreshTokens 
																 DELETE FROM ApplicationUsers 
																 WHERE Email != 'SuperUser@test.com'  
																 COMMIT TRANSACTION";

        public const string Delete_Events = @"DELETE FROM Events";
        public const string Delete_EventApplications_Performers_Events_AppUsers = @"DELETE FROM EventApplications
                                                                                    DELETE FROM Performers
                                                                                    DELETE FROM [Events]
                                                                                    DELETE FROM ApplicationUsers 
                                                                                    WHERE Email != 'SuperUser@test.com'";
    }
}
