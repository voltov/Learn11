public class Magazine : Document
{
    public string Publisher { get; set; }
    public int ReleaseNumber { get; set; }
    public DateTime PublishDate { get; set; }

    public override string DocumentNumber => ReleaseNumber.ToString();
}
