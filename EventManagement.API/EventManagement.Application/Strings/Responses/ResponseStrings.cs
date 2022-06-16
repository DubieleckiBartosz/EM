using System;
using System.Collections.Generic;

namespace EventManagement.Application.Strings.Responses
{
    public static class ResponseStrings
    {
        //Exception messages
        public const string MessageException = "One or more validation failures have occurred.";
        public const string ServerError = "Internal server error.";
        public const string DataNotFound = "Data not found.";
        public const string OperationFailed = "Operation failed.";
        public const string EventCannotBeNull = "Event cannot be null.";
        public static string InvalidType (string value) => $"{value} - invalid type.";

        //Event
        public const string EventCreated = "A new event has been created.";
        public const string EventUpdated = "Event has been updated.";

        //Event exceptions
        public const string EventNotFound = "Event not found.";
        public const string CreationNewEventFailed = "The creation of a new event has failed.";

        //Opinion
        public const string OpinionRemoved = "The opinion has been removed.";
        public const string OpinionAdded = "Opinion added.";
        public const string OpinionUpdated = "The opinion has been updated.";

        //Performer
        public const string NewPerformerRegistered = "The perforemer has been registered.";

        //Performer exceptions
        public const string UpdateFailed = "Update failed.";
        public const string CreatePerformerFailed = "Failed to create a new performer.";

        //Image
        public static string NewImageCreated(int eventId) => $"A new image has been added to event {eventId}";

        //Image exceptions
        public const string ImageUpdateFailed = "Image update failed.";
        public const string SaveImageFailed = "Failed to save a new image.";

        //Application exceptions
        public const string CreateNewApplicationFailed = "Creating a new application failed.";
        public const string ApplicationUpdateFailed = "Application update failed.";


        //Opinion exceptions
        public const string CreationNewOpinionFailed = "Failed to create a new opinion.";

        //Proposal exceptions
        public const string OldDateForProposal = "The date cannot be older than the beginning of the event.";

        //Authorization
        public static string UserExist(string email) => $"Email {email} is already registered.";
        public static string IncorrectCredentials(string email) => $"Incorrect credentials for user {email}.";
        public const string RegisterFailed = "Cannot create user, try again later";
        public const string RegisterSuccess = "User created!";
        public const string UserNotFound = "User not found.";
        public const string IncorrectData = "Incorrect data! Check data and try again.";
        public static string RoleNotFound(List<string> allRoles) =>
            $"Role not found. You can choose one of the roles: {string.Join($", {Environment.NewLine}", allRoles)}";
        public const string  UserAlreadyHasRole = "User has this role.";
        public const string  NewRoleForUserFailed = "Failed to create a new role for a user.";
        public const string  OperationSuccess = "The operation was successful.";
        public const string  RefreshTokenNotFound = "Refresh token was not found.";
        public const string RefreshTokenRevoked = "Refresh token was revoked.";
        public const string TokenNotMatch = "Token did not match any users.";
        public const string TokenNotActive = "Token is not active.";
        public const string TokenIsEmptyOrNull = "Token is null or empty.";
        public const string NoPermission = "You are not authorized to perform this operation!.";

    }
}