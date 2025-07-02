using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using API.Genius.Core.Models;

namespace API.Genius.Data.Mappings;

public class EntradaSaidaPlacaMapping : IEntityTypeConfiguration<EntradaSaidaPlaca>
{
    public void Configure(EntityTypeBuilder<EntradaSaidaPlaca> builder)
    {
        builder.ToTable("EntradaSaidaPlaca");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Placa)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(10);
        builder.Property(x => x.IdCamera)
            .IsRequired()
            .HasColumnType("INT");
        builder.Property(x => x.Data)
            .IsRequired()
            .HasColumnType("DATETIME");
        builder.Property(x => x.Status)
            .HasColumnType("INT");
        // Ignora gravação do status no INSERT (null)            
        builder.Property(x => x.Status)
            .Metadata.SetBeforeSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
        // Permite gravação do status no UPDATE
        builder.Property(x => x.Status)
            .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Save);
        builder.Property(x => x.ArquivoImagem)
            .HasColumnType("VARCHAR");
    }
}
