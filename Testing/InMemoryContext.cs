using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;

namespace Testing;

public class InMemoryContext
{
    public static MeerkatContext GetMeerkatContext()
    {
        var options = new DbContextOptionsBuilder<MeerkatContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        
        return new MeerkatContext(options);
    }
}