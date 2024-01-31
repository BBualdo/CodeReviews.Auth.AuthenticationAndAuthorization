﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using MvcMovies.Data;
using MvcMovies.Models;

namespace MvcMovies.Controllers
{
    public class MusicController : Controller
    {
        private readonly MoviesContext _context;
        private readonly DbLogger _dbLogger;

        public MusicController(MoviesContext context)
        {
            _context = context;
        }

        // GET: Music
        public async Task<IActionResult> Index(string searchArtist, string musicGenre)
        {
            IQueryable<string> genreQuery = from m in _context.Music
                                            orderby m.Genre
                                            select m.Genre;

            var music = from m in _context.Music
                        select m;

            if (!string.IsNullOrEmpty(searchArtist))
            {
                music = music.Where(s => s.Artist!.Contains(searchArtist));
            }

            if (!string.IsNullOrEmpty(musicGenre))
            {
                music = music.Where(x => x.Genre == musicGenre);
            }

            var musicGenreVM = new MusicGenreViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Music = await music.ToListAsync()
            };

            return View(musicGenreVM);
        }

        // GET: Music/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Music == null)
            {
                return NotFound();
            }

            var music = await _context.Music
                .FirstOrDefaultAsync(m => m.Id == id);
            if (music == null)
            {
                return NotFound();
            }

            return View(music);
        }

        // GET: Music/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Music/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Artist,AlbumName,ReleaseDate,Genre,Price")] Music music)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(music);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(music);
            }
            catch (Exception ex)
            {

                if (ex != null)
                {
                    _dbLogger.LogError(ex);
                }
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: Music/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Music == null)
            {
                return NotFound();
            }

            var music = await _context.Music.FindAsync(id);
            if (music == null)
            {
                return NotFound();
            }
            return View(music);
        }

        // POST: Music/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Artist,AlbumName,ReleaseDate,Genre,Price")] Music music)
        {
            try
            {
                if (id != music.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(music);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MusicExists(music.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(music);
            }
            catch (Exception ex)
            {

                if (ex != null)
                {
                    _dbLogger.LogError(ex);
                }
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: Music/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || _context.Music == null)
                {
                    return NotFound();
                }

                var music = await _context.Music
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (music == null)
                {
                    return NotFound();
                }

                return View(music);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    _dbLogger.LogError(ex);
                }
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: Music/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Music == null)
            {
                return Problem("Entity set 'MoviesContext.Music'  is null.");
            }
            var music = await _context.Music.FindAsync(id);
            if (music != null)
            {
                _context.Music.Remove(music);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusicExists(int id)
        {
            return (_context.Music?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
