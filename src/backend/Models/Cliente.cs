using System;
using System.ComponentModel.DataAnnotations;

namespace BolittosApi.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(150)]
        public string Nome { get; set; } = null!;

        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(11), MaxLength(14)]
        public string Telefone { get; set; } = null!;

        [Required, MinLength(6), MaxLength(20)]
        public string Senha { get; set; } = null!;
        
        [Required, MaxLength(100)]
        public string Hash { get; set; } = null!;
    }
}
