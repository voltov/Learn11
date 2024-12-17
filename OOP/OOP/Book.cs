public class Book : Document
{
    public string ISBN { get; set; }
    public int NumberOfPages { get; set; }
    public string Publisher { get; set; }

    public override string DocumentNumber => ISBN;
}
