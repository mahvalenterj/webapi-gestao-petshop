using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Api.Database;
using PetShop.Api.Domain.Models.Base;
using PetShop.Api.Domain.Models.Responses;
using System.Net.Mime;

namespace PetShop.Api.Controllers
{
    [ApiController]
    [Route("api/estoque")]
    public class InventoryController : ControllerBase
    {
        private readonly IPetShopDbContext petShopDbContext;
        private readonly ILogger<InventoryController> logger;

        public InventoryController(IPetShopDbContext petShopDbContext, ILogger<InventoryController> logger)
        {
            this.petShopDbContext = petShopDbContext;
            this.logger = logger;
        }

        /// <summary>
        /// Retorna os itens do estoque com seus produtos e quantidades.
        /// </summary>
        /// <response code="200">Lista de itens do estoque. Pode ser uma lista vazia.</response>
        [HttpGet]
        [ProducesResponseType(typeof(GetInventoryResponse), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            logger.LogTrace("Iniciou o método Get");

            var entity = new GetInventoryResponse();

            entity.Inventory = await petShopDbContext.Inventory
                .AsNoTracking()
                .Select(x => new InventoryBaseModel
                {
                    Product = x.Product,
                    Quantity = x.Quantity,
                })
                .ToListAsync(cancellationToken);

            logger.LogTrace("Finalizou o método Get");
            return Ok(entity);
        }
    }
}
