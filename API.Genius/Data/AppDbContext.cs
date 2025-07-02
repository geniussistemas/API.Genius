using API.Genius.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace API.Genius.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EntradaSaidaPlaca> EntradaSaidaPlaca { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Adiciona configurações para todas as classes que implementam
        // IEntityTypeConfiguration no Assembly atual
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //        base.OnModelCreating(modelBuilder);

        // Informa o EF que a tabela EntradaSaidaPlaca possui uma trigger
        // Com isso o EF trabalhe num modo menos eficiente de acesso (antigo)
        // A partir da versão do EF Core 7.0 um modo mais eficiente foi 
        // implementado, mas ele exige que a tabela não tenha triggers ou 
        // certas colunas computadas
        // ver https://learn.microsoft.com/pt-br/ef/core/what-is-new/ef-core-7.0/breaking-changes?tabs=v7#sqlserver-tables-with-triggers
        modelBuilder.Entity<EntradaSaidaPlaca>()
            .ToTable(tb => tb.HasTrigger("PROC_DELETA_LOG_3_MESES"));

    }
}
