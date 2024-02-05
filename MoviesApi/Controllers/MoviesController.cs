using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private new List<string> _Extentions = new List<string> { ".jpg", ".png" };
        private long _MaxSize = 1048576;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Movies = await _context.Movies
                .OrderByDescending(g => g.Year)
                .Include(m => m.Genre)
                .ToListAsync();

            return Ok(Movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Movies = await _context.Movies
                //.FindAsync(id);
                .Include(m => m.Genre)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (Movies == null)
                return BadRequest($"there is no Movies with this id ({id})");
                

            return Ok(Movies);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreId(byte id)
        {
            var Movies = await _context.Movies
                .Where(g => g.GenreId == id)
                .OrderByDescending(g => g.Year)
                .Include(m => m.Genre)
                .ToListAsync();

            if (Movies == null)
                return BadRequest($"there is no Movies with this id ({id})");


            return Ok(Movies);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie([FromForm] MovieDto dto)
        {
            if (!_Extentions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("only .png or .jpg images are allowed!");

            if (dto.Poster.Length > _MaxSize)
                return BadRequest("Max allowed size for poster is 1MB!");

            var IsvalidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!IsvalidGenre) return BadRequest($"Invalid genre ID ({dto.GenreId})!");

            using var datastream = new MemoryStream();
            await dto.Poster.CopyToAsync(datastream);
          
            var movie = new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Year = dto.Year,
                Rate = dto.Rate,
                Storeline = dto.Storeline,
                Poster = datastream.ToArray(),
            };
            await _context.AddAsync(movie);

            _context.SaveChanges();

            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.SingleOrDefaultAsync(x => x.Id == id);

            if (movie == null) return NotFound($"there is no movie with this id ({id})");

            _context.Remove(movie);
            _context.SaveChanges();

            return Ok(movie);
        }

    }
}
