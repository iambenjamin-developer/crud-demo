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

        /// <summary>
        /// Obtiene la lista de todos los productos disponibles.
        /// </summary>
        /// <remarks>
        /// Devuelve todos los productos registrados en el sistema.
        /// </remarks>
        /// <response code="200">Lista de productos obtenida correctamente.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var dtos = await _productService.GetAllAsync();
            return Ok(dtos);
        }

        /// <summary>
        /// Obtiene los detalles de un producto específico por su ID.
        /// </summary>
        /// <param name="id">ID del producto.</param>
        /// <remarks>
        /// Devuelve el producto que coincide con el ID proporcionado.
        /// </remarks>
        /// <response code="200">Producto encontrado.</response>
        /// <response code="404">Producto no encontrado.</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(long id)
        {
            var dto = await _productService.GetByIdAsync(id);
            if (dto == null)
                return NotFound($"Producto con Id: {id} no encontrado");

            return Ok(dto);
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="dto">Datos del producto a crear.</param>
        /// <remarks>
        /// Crea un producto en la base de datos. Valida que el SKU sea único y que la categoría exista.
        /// </remarks>
        /// <response code="201">Producto creado correctamente.</response>
        /// <response code="400">Datos inválidos o SKU/categoría no válidos.</response>
        /// <response code="500">Error inesperado.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        /// <param name="id">ID del producto a actualizar.</param>
        /// <param name="dto">Datos actualizados del producto.</param>
        /// <remarks>
        /// Actualiza la información del producto. Valida que el SKU sea único y que la categoría exista.
        /// </remarks>
        /// <response code="204">Actualización exitosa.</response>
        /// <response code="400">Datos inválidos o SKU/categoría no válidos.</response>
        /// <response code="404">Producto no encontrado.</response>
        /// <response code="500">Error inesperado.</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un producto por su ID.
        /// </summary>
        /// <param name="id">ID del producto a eliminar.</param>
        /// <remarks>
        /// Elimina el producto de la base de datos.
        /// </remarks>
        /// <response code="204">Eliminación exitosa.</response>
        /// <response code="404">Producto no encontrado.</response>
        /// <response code="500">Error inesperado.</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var isDeleted = await _productService.DeleteAsync(id);
                if (!isDeleted)
                    return NotFound($"Producto con Id: {id} no encontrado");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }
    }
}
