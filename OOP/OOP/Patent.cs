public class Patent : Document
{
    public DateTime ExpirationDate { get; set; }
    public string UniqueId { get; set; }

    public override string DocumentNumber => UniqueId;
}
