﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using BookStore.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BookStore.Models.ViewModels;
using System.Diagnostics;
using BookStore.Services.Exceptions;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookService _service;
        private readonly GenreService _genreService;

        public BooksController(BookService service, GenreService genreService)
        {
            _service = service;
            _genreService = genreService;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _service.FindAllAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "o id não foi fornecido" });
            }

            var book = await _service.FindByIdAsync(id.Value);
            if (book is null)
            {
                return RedirectToAction(nameof(Error), new { message = "O id não foi encontrado." });
            }

            return View(book);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            List<Genre> genres = await _genreService.FindAllAsync();

            BookFormViewModel viewModel = new BookFormViewModel { Genres = genres };
            return View(viewModel);
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Genres = await _genreService.FindAllAsync();
                return View(viewModel);
            }

            viewModel.Book.Genres = new List<Genre>();

            foreach (int genreId in viewModel.SelectedGenresIds)
            {
                Genre genre = await _genreService.FindByIdAsync(genreId);
                if (genre is not null)
                {
                    viewModel.Book.Genres.Add(genre);
                }
            }

            await _service.InsertAsync(viewModel.Book);
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "o id não foi fornecido" });
            }

            var book = await _service.FindByIdAsync(id.Value);
            if (book is null)
            {
                return RedirectToAction(nameof(Error), new { message = "O id não foi encontrado." });
            }

            List<Genre> genres = await _genreService.FindAllAsync();
            BookFormViewModel viewModel = new BookFormViewModel { Book = book, Genres = genres };

            return View(viewModel);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id != viewModel.Book.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Id's não condizentes." });
            }

            try
            {
                await _service.UpdateAsync(viewModel);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "o id não foi fornecido" });
            }

            var book = await _service.FindByIdAsync(id.Value);
            if (book is null)
            {
                return RedirectToAction(nameof(Error), new { message = "O id não foi encontrado." });
            }

            return View(book);
        }

        // POST: Books/Delete/5
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
                return RedirectToAction(nameof(Error), new { message = ex.Message });
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
    }
}
