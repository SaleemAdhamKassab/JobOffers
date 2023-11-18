namespace JobOffers.Configurations
{
    public class Helper
    {
        public struct Roles
        {
            public const string Admin = "Admin";
            public const string Publisher = "Publisher";
            public const string JobSeeker = "Job Seeker";
        }

        public struct UploadFile
        {
            public const string UserDefaultImage = "UserDefaultImage.png";
            public const string JobDefaultImage = "JobDefaultImage.jpg";
            public const int MaxAllowedSize = 1048576;
            public const string CvAllowedExtenstions = ".pdf";
            public static List<string> ImageAllowedExtenstions = new() { ".jpeg", ".jpg", ".png" };
        }

        public struct PageSize
        {
            public const int Default = 5;
            public const int Home = 8;
            public const int Candidates = 1;
        }
    }
}