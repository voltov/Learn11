public abstract class Document
{
    public abstract string DocumentNumber { get; }
    public string Title { get; set; }
    public string[] Authors { get; set; }
    public DateTime DatePublished { get; set; }
}
