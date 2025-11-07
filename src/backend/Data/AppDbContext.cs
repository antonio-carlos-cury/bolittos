using Microsoft.EntityFrameworkCore;
using BolittosApi.Models;

namespace BolittosApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) {}

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<PedidoProduto> PedidosProdutos => Set<PedidoProduto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.Nome).HasMaxLength(150).IsRequired();
                b.Property(c => c.Email).HasMaxLength(150).IsRequired();
                b.Property(c => c.Telefone).HasMaxLength(14).IsRequired();
                b.Property(c => c.Senha).HasMaxLength(20).IsRequired();
                b.Property(c => c.Hash).HasMaxLength(100).IsRequired();
                b.Property(c => c.DataCadastro)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP")
                 .IsRequired();
            });

            modelBuilder.Entity<Produto>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Nome).HasMaxLength(150).IsRequired();
                b.Property(p => p.Descricao).HasMaxLength(1000);
                b.Property(p => p.PrecoCusto).IsRequired();
                b.Property(p => p.PrecoVenda).IsRequired();
                b.Property(p => p.DataCadastro)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP")
                 .IsRequired();
            });

            modelBuilder.Entity<Pedido>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.DataInclusao)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP")
                 .IsRequired();
                b.Property(p => p.ValorTotal)
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

                b.HasOne(p => p.Cliente)
                 .WithMany()
                 .HasForeignKey(p => p.ClienteId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PedidoProduto>(b =>
            {
                b.HasKey(pp => new { pp.PedidoId, pp.ProdutoId });

                b.Property(pp => pp.Quantidade).IsRequired();
                b.Property(pp => pp.ValorUnitario)
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();
                b.Property(pp => pp.ValorTotal)
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

                b.HasOne(pp => pp.Pedido)
                 .WithMany(p => p.Itens)
                 .HasForeignKey(pp => pp.PedidoId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(pp => pp.Produto)
                 .WithMany()
                 .HasForeignKey(pp => pp.ProdutoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

