using Microsoft.AspNetCore.Mvc;
using PetShop.Api.Database;
using PetShop.Api.Domain.Models.Responses.Pet;
using System.Net.Mime;

namespace PetShop.Api.Controllers
{
    /// <summary>
    /// Controller utilizado para operaçôes de CRUD de Pets
    /// </summary>
    [ApiController]
    [Route("api/pets")]
    [Consumes(MediaTypeNames.Application.Json)]
    public class PetsController : ControllerBase
    {
        private readonly IPetShopDbContext petShopDbContext;
        private readonly ILogger<PetsController> logger;

        public PetsController(IPetShopDbContext petShopDbContext, ILogger<PetsController> logger)
        {
            this.petShopDbContext = petShopDbContext;
            this.logger = logger;
        }

        /// <summary>
        /// Retorna as informações sobre o pet com o ID especificado.
        /// </summary>
        /// <param name="id">Id do Pet</param>
        /// <response code="200">Retorna os dados do pet, quando encontrado.</response>
        /// <response code="404">Pet não encontrado.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult GetById(int id)
        {
            logger.LogTrace("Iniciou o método GetById");
            var entity = petShopDbContext.Pets.Find(id);

            if (entity == null)
                return NotFound();

            var model = new GetPetByIdResponse
            {
                PetAnimalType = entity.AnimalType,
                PetName = entity.Name,
                PetBreed = entity.Breed
            };

            logger.LogTrace("Finalizou o método GetById");
            return Ok(model);
        }
    }
}
