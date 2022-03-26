using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoRetrieval.Models;
using LemmaSharp.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StopWord;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InfoRetrieval.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly documentsContext _context;
        public DictionaryController(documentsContext context)
        {
            _context = context;
        }
        // GET: api/<DocumentController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dictionary>>> GetDictionary()
        {
            var a = _context.Dictionary.ToListAsync();
            return await a;
        }
        [HttpGet("termms")]
        public async Task<ActionResult<IEnumerable<string>>> GetTerms()
        {
            var a = _context.Dictionary.Select(x => x.term).ToListAsync();
            return await a;
        }

        

        

        // GET: api/Dictionary/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dictionary>> GetDictionary(int id)
        {
            var Document = await _context.Dictionary.FindAsync(id);

            if (Document == null)
            {
                return NotFound();
            }

            

            return Document;
        }



        // PUT: api/Dictionary/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDictionary(int id, Dictionary Dictionary)
        {
            if (id != Dictionary.termId)
            {
                return BadRequest();
            }

            _context.Entry(Dictionary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DictionaryExists(id))
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

        // POST: api/Dictionary
        [HttpPost]
        public async Task<ActionResult<Dictionary>> PostDictionary(Dictionary Dictionary)
        {
            
            _context.Dictionary.Add(Dictionary);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDictionary", new { id = Dictionary.termId }, Dictionary);
        }

        // DELETE: api/Dictionary/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDictionary(int id)
        {
            var Dictionary = await _context.Dictionary.FindAsync(id);
            if (Dictionary == null)
            {
                return NotFound();
            }

            _context.Dictionary.Remove(Dictionary);
            _context.Dictionary.RemoveRange(_context.Dictionary);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DictionaryExists(int id)
        {
            return _context.Dictionary.Any(e => e.termId == id);
        }
    }
}
