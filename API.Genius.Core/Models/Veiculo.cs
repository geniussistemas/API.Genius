namespace API.Genius.Core.Models;

public class Veiculo
{
    // TODO: Complementar com as demais informações do veículo
    public long Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
    public int CameraId { get; set; }
    public DateTime? EntryDateTime { get; set; }
    public string EntryImage { get; set; } = string.Empty;
}
