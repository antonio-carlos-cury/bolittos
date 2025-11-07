using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BolittosApi.Models
{
    public class PedidoProduto
    {
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorUnitario { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        public Pedido? Pedido { get; set; }
        public Produto? Produto { get; set; }
    }
}
