using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BolittosApi.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public DateTime DataInclusao { get; set; } = DateTime.UtcNow;

        [Required]
        public int ClienteId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        public Cliente? Cliente { get; set; }
        public ICollection<PedidoProduto>? Itens { get; set; }
    }
}
