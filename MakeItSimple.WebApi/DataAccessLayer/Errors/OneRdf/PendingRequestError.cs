using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.OneRdf
{
    public class PendingRequestError
    {
        public static Error EmptyIdPrefix() =>
        new("PendingRequest.EmptyidPrefix", "Pending Account employee prefix is required!");
        public static Error EmptyIdNo() =>
        new("PendingRequest.EmptyIdNo", "Pending Account employee Id is required!");
        public static Error EmptyUsername() =>
        new("PendingRequest.EmptyUsername", "Pending Account username is required!");
        public static Error EmptyPassword() =>
        new("PendingRequest.EmptyPassword", "Pending Account password is required!");
        public static Error EmptyFirstName() =>
        new("PendingRequest.EmptyFirstName", "Pending Account first name is required!");
        public static Error EmptyLastName() =>
        new("PendingRequest.EmptyLastName", "Pending Account last name is required!");
        public static Error EmptyMiddleName() =>
        new("PendingRequest.EmptyMiddleName", "Pending Account middle name is required!");


        public static Error UserNameExists() =>
        new("PendingRequest.UserNameExists", "Username already exists.");

        public static Error EmpIdExists() =>
        new("PendingRequest.EmpIdExists", "Employee Id already exists.");

        public static Error PendingUserError() =>
        new("PendingRequest.PendingUserError", "Error in Pending Users.");

        public static Error UpdatedCredentials() =>
        new("PendingRequest.PendingUserError", "Error in Pending Users.");
    }
}
