using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BolittosApi.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(150)]
        public string Nome { get; set; } = null!;

        [MaxLength(1000)]
        public string? Descricao { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoCusto { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoVenda { get; set; }
    }
}
