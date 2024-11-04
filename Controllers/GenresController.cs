﻿using BookStore.Data;
using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class GenresController : Controller
    {
        private readonly GenreService _service;

		public GenresController(GenreService service)
		{
			_service = service;
		}

		public IActionResult Index()
        {
            return View(_service.FindAll());
        }
    }
}
