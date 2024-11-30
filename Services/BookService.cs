using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class BookService
    {
        private readonly BookstoreContext _context;

        public BookService(BookstoreContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> FindAllAsync()
        {
            return await _context.Books.Include(x => x.Genres).ToListAsync();
        }

        public async Task<Book> FindByIdAsync(int id)
        {
            return await _context.Books.Include(x => x.Genres).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task InsertAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BookFormViewModel viewmodel)
        {
            bool hasAny = await _context.Books.AnyAsync(x => x.Id == viewmodel.Book.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id não encontrado");
            }

            try
            {
                Book? dbBook = await _context.Books.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == viewmodel.Book.Id);

                List<Genre> selectedGenres = new List<Genre>();

                foreach (int genreId in viewmodel.SelectedGenresIds)
                {
                    Genre genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == genreId);
                    if (genre is not null)
                    {
                        selectedGenres.Add(genre);
                    }
                }

                List<Genre> currentGenres = dbBook.Genres.ToList();

                List<Genre> genresToRemove = currentGenres.Where(current => !selectedGenres.Any(selected => selected.Id == current.Id)).ToList();

                List<Genre> genresToAdd = selectedGenres.Where(selected => !currentGenres.Any(current => current.Id == selected.Id)).ToList();

                foreach (Genre genre in genresToRemove)
                {
                    dbBook.Genres.Remove(genre);
                }

                foreach (Genre genre in genresToAdd)
                {
                    dbBook.Genres.Add(genre);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbConcurrencyException(ex.Message);
            }
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new IntegrityException(ex.Message);
            }
        }
    }
}
