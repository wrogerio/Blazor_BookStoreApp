using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreApp.Api.Data;
using BookStoreApp.Api.Models.Book;
using BookStoreApp.Api.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly BookStoreContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<BooksController> _logger;

    public BooksController(BookStoreContext context, IMapper mapper, ILogger<BooksController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookReadOnlyDto>>> GetBooks()
    {
        _logger.LogInformation($"GET request for all books received. - {nameof(GetBooks)}");
        try
        {
            var books = await _context.Books.Include(a => a.Author).ToListAsync();
            var booksDtos = _mapper.Map<IEnumerable<BookReadOnlyDto>>(books);
            _logger.LogInformation($"GET request for all books completed. - {nameof(GetBooks)}");
            return Ok(booksDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error perfoming GET in {nameof(GetBooks)} - {ex.Message}");
            return StatusCode(500, Messages.Error500);
        }
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDetailDto>> GetBook(int id)
    {
        _logger.LogInformation($"GET request for book with id {id} received. - {nameof(GetBook)}");
        try
        {
            var book = await _context.Books
                .Include(x => x.Author)
                .ProjectTo<BookDetailDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (book == null) return NotFound();

            _logger.LogInformation($"GET request for book with id {id} completed. - {nameof(GetBook)}");
            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error perfoming GET in {nameof(GetBook)} - {ex.Message}");
            return StatusCode(500, Messages.Error500);
        }
    }

    // PUT: api/Books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
    {
        _logger.LogInformation($"PUT request for book with id {id} received. - {nameof(PutBook)}");
        if (id != bookDto.Id)
        {
            _logger.LogWarning($"PUT request for book with id {id} failed. - {nameof(PutBook)}");
            return BadRequest();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            _logger.LogWarning($"Book is null for id {id}. - {nameof(PutBook)}");
            return NotFound();
        }

        _mapper.Map(bookDto, book);
        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"PUT request for book with id {id} completed. - {nameof(PutBook)}");
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await BookExists(id))
            {
                _logger.LogWarning($"{nameof(BookExists)} return false. - {nameof(PutBook)}");
                return NotFound();
            }
            else throw;
        }

        return NoContent();
    }

    // POST: api/Books
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(BookCreateDto bookDto)
    {
        _logger.LogInformation($"POST request for book received. - {nameof(PostBook)}");
        try
        {
            var book = _mapper.Map<Book>(bookDto);
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"POST request for book completed. - {nameof(PostBook)}");
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error perfoming POST in {nameof(PostBook)} - {ex.Message}");
            return StatusCode(500, Messages.Error500);
        }
    }

    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        _logger.LogInformation($"DELETE request for book with id {id} received. - {nameof(DeleteBook)}");
        try
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) {
                _logger.LogWarning($"Book is null for id {id}. - {nameof(DeleteBook)}");
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"DELETE request for book with id {id} completed. - {nameof(DeleteBook)}");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error perfoming DELETE in {nameof(DeleteBook)} - {ex.Message}");
            return StatusCode(500, Messages.Error500);
        }
    }

    private async Task<bool> BookExists(int id) => await _context.Books.AnyAsync(e => e.Id == id);
}
