using BookStore.Data;
using BookStore.Models;
using BookStore.Services.Exceptions;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
	public class GenreService
	{
		private readonly BookstoreContext _context;

		public GenreService(BookstoreContext context)
		{
			_context = context;
		}

        public async Task<List<Genre>> FindAllAsync()
		{
			return await _context.Genres.ToListAsync();
		}

		public async Task InsertAsync(Genre genre)
		{
			_context.Add(genre);
			await _context.SaveChangesAsync();
		}

		public async Task<Genre> FindByIdAsync(int id)
		{
			return await _context.Genres.FindAsync(id);
		}

		public async Task RemoveAsync(int id)
		{
			try
			{
				Genre genre = await _context.Genres.FindAsync(id);
				_context.Remove(genre);
				await _context.SaveChangesAsync();
            }
			catch (DbUpdateException ex)
			{
				throw new IntegrityException(ex.Message);
			}
		}
    }
}
