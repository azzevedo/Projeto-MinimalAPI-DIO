using Microsoft.EntityFrameworkCore;


namespace EndpointsTestXUnit.Helpers;

public class DatabaseTest : ApiTestBase
{
    [Fact]
    public async Task Test_Database_Creation()
    {
        // Verifica se a tabela Administradores existe e tem dados
        var administradores = await db.Administradores.ToListAsync();

        Assert.NotNull(administradores);
        Assert.True(administradores.Count > 0);
    }
}