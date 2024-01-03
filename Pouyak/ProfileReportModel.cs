namespace Pouyak
{
    public class ProfileReportModel
    {
        public int Id { get; set; }
        public double? InstantFlow { get; set; }

        public double? TotalConsumption { get; set; }

        public string? PersianDateTimeProfile { get; set; }

        public string? GreDateTimeProfile { get; set; }
    }
}
