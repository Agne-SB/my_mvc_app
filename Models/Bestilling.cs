namespace MyMvcApp.Models
{
    public class Bestilling
    {
        public int Id { get; set; }
        public string? RefNo { get; set; }
        public string? Selger { get; set; }
        public string? Varegruppe { get; set; }
        public string? InfoOmVare { get; set; }
        public DateTime SendesFraLager { get; set; }
        public string? Status { get; set; }
        public string? Plassering { get; set; }
        public DateTime? DatoLevert { get; set; }
        public DateTime? DatoMontering { get; set; }
        public bool OppdragStartet { get; set; }
        public bool KundeInformert { get; set; }
        public string? Note { get; set; }
        public string? GrunnAvRetur { get; set; }
        public bool OutletLagtUt { get; set; }
        public bool Solgt { get; set; }
        public bool KastetUt { get; set; }

    }
}
