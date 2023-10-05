using FluentValidation;
using PetShop.Api.Database;
using PetShop.Api.Domain.Entities;

namespace PetShop.Api.Domain.Validators
{

    public class ProductValidator : AbstractValidator<Product>
    {
        private readonly IPetShopDbContext context;

        public ProductValidator(IPetShopDbContext context)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 300)
                    .WithMessage("O nome do Produto deve conter entre 2 e 300 caracteres");

            RuleFor(x => x.BestBefore)
                .NotEmpty().WithMessage("A data de validade não pode estar vazia");
            this.context = context;
        }
    }
}
