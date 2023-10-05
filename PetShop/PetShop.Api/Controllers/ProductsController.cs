using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Api.Database;
using PetShop.Api.Domain.Entities;
using PetShop.Api.Domain.Models.Requests.Product;
using PetShop.Api.Domain.Models.Responses.Product;
using PetShop.Api.Domain.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetShop.Api.Database;
using PetShop.Api.Domain.Models.Base;
using PetShop.Api.Domain.Models.Responses.Employee;
using PetShop.Api.Domain.Models.Responses.Product;
using System.Net.Mime;
using Microsoft.Extensions.Logging;

namespace PetShop.Api.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    [Consumes(MediaTypeNames.Application.Json)]
    public class ProductsController : ControllerBase

    {
        private readonly IPetShopDbContext petShopDbContext;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(IPetShopDbContext petShopDbContext, ILogger<ProductsController> logger)
        {
            this.petShopDbContext = petShopDbContext;
            this.logger = logger;
        }

        /// <summary>
        /// Cria um novo produto com base nos dados fornecidos.
        /// </summary>
        /// <param name="request">Dados do produto a serem criados.</param>
        /// <response code="201">Produto criado com sucesso.</response>
        /// <response code="400">Requisição inválida ou erro ao criar o produto.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            logger.LogTrace("Iniciou o método Post-Products");
            try
            {
                var entity = new Product
                {
                    Name = request.ProductName,
                    BestBefore = request.ProductBestBefore,
                    Price = request.ProductPrice,
                    Quantity = request.ProductQuantity,
                };

                var validator = new ProductValidator(petShopDbContext);
                var validationResult = validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    validationResult.AddToModelState(ModelState);
                    return ValidationProblem(statusCode: StatusCodes.Status422UnprocessableEntity,
                        modelStateDictionary: ModelState);
                }

                petShopDbContext.Products.Add(entity);
                petShopDbContext.SaveChanges();

                var responseModel = new CreateProductResponse
                {
                    ProductName = request.ProductName,
                    ProductBestBefore = request.ProductBestBefore
                };

                logger.LogTrace("Finalizou o método Post-Products");
                throw new Exception();
            }
            catch(Exception ex)
            {
                logger.LogTrace(ex, "Houve um erro no método Post-Products");
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult DeleteById(int id)
        {
            var entity = petShopDbContext.Products.Find(id);

            if (entity == null)
                return NotFound(new { Message = "Produto não encontrado." });

            try
            {
                entity.IsDeleted = true;

                petShopDbContext.SaveChanges();

                return Ok(new { Message = "Produto marcado como deletado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Erro ao marcar o produto como deletado.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna todos os produtos cadastrados.
        /// </summary>
        /// <response code="200">Lista com todos os produtos cadastrados. Pode ser uma lista 
        /// vazia caso nao haja produto cadastrado.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult GetProducts()
        {
            logger.LogTrace("Iniciou o método Get Products.");
            try
            {
                var products = new GetProductsResponse();

                products.Products = petShopDbContext.Products
                   .Select(x => new ProductBaseModel
                   {
                       ProductId = x.Id,
                       ProductName = x.Name,
                       ProductBestBefore = x.BestBefore
                   });
                logger.LogTrace("Finalizou o método Get Products.");
                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro no método Get Products.");
                throw;
            }
        }

        /// <summary>
        /// Retorna as informações do produto com o ID.
        /// </summary>
        /// <param name="id">Id do Produto</param>
        /// <response code="200">Retorna os dados do produto, quando encontrado.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult GetProductById(int id)
        {
            logger.LogTrace("Iniciou o método get");

            try
            {

                var entity = petShopDbContext.Products.Find(id);

                if (entity == null)
                    return NotFound();

                var model = new GetProductByIdResponse
                {
                    ProductName = entity.Name,
                    ProductId = entity.Id,
                    ProductBestBefore = entity.BestBefore,
                    ProductPrice = entity.Price,
                    ProductIsDeleted = entity.IsDeleted,
                    ProductQuantity = entity.Quantity
                };

                logger.LogTrace("Finalizou o método get");
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro no método GetProductById");
                throw;
            }
        }
    }
}

