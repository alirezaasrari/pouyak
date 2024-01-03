namespace Pouyak
{
    public class ApiQueryParams
    {
        public string FromDate { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public bool Daily { get; set; }
        public bool Monthly { get; set; }
        public bool Yearly { get; set; }
        public bool CMUnit { get; set; }
        public string FlowMeterSerial { get; set; } = string.Empty;
    }
}
