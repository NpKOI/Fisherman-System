public class Ship
{
    public int Id { get; set; }

    public string InternationalNumber { get; set; }

    public string CallSign { get; set; }

    public string Marking { get; set; }

    public double Length { get; set; }

    public double Width { get; set; }

    public double Tonnage { get; set; }

    public double EnginePower { get; set; }

    public string FuelType { get; set; }

    public int OwnerId { get; set; }

    public Fisherman Owner { get; set; }

    public ICollection<FishingPermit> Permits { get; set; }
}