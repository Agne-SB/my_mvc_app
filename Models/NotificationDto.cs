namespace MyMvcApp.Models
{
    public class NotificationDto
    {
        public int BestillingId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Selger { get; set; } = string.Empty;
        public string RefNo { get; set; } = string.Empty;
        public string? Plassering { get; set; }
        public DateTime? DatoLevert { get; set; }
        public string? Adresse { get; set; }
        public DateTime? MonteringDato { get; set; }
        public string? GrunnAvRetur { get; set; }
        public string? CustomText { get; set; }
    }
}
