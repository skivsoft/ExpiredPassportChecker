namespace ExpiredPassportChecker.Settings
{
    public class MainSettings
    {
        public string SourceBaseUrl { get; set; }
        
        public string SourceFileName { get; set; }
        
        public int UnpackBufferSize { get; set; }
        public EnabledStepsInfo EnabledSteps { get; set; }
    }
}