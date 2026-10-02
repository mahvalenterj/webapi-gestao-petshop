using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PetShop.Api.Database;
using PetShop.Api.Domain.Entities;

namespace PetShop.Api.Domain.Validators
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        private readonly IPetShopDbContext context;

        public EmployeeValidator(IPetShopDbContext context)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 300)
                    .WithMessage("O nome do Employee deve conter entre 3 e 300 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty()
                .MustAsync(ValidaEmailUnico)
                    .WithMessage("E-mail já cadastrado")
                .EmailAddress()
                    .WithMessage("É obrigatório um e-mail válido");
            this.context = context;
        }

        private async Task<bool> ValidaEmailUnico(string? email, CancellationToken cancellationToken)
        {
            var inUse = await context.Employees.AnyAsync(x => x.Email == email, cancellationToken);

            return !inUse;
        }
    }
}
