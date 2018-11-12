// // Developed by Softeq Development Corporation
// // http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling
{
    public static class ErrorCode
    {
        public static string UnknownError { get; set; } = "internal_error";
        public static string NotFound { get; set; } = "not_found_error";
        public static string BadRequest { get; set; } = "bad_request_error";
        public static string ForbiddenError { get; set; } = "forbidden_error";
        public static string DuplicateError { get; set; } = "duplicate_error";
        public static string ConflictError { get; set; } = "conflict_error";
        public static string LimmitedAttachmentsError { get; set; } = "limited_attachments_error";
    }
}