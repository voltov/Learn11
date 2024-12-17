using Newtonsoft.Json;

class Program
{
    private static Dictionary<string, CacheEntry<Document>> _cache = new Dictionary<string, CacheEntry<Document>>();
    private static Dictionary<string, TimeSpan> _cacheDurations = new Dictionary<string, TimeSpan>
    {
        { "Book", TimeSpan.FromMinutes(10) },
        { "LocalizedBook", TimeSpan.FromMinutes(5) },
        { "Patent", TimeSpan.Zero }, // Infinite cache duration
        { "Magazine", TimeSpan.FromMinutes(15) }
    };

    static void Main(string[] args)
    {
        string baseDirectory = AppContext.BaseDirectory;
        string libraryPath = Path.Combine(baseDirectory, @"..\..\..\library");

        while (true)
        {
            Console.WriteLine("Enter document number to search (or type 'exit' to quit):");
            string documentNumber = Console.ReadLine();

            if (documentNumber.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            var document = GetDocumentFromCacheOrFile(libraryPath, documentNumber);

            if (document != null)
            {
                Console.WriteLine($"Found Document: {document.Title}");
                Console.WriteLine($"Document Number: {document.DocumentNumber}");
                if (document.Authors != null)
                {
                    Console.WriteLine($"Authors: {string.Join(", ", document.Authors)}");
                }
                Console.WriteLine($"Date Published: {document.DatePublished.ToShortDateString()}");
                if (document is Book book)
                {
                    Console.WriteLine($"ISBN: {book.ISBN}");
                    Console.WriteLine($"Number of Pages: {book.NumberOfPages}");
                    Console.WriteLine($"Publisher: {book.Publisher}");
                }
                if (document is LocalizedBook localizedBook)
                {
                    Console.WriteLine($"Original Publisher: {localizedBook.OriginalPublisher}");
                    Console.WriteLine($"Country of Localization: {localizedBook.CountryOfLocalization}");
                    Console.WriteLine($"Local Publisher: {localizedBook.LocalPublisher}");
                }
                if (document is Patent patent)
                {
                    Console.WriteLine($"Expiration Date: {patent.ExpirationDate.ToShortDateString()}");
                    Console.WriteLine($"Unique ID: {patent.UniqueId}");
                }
                if (document is Magazine magazine)
                {
                    Console.WriteLine($"Publisher: {magazine.Publisher}");
                    Console.WriteLine($"Release Number: {magazine.ReleaseNumber}");
                    Console.WriteLine($"Publish Date: {magazine.PublishDate.ToShortDateString()}");
                }

                Console.WriteLine("========================================\n");
            }
            else
            {
                Console.WriteLine("Document not found.");
            }
        }
    }

    private static Document GetDocumentFromCacheOrFile(string libraryPath, string documentNumber)
    {
        if (_cache.ContainsKey(documentNumber))
        {
            var cacheEntry = _cache[documentNumber];
            if (cacheEntry.Expiration > DateTime.Now)
            {
                return cacheEntry.Value;
            }
            else
            {
                _cache.Remove(documentNumber);
            }
        }

        if (Directory.Exists(libraryPath))
        {
            foreach (var filePath in Directory.GetFiles(libraryPath, "*.json"))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var documentType = fileName.Split('_')[0];

                var jsonData = File.ReadAllText(filePath);
                Document document = null;

                switch (documentType)
                {
                    case "Book":
                        document = JsonConvert.DeserializeObject<Book>(jsonData);
                        break;
                    case "LocalizedBook":
                        document = JsonConvert.DeserializeObject<LocalizedBook>(jsonData);
                        break;
                    case "Patent":
                        document = JsonConvert.DeserializeObject<Patent>(jsonData);
                        break;
                    case "Magazine":
                        document = JsonConvert.DeserializeObject<Magazine>(jsonData);
                        break;
                }

                if (document != null && document.DocumentNumber == documentNumber)
                {
                    if (_cacheDurations.ContainsKey(documentType))
                    {
                        DateTime expiration;
                        if (_cacheDurations[documentType] == TimeSpan.Zero)
                        {
                            expiration = DateTime.MaxValue; // Infinite cache duration
                        }
                        else
                        {
                            expiration = DateTime.Now.Add(_cacheDurations[documentType]);
                        }
                        _cache[documentNumber] = new CacheEntry<Document> { Value = document, Expiration = expiration };
                    }
                    return document;
                }
            }
        }
        else
        {
            Console.WriteLine($"Directory '{libraryPath}' does not exist.");
        }

        return null;
    }
}
