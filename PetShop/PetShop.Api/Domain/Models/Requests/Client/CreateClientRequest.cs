using PetShop.Api.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace PetShop.Api.Domain.Models.Requests.Client
{
    public class CreateClientRequest : ClientBaseModel
    {
        [Required(ErrorMessage = "O campo 'Name' é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo 'Name' deve ter no máximo 50 caracteres.")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "O campo 'Email' é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo 'Email' deve ser um endereço de email válido.")]
        [StringLength(100, ErrorMessage = "O campo 'Email' deve ter no máximo 100 caracteres.")]
        public string ClientEmail { get; set; }

        [Required(ErrorMessage = "O campo 'Cpf' é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O campo 'Cpf' deve conter 11 dígitos numéricos.")]
        public string ClientCpf { get; set; }

    }
}
