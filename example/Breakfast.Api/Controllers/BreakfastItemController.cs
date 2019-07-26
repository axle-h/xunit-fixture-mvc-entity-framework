using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Breakfast.Api.Data;
using Breakfast.Api.Entities;
using Breakfast.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Breakfast.Api.Controllers
{
    [Route("api/[controller]")]
    public class BreakfastItemController : Controller
    {
        private readonly BreakfastContext _context;

        public BreakfastItemController(BreakfastContext context)
        {
            _context = context;
        }

        [HttpGet]
        public Task<List<BreakfastItem>> GetAll() => _context.BreakfastItems.ToListAsync();

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var item = await _context.BreakfastItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<BreakfastItem> Post([FromBody] CreateOrUpdateBreakfastItemRequest request)
        {
            var item = new BreakfastItem
                       {
                           Name = request.Name,
                           Rating = request.Rating
                       };
            await _context.BreakfastItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] CreateOrUpdateBreakfastItemRequest request)
        {
            var existing = await _context.BreakfastItems.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = request.Name;
            existing.Rating = request.Rating;
            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] CreateOrUpdateBreakfastItemRequest request)
        {
            var existing = await _context.BreakfastItems.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (request.Name != null)
            {
                existing.Name = request.Name;
            }

            if (request.Rating.HasValue)
            {
                existing.Rating = request.Rating;
            }

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.BreakfastItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }
    }
}
