using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return Ok(genres); 
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre(CreateGenreDto dto)
        {
            var genre = new Genre { Name = dto.Name};
            await _context.AddAsync(genre);

            _context.SaveChanges();

            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id,[FromBody]CreateGenreDto dto)
        { 
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null) 
                return NotFound($"no genre was found with id:{id}");

            genre.Name = dto.Name;
            _context.SaveChanges();

            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> UDeleteGenre(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
                return NotFound($"no genre was found with id:{id}");

            _context.Remove(genre);
            _context.SaveChanges();

            return Ok(genre);
        }
    }
}