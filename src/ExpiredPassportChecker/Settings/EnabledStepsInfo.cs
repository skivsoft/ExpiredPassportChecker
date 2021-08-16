namespace ExpiredPassportChecker.Settings
{
    public class EnabledStepsInfo
    {
        public bool DownloadBzip2 { get; set; }
        
        public bool UnpackFromBzip2 { get; set; }
        
        public bool PackCsvToPassportData { get; set; }
        
        public bool RepackBzip2ToPassportData { get; set; }
    }
}
