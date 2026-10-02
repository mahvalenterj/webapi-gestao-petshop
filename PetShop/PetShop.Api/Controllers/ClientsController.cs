using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Api.Database;
using PetShop.Api.Domain.Entities;
using PetShop.Api.Domain.Models.Base;
using PetShop.Api.Domain.Models.Requests.Client;
using PetShop.Api.Domain.Models.Responses;
using PetShop.Api.Domain.Models.Responses.Client;
using PetShop.Api.Domain.Models.Responses.Employee;
using System.Net.Mime;

namespace PetShop.Api.Controllers
{
    [ApiController]
    [Route("api/cliente")]
    [Consumes(MediaTypeNames.Application.Json)]
    public class ClientsController : ControllerBase
    {
        private readonly IPetShopDbContext petShopDbContext;
        private readonly ILogger<ClientsController> logger;

        public ClientsController(IPetShopDbContext petShopDbContext, ILogger<ClientsController> logger)
        {
            this.petShopDbContext = petShopDbContext;
            this.logger = logger;
        }

        /// <summary>
        /// Retorna todos os Clientes cadastrados.
        /// </summary>
        /// <response code="200">Retorna uma colecao de todos os Clientes. Retorna colecao vazia caso não tenha clientes cadastrados.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            logger.LogTrace("Iniciou o método Get");

            var clients = new GetClientResponse();

            clients.Clients = await petShopDbContext.Clients
                .AsNoTracking()
                .Select(x => new ClientBaseModel
                {
                    ClientName = x.Name,
                    ClientEmail = x.Email,
                    ClientCpf = x.Cpf
                })
                .ToListAsync(cancellationToken);

            logger.LogTrace("Finalizou o método Get");
            return Ok(clients);
        }

        /// <summary>
        /// Retorna as informações sobre o cliente com o ID especificado.
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <response code="200">Retorna os dados do cliente quando encontrado.</response>
        /// <response code="404">Cliente não encontrado.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            logger.LogTrace("Iniciou o método Get por Id");

            var entity = await petShopDbContext.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
                return NotFound();

            var model = new GetClientByIdResponse
            {
                ClientName = entity.Name,
                ClientEmail = entity.Email,
                ClientCpf = entity.Cpf,
                ClientPets = entity.Pets
            };

            logger.LogTrace("Finalizou o método Get por Id");
            return Ok(model);
        }

        /// <summary>
        /// Cria um novo cliente com base nos dados fornecidos.
        /// </summary>
        /// <param name="request">Dados do cliente a serem criados.</param>
        /// <response code="201">Cliente criado com sucesso.</response>
        /// <response code="400">Requisição inválida ou erro ao criar o cliente.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest request, CancellationToken cancellationToken)
        {
            var entity = new Client
            {
                Name = request.ClientName,
                Email = request.ClientEmail,
                Cpf = request.ClientCpf
            };

            petShopDbContext.Clients.Add(entity);

            try
            {
                await petShopDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Erro ao criar o cliente.");
            }

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient(UpdateClientRequest request, CancellationToken cancellationToken)
        {
            logger.LogTrace("Iniciou o método Update");

            var model = await petShopDbContext.Clients
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (model is null)
            {
                return BadRequest();
            }

            model.Name = request.ClientName;
            model.Email = request.ClientEmail;

            await petShopDbContext.SaveChangesAsync(cancellationToken);

            var entityResponse = new UpdateClientResponse
            {
                ClientEmail = request.ClientEmail,
                ClientName = request.ClientName
            };

            logger.LogTrace("Finalizou o método Update");
            return Ok(entityResponse);
        }

        /// <summary>
        /// Deleta o Cliente cadastrado pelo Id.
        /// </summary>
        /// <response code="200">Retorna que um cliente foi excluido.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await petShopDbContext.Clients.FindAsync(new object[] { id }, cancellationToken);

            if (entity is null)
            {
                return BadRequest();
            }

            petShopDbContext.Clients.Remove(entity);
            await petShopDbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}
