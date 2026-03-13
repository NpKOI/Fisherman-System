public class FishingPermit
{
    public int Id { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsRevoked { get; set; }

    public int ShipId { get; set; }

    public Ship Ship { get; set; }

    public int CaptainId { get; set; }

    public Fisherman Captain { get; set; }


}