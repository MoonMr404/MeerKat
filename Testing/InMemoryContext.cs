using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;

namespace UnitTesting;

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