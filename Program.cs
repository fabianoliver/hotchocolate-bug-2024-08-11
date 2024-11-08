using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);



builder.AddGraphQL()
    .AddDocumentFromString($@"
                type Subscription {{
                  bookAdded: Book!
                }}

                type Query {{
                    test: String!
                }}

                type Book {{
                  title: String
                }}

                directive @bug(test: Int!) on SUBSCRIPTION
            ")
    .BindRuntimeType<Subscription>()
    .BindRuntimeType<Query>();

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);

public record Book(string Title);

public class Subscription
{
    [Subscribe(With = nameof(GetStream))]
    public Book BookAdded([EventMessage] Book book) => book;

    private async IAsyncEnumerable<Book> GetStream([EnumeratorCancellation] CancellationToken ct = default)
    {
        var i = 0;
        while (!ct.IsCancellationRequested)
        {
            yield return new Book(i++.ToString());
            await Task.Delay(TimeSpan.FromSeconds(1), ct);
        }
    }
}

public class Query
{
    public string Test => "Hello";
}