using TMS.Entity;

namespace TMS.Helper
{
    public static class MessageHelper
    {
        public const string UsernamePasswordIncorrect = "The email address or password is not correct.";
        public const string AccountInactive = "Your account is inactive.";
        public const string AccountNotVerified = "Your account is not verified.";
        public const string UserExists = "User is already exists.";
        public const string SaveError = "Error while saving record.";
        public const string DeleteError = "Error while delete record.";
        public const string ActiveCPA = "Client is not deleted because of it is used by another.";
        public const string DuplicateRecord = "Record already exists.";
        public const string SelectCPA = "Please select Client.";
        public const string SubProcessExists = "Process has existing subprocess";
        public const string UserUnavailable = "You are not active user please contact to administrator";
        public const string UpdatePermissionError = "Error occurred while update permission details";
        public const string SelectDefaultLandingPage = "Please select default landing page for role type permission";
        public const string PasswordResetLinkExpired = "The Password Reset Link Has Expired";
        public const string RoleExists = "Role type already exists.";
        public const string EmailNotRegistered = "Your email id is not registered";
        public const string ProjectsExists = "Project already exists.";
        public const string ProcessExists = "Process already exists.";
        public const string DuplicateValueInExcel = "Duplicate values are not added.";
        public const string InvalidExcelData = "Recoders are in valid in excel please varify.";
        public const string UserForRoleExists = "The role is already assigned to the user hence can't be deleted.";
        //public const string Invalidlink = "Invalid link.";
        public const string InvalidTime = "Your manual task maximum time limit 16 hours in a day.";

        public static ApiError SomethingWentWrong
        {
            get
            {
                return new ApiError { ErrorCode = "999", Error = "Something went wrong.", ErrorDetail = "Something went wrong." };
            }
        }
        public static ApiError Invalidlink
        {
            get
            {
                return new ApiError { ErrorCode = "1003", Error = "Invalid link.", ErrorDetail = "Invalid link." };
            }
        }
        public static ApiError LinkExpire
        {
            get
            {
                return new ApiError { ErrorCode = "1004", Error = "Link is Expired.", ErrorDetail = "Link is Expired." };
            }
        }

        public static ApiError InvalidCode
        {
            get
            {
                return new ApiError { ErrorCode = "1002", Error = "Invalid otp.", ErrorDetail = "Invalid otp." };
            }
        }
        public static ApiError CodeExpire
        {
            get
            {
                return new ApiError { ErrorCode = "1001", Error = "Otp expire.", ErrorDetail = "Otp expire." };
            }
        }
        public static ApiError CheckListProfile
        {
            get
            {
                return new ApiError { ErrorCode = "1005", Error = "Profile incomplete.", ErrorDetail = "Profile incomplete." };
            }
        }
        public static ApiError UnCannotEditSubmitChecklist
        {
            get
            {
                return new ApiError { ErrorCode = "1006", Error = "You cannot edit submitted checklist.", ErrorDetail = "You cannot edit submitted checklist." };
            }
        }
        public static ApiError FileUploadFailed
        {
            get
            {
                return new ApiError { ErrorCode = "1007", Error = "File upload failed.", ErrorDetail = "File upload failed." };
            }
        }
        public static ApiError GetFileFailed
        {
            get
            {
                return new ApiError { ErrorCode = "1008", Error = "Error while getting file.", ErrorDetail = "Error while getting file." };
            }
        }
        public static ApiError RemoveFileFailed
        {
            get
            {
                return new ApiError { ErrorCode = "1010", Error = "File remove failed.", ErrorDetail = "File remove failed." };
            }
        }
        public static ApiError NoDataFound
        {
            get
            {
                return new ApiError { ErrorCode = "121", Error = "No Data Found.", ErrorDetail = "No Data Found." };
            }
        }

        #region DMS Error Messages
        public static ApiError FolderCreationError
        {
            get
            {
                return new ApiError { ErrorCode = "181", Error = "Error while creating folder.", ErrorDetail = "Error while creating folder." };
            }
        }
        public static ApiError RenameError
        {
            get
            {
                return new ApiError { ErrorCode = "182", Error = "Error while renaming.", ErrorDetail = "Error while renaming." };
            }
        }
        public static ApiError DeleteFileError
        {
            get
            {
                return new ApiError { ErrorCode = "183", Error = "Error while deleting.", ErrorDetail = "Error while deleting." };
            }
        }
        public static ApiError RestoreFileError
        {
            get
            {
                return new ApiError { ErrorCode = "184", Error = "Error while restoring.", ErrorDetail = "Error while restoring." };
            }
        }
        public static ApiError MoveFileError
        {
            get
            {
                return new ApiError { ErrorCode = "185", Error = "Error while moving file.", ErrorDetail = "Error while moving file." };
            }
        }
        public static ApiError ManageTagError
        {
            get
            {
                return new ApiError { ErrorCode = "186", Error = "Error while adding tags.", ErrorDetail = "Error while adding tags." };
            }
        }
        public static ApiError NameExist(string text)
        {
            return new ApiError
            {
                ErrorCode = "187",
                Error = string.Format("{0} name already exist.", text),
                ErrorDetail = string.Format("{0} name already exist.", text)
            };
        }
        #endregion
    }
}
