public class FishingTicket
{
    public int Id { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public decimal Price { get; set; }

    public int FishermanId { get; set; }

    public Fisherman Fisherman { get; set; }

    
}