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
    public class DocumentController : ControllerBase
    {
        private readonly documentsContext _context;
        public DocumentController(documentsContext context)
        {
            _context = context;
        }
        // GET: api/<DocumentController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Documents>>> GetDocuments()
        {
            return await _context.Documents.ToListAsync();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static string StripPunctuation(string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string[] ProcessDocument(string doc)
        {
            
            var filepath = Path.Combine(Environment.CurrentDirectory, "full7z-mlteast-en.lem");
            var stream = System.IO.File.OpenRead(filepath);
            var lemmatizer = new Lemmatizer(stream);
            doc = doc.RemoveStopWords("en");
            var docArr = doc.Split(' ');
            for (int i = 0; i < docArr.Length; i++)
            {
                docArr[i]= lemmatizer.Lemmatize(docArr[i].ToLower());
                docArr[i] = StripPunctuation(docArr[i]);

                Console.WriteLine(docArr[i]);
            }
            
            
            
            return docArr;
        }

        // GET: api/Documents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Documents>> GetDocuments(int id)
        {
            var Document = await _context.Documents.FindAsync(id);
            
            if (Document == null)
            {
                return NotFound();
            }
            ProcessDocument(Document.rawDocument);
            

            return Document;
        }

        // PUT: api/Documents/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocuments(int id, Documents Documents)
        {
            if (id != Documents.docID)
            {
                return BadRequest();
            }

            _context.Entry(Documents).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentsExists(id))
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

        // POST: api/Documents
        [HttpPost]
        public async Task<ActionResult<Documents>> PostDocuments(Documents Documents)
        {
            _context.Documents.Add(Documents);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocuments", new { id = Documents.docID }, Documents);
        }

        // DELETE: api/Documents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocuments(int id)
        {
            var Documents = await _context.Documents.FindAsync(id);
            if (Documents == null)
            {
                return NotFound();
            }

            _context.Documents.Remove(Documents);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentsExists(int id)
        {
            return _context.Documents.Any(e => e.docID == id);
        }
    }
}