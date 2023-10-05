using FluentValidation;
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
                .Must(ValidaEmailUnico)
                    .WithMessage("E-mail já cadastrado")
                .EmailAddress()
                    .WithMessage("É obrigatório um e-mail válido");
            this.context = context;
        }

        private bool ValidaEmailUnico(string arg1)
        {
            var inUse = context.Employees.Any(x => x.Email == arg1);


            return !inUse;
        }
    }
}
