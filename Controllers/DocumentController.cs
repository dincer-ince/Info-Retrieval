using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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

        /*public double getNormTf(int tf, int termNum )
        {
            return tf / termNum;
        }*/
        [HttpGet("getNorm/{id1}")]
        public double[] getNormalizedTfIdfList(int id1)
        {
            var doc1 = _context.Documents.Find(id1);
            
            List<double> doc1Vector = new List<double>();
            
            var dictionary = _context.Dictionary.ToList();
            var docCount = _context.Documents.Count();

            foreach (var term in dictionary)
            {
                if (term.docID.Contains(doc1.docID))
                {
                    for (int i = 0; i < term.docID.Length; i++)
                    {
                        if (term.docID[i] == doc1.docID)
                        {
                            //doc1Vector.Add((term.tfPerDoc[i] / doc1.terms.Length) * (1+(Math.Log((_context.Documents.Count()/term.docID.Length),2))));
                            doc1Vector.Add(    (1+Math.Log10(term.tfPerDoc[i])) * (1 + (Math.Log10((docCount / term.docID.Length)))));
                        }
                    }
                }
                else
                {
                    doc1Vector.Add(0);
                }

                

                
            }


            return doc1Vector.ToArray();
        }

        [HttpGet("cosine-{id1}-{id2}")]
        public double getCosineSimilarity(int id1,int id2)
        {
            var vector1 = getNormalizedTfIdfList(id1);
            var vector2 = getNormalizedTfIdfList(id2);


            //Cosine Similarity(Document1,Document2) = Dot product(Document1,Document2) / ||Document1|| * ||Document2||
            double dotproduct = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                dotproduct= dotproduct+ vector1[i]*vector2[i];
            }

            double temp1 = 0;
            double temp2 = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                temp1 = temp1 + vector1[i] * vector1[i];
                temp2 = temp2 + vector2[i] * vector2[i];
            }

            double sqrtdocs =Math.Sqrt(temp1) * Math.Sqrt(temp2);

            return dotproduct / sqrtdocs;
        }
        [HttpGet("MostSimilarDocuments")]
        public String MostSimilarTwoDocuments()
        {
            var docs = _context.Documents.ToList();
            int[] docpair = new int[2];
            double tempsim = 0;
            for (int i = 0; i < docs.Count-1; i++)
            {
                for (int j = i+1; j < docs.Count; j++)
                {
                    if (docs[i].docID != docs[j].docID)
                    {
                        var a =getCosineSimilarity(docs[i].docID, docs[j].docID);
                        Console.WriteLine("{0:0.0000} for "+ docs[i].docID +" and "+ docs[j].docID,a);
                        if (a > tempsim)
                        {
                            tempsim = a;
                            docpair[0] = i;
                            docpair[1] = j;
                        }
                    }
                }
            }
            string result = "highest Cosine similarity between all documents= " + tempsim + " between documents " + (docpair[0]+1) + " and " + (docpair[1]+1) + ".\n Documents are: \n" + docs[docpair[0]].rawDocument + "\n********\n" + docs[docpair[1]].rawDocument;
            return result;
        }




        // GET: api/<DocumentController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Documents>>> GetDocuments()
        {
            return await _context.Documents.ToListAsync();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<Dictionary>>> UpdateDictionary(Documents doc)
        {
            
            var dictTerms = await _context.Dictionary.ToListAsync();
            foreach (var term in doc.terms)
            {
                if (dictTerms.Count == 0)
                {
                    var newTerm = new Dictionary();
                    newTerm.termId = 0;
                    newTerm.docID = new int[] { doc.docID };
                    newTerm.term = term;
                    newTerm.tfPerDoc = new double[] { 1 };
                    newTerm.df = 0;
                    _context.Dictionary.Add(newTerm);
                    _context.SaveChanges();
                    dictTerms = await _context.Dictionary.ToListAsync();
                }
                else
                {

                    foreach (var dictterm in dictTerms)
                    {
                        if (dictterm.term == term)
                        {
                            if (dictterm.docID.Contains(doc.docID))
                            {
                                Console.WriteLine(term + "buldu tf arttirdi");
                                for (int i = 0; i < dictterm.docID.Length; i++)
                                {
                                    if (dictterm.docID[i] == doc.docID)
                                    {
                                        dictterm.tfPerDoc[i] += 1;
                                        _context.Dictionary.Update(dictterm);
                                        _context.SaveChanges();
                                        dictTerms = await _context.Dictionary.ToListAsync();
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(term + "buldu icinde docIDsi yokmus ama docID.length=" + dictterm.docID.Length);
                                dictterm.docID = dictterm.docID.Append(doc.docID).ToArray();
                                dictterm.tfPerDoc = dictterm.tfPerDoc.Append(1).ToArray();
                                Console.WriteLine("simdi bu=" + dictterm.docID.Length);
                                _context.Dictionary.Update(dictterm);
                                _context.SaveChanges();
                                dictTerms = await _context.Dictionary.ToListAsync();
                            }
                            goto bos;
                        }

                    }



                    var newTerm = new Dictionary();
                    newTerm.termId = 0;
                    newTerm.docID = new int[] { doc.docID };
                    newTerm.term = term;
                    newTerm.tfPerDoc = new double[] { 1 };
                    newTerm.df = 0;
                    _context.Dictionary.Add(newTerm);
                    _context.SaveChanges();
                    dictTerms = await _context.Dictionary.ToListAsync();


                bos:
                    Console.WriteLine("test");

                }

            }
            _context.SaveChanges();
            Console.WriteLine("done");
            return await _context.Dictionary.ToListAsync();
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public void AddDocumentToDictionary(Documents doc)
        {
            var terms = doc.terms;
            foreach (var term in terms)
            {
                var dictTerm = CheckInDictionary(term);
                if (dictTerm.termId != -100)
                {

                }
                else
                {
                    var newTerm = new Dictionary();
                    newTerm.docID = new int[] { doc.docID };
                    newTerm.tfPerDoc = new double[] { 1 };
                    newTerm.term = term;
                    newTerm.df = 1;
                    _context.Dictionary.Add(newTerm);
                    _context.SaveChanges();
                }
            }


        }
        [HttpGet("check/{term}")]
        public Dictionary CheckInDictionary(string term)
        {
            try { var a = _context.Dictionary.Where(x => x.term == term).First();
                return a;
            }
            catch
            {
                var empty = new Dictionary();
                empty.termId = -100;
                return empty;
            }
            
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
            
            Document.terms= ProcessDocument(Document.rawDocument);
            await _context.SaveChangesAsync();

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
            var procDoc = ProcessDocument(Documents.rawDocument);
            Documents.terms = procDoc;
            _context.Documents.Add(Documents);
            await _context.SaveChangesAsync();
            await UpdateDictionary(Documents);
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