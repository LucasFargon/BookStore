using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data
{
	public class BookstoreContext : DbContext
	{
		public BookstoreContext(DbContextOptions<BookstoreContext> options) : base(options)
		{
		}

        public DbSet<Genre> Genres { get; set; }
		public DbSet<Book> Books { get; set; }
    }
}
