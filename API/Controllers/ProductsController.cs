using Application.DTOs.Products;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var dtos = await _productService.GetAllAsync();
            return Ok(dtos);
        }


        [HttpGet("{id:long}")]
        public async Task<ActionResult<ProductDto>> GetById(long id)
        {
            var dto = await _productService.GetByIdAsync(id);
            if (dto == null)
                return NotFound($"Producto con Id: {id} no encontrado");

            return Ok(dto);
        }


        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
        {
            try
            {
                var newProductDto = await _productService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = newProductDto.Id }, newProductDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                bool isUpdated = await _productService.UpdateAsync(id, dto);
                if (!isUpdated)
                    return NotFound($"Producto con Id: {id} no encontrado");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var isDeleted = await _productService.DeleteAsync(id);
            if (!isDeleted)
                return NotFound($"Producto con Id: {id} no encontrado");

            return NoContent();
        }
    }
}
