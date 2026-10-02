using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Configuration;
using PetShop.Api.Database;
using PetShop.Api.Domain.Entities;
using PetShop.Api.Domain.Models.Base;
using PetShop.Api.Domain.Models.Requests.Employee;
using PetShop.Api.Domain.Models.Responses.Employee;
using PetShop.Api.Domain.Validators;
using System.Net.Mime;

namespace PetShop.Api.Controllers
{
    /// <summary>
    /// Controller utilizado para operaï¿½ï¿½es de CRUD de Colaboradores
    /// </summary>
    [ApiController]
    [Route("api/employees")]
    [Consumes(MediaTypeNames.Application.Json)]
    public class EmployeeController : ControllerBase
    {
        private readonly IPetShopDbContext petShopDbContext;
        private readonly ILogger<EmployeeController> logger;
        //PetShop.Api.Controllers.EmployeeController
        public EmployeeController(IPetShopDbContext petShopDbContext, ILogger<EmployeeController> logger)
        {
            this.petShopDbContext = petShopDbContext;
            this.logger = logger;
        }

        /// <summary>
        /// Retorna todos os Employees cadastrados.
        /// </summary>
        /// <response code="200">Colecao de Employees. Pode ser uma colecao 
        /// vazia caso nao existam employees cadastrados.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            logger.LogTrace("Iniciou o método Get");

            var employees = new GetEmployeeResponse();

            employees.Employees = await petShopDbContext.Employees
                .AsNoTracking()
                .Select(x => new EmployeeBaseModel
                {
                    EmployeeEmail = x.Email,
                    EmployeeName = x.Name
                })
                .ToListAsync(cancellationToken);

            logger.LogTrace("Finalizou o método Get");
            return Ok(employees);
        }

        /// <summary>
        /// Retorna as informacoes sobre o colaborador com id <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id do Colaborador</param>
        /// <response code="200">Retorna os dados do colaborador, quando encontrado.</response>
        /// <response code="404">Colaborador nï¿½o encontrado</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var entity = await petShopDbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
                return NotFound();

            var model = new GetEmployeeByIdResponse
            {
                EmployeeName = entity.Name,
                EmployeeEmail = entity.Email
            };

            return Ok(model);
        }

        /// <summary>
        /// Cria um novo Employee no banco de dados.
        /// </summary>
        /// <param name="request">Dados do Employee</param>
        /// <response code="201">Retorna o objeto recêm criado</response>
        /// <response code="400">Retorna um BadRequest se os dados da request são inválidos</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CreateEmployeeResponse), StatusCodes.Status201Created)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeRequest request, CancellationToken cancellationToken)
        {
            logger.LogTrace(LogEvents.PostEndpoint, "Iniciou o evento de Post Employee");

            var entity = new Employee
            {
                Name = request.EmployeeName,
                Email = request.EmployeeEmail
            };

            var validator = new EmployeeValidator(petShopDbContext);
            var validationResult = await validator.ValidateAsync(entity, cancellationToken);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(statusCode: StatusCodes.Status422UnprocessableEntity,
                    modelStateDictionary: ModelState);
            }

            petShopDbContext.Employees.Add(entity);
            await petShopDbContext.SaveChangesAsync(cancellationToken);

            var responseModel = new CreateEmployeeResponse
            {
                EmployeeName = request.EmployeeName,
                EmployeeEmail = request.EmployeeEmail,
            };

            logger.LogTrace(LogEvents.PostEndpoint, "Finalizou o evento de Post Employee");

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, responseModel);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeRequest request, CancellationToken cancellationToken)
        {
            var model = await petShopDbContext.Employees
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (model is null)
            {
                return BadRequest();
            }

            model.Name = request.EmployeeName;
            model.Email = request.EmployeeEmail;

            await petShopDbContext.SaveChangesAsync(cancellationToken);

            var entityResponse = new UpdateEmployeeResponse
            {
                EmployeeEmail = request.EmployeeEmail,
                EmployeeName = request.EmployeeName
            };

            return Ok(entityResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            logger.LogTrace("Iniciou o método Delete");

            var entity = await petShopDbContext.Employees.FindAsync(new object[] { id }, cancellationToken);

            if (entity is null)
            {
                return BadRequest();
            }

            logger.LogTrace(LogEvents.PostEndpoint, "Finalizou o evento de Delete Employee");

            petShopDbContext.Employees.Remove(entity);
            await petShopDbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}