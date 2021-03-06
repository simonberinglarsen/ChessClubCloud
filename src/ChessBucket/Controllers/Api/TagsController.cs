using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChessBucket.Data;
using ChessBucket.Models;

namespace ChessBucket.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Tags")]
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Tags
        [HttpGet]
        public IActionResult GetTags([FromQuery] string contains)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<Tag> tags;
            if (!string.IsNullOrWhiteSpace(contains))
                tags = _context.Tags.Where(m => m.Name.ToLower().Contains(contains.ToLower())).ToList();
            else
                tags = _context.Tags.ToList();

            if (!tags.Any())
            {
                return NotFound();
            }

            return Ok(tags);
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public IActionResult GetTag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Tag tag = _context.Tags.SingleOrDefault(m => m.Id == id);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }

      
        // PUT: api/Tags/5
        [HttpPut("{id}")]
        public IActionResult PutTag([FromRoute] int id, [FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tag.Id)
            {
                return BadRequest();
            }

            _context.Entry(tag).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tags
        [HttpPost]
        public IActionResult PostTag([FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tags.Add(tag);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TagExists(tag.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTag", new { id = tag.Id }, tag);
        }

        // DELETE: api/Tags/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gameTags = _context.GameTags.Where(gt => gt.Tag.Id == id).ToList();
            Tag tag = _context.Tags.SingleOrDefault(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.GameTags.RemoveRange(gameTags);
            _context.Tags.Remove(tag);
            _context.SaveChanges();

            return Ok(tag);
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }
    }
}