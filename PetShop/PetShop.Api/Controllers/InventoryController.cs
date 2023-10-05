using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Api.Database;
using PetShop.Api.Domain.Entities;
using PetShop.Api.Domain.Models.Base;
using PetShop.Api.Domain.Models.Requests;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult Get()
        {
            logger.LogTrace("Iniciou o método Get");
            try
            {
                var entity = new Domain.Models.Responses.GetInventoryResponse();

                entity.Inventory = petShopDbContext.Inventory.Select(x => new InventoryBaseModel
                {
                    Product = x.Product,
                    Quantity = x.Quantity,
                });

                logger.LogTrace("Finalizou o método Get");

                return Ok(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro no método GET Inventory");
                throw;
            }

        }
    }
}
