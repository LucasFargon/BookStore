using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Services;
using BookStore.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace BookStore.Controllers
{
    public class GenresController : Controller
    {
        private readonly GenreService _service;
        
		public GenresController(GenreService service)
		{
			_service = service;
		}

		public async Task<IActionResult> Index()
        {
            return View(await _service.FindAllAsync());
        }

        // Genre/Create/x
        public IActionResult Create()
        {
            return View();
        }

        // POST Genre/Create/x
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            await _service.InsertAsync(genre);

            return RedirectToAction(nameof(Index));
        }

        // Genre/Delete/x
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "O id não foi fornecido."});
            }
            Genre genre = await _service.FindByIdAsync(id.Value);
            if (genre is null)
            {
                return RedirectToAction(nameof(Error), new { message = "O id não foi encontrado." });
            }
            
            return View(genre);
        }


        // POST Genre/Delete/x
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException ex)
            {
                return RedirectToAction(nameof(Error), new {message =  ex.Message});
            }
        }


		public IActionResult Error(string message)
		{
			var viewModel = new ErrorViewModel
			{
				Message = message,
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
			};
			return View(viewModel);
		}

        // Genre/Edit/x
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "o id não foi fornecido" });
            }
            Genre genre = await _service.FindByIdAsync(id.Value);
            if (genre is null)
            {
                return RedirectToAction(nameof(Error), new { message = "O id não foi encontrado." });
            }

            return View(genre);
        }


        // POST Genre/Edit/x
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id != genre.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Id's não condizentes" });
            }

            try
            {
                await _service.UpdateAsync(genre);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }
	}
}
