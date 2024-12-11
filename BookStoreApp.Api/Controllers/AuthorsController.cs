using AutoMapper;
using BookStoreApp.Api.Data;
using BookStoreApp.Api.Models.Author;
using BookStoreApp.Api.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly BookStoreContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(BookStoreContext context, IMapper mapper, ILogger<AuthorsController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/Authors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorReadOnlyDto>>> GetAuthors()
    {
        _logger.LogInformation($"GET request for all authors received. - {nameof(GetAuthors)}");
        try
        {
            var authors = await _context.Authors.ToListAsync();
            var authorsDtos = _mapper.Map<IEnumerable<AuthorReadOnlyDto>>(authors);
            _logger.LogInformation($"GET request for all authors completed. - {nameof(GetAuthors)}");
            return Ok(authorsDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error perfoming GET in {nameof(GetAuthors)} - {ex.Message}");
            return StatusCode(500, Messages.Error500);
        }
    }

    // GET: api/Authors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorReadOnlyDto>> GetAuthor(int id)
    {
        _logger.LogInformation($"GET request for author with id {id} received. - {nameof(GetAuthor)}");
        try
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            var authorDto = _mapper.Map<AuthorReadOnlyDto>(author);
            _logger.LogInformation($"GET request for author with id {id} completed. - {nameof(GetAuthor)}");
            return Ok(authorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error perfoming GET in {nameof(GetAuthor)} - {ex.Message}");
            return StatusCode(500, Messages.Error500);
        }
    }

    // PUT: api/Authors/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAuthor(int id, AuthorReadOnlyDto authorDto)
    {
        _logger.LogInformation($"PUT request for author with id {id} received. - {nameof(PutAuthor)}");
        if (id != authorDto.Id)
        {
            _logger.LogWarning($"PUT request for author with id {id} failed. - {nameof(PutAuthor)}");
            return BadRequest();
        }

        var author = await _context.Authors.FindAsync(id);
        if (author == null)
        {
            _logger.LogWarning($"Author is null for id {id}. - {nameof(PutAuthor)}");
            return NotFound();
        }

        _mapper.Map(authorDto, author);
        _context.Entry(author).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"PUT request for author with id {id} completed. - {nameof(PutAuthor)}");
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await AuthorExists(id))
            {
                _logger.LogWarning($"{nameof(AuthorExists)} return false. - {nameof(PutAuthor)}");
                return NotFound();
            }
            else throw;
        }

        return NoContent();
    }

    // POST: api/Authors
    [HttpPost]
    public async Task<ActionResult<Author>> PostAuthor(AuthorCreateDto authorDto)
    {
        _logger.LogInformation($"POST request for author received. - {nameof(PostAuthor)}");
        try
        {
            var author = _mapper.Map<Author>(authorDto);
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"POST request for author completed. - {nameof(PostAuthor)}");
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error perfoming POST in {nameof(PostAuthor)} - {ex.Message}");
            return StatusCode(500, Messages.Error500);
        }
    }

    // DELETE: api/Authors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        _logger.LogInformation($"DELETE request for author with id {id} received. - {nameof(DeleteAuthor)}");
        try
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) {
                _logger.LogWarning($"Author is null for id {id}. - {nameof(DeleteAuthor)}");
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"DELETE request for author with id {id} completed. - {nameof(DeleteAuthor)}");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error perfoming DELETE in {nameof(DeleteAuthor)} - {ex.Message}");
            return StatusCode(500, Messages.Error500);
        }
    }

    private async Task<bool> AuthorExists(int id) => await _context.Authors.AnyAsync(e => e.Id == id);
}
