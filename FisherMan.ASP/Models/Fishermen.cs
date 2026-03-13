public class Fisherman
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime BirthDate { get; set; }


    // Navigation properties
    public ICollection<FishingTicket> Tickets { get; set; }

    public ICollection<Ship> OwnedShips { get; set; }

    public ICollection<FishingPermit> CaptainPermits { get; set; }
}