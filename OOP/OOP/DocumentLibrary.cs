public class DocumentLibrary
{
    private readonly List<Document> _documents = new List<Document>();

    public void AddDocument(Document document)
    {
        _documents.Add(document);
    }

    public IEnumerable<Document> SearchByDocumentNumber(string documentNumber)
    {
        return _documents.Where(d => d.DocumentNumber == documentNumber);
    }
}
