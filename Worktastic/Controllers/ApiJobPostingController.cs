using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Worktastic.Data;
using Worktastic.Filters;
using Worktastic.Models;

namespace Worktastic.Controllers
{
    [Route("api/jobposting")]
    [ApiController]
    [ApiKeyAuthorization]
    public class ApiJobPostingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ApiJobPostingController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var allJobPosting = _context.JobPostings.ToArray();

            return Ok(allJobPosting);
        }
        [ApiKeyAuthorization]
        [HttpGet("GetById")]
        public IActionResult GetById(int id)
        {
            var jobPosting = _context.JobPostings.SingleOrDefault(p => p.Id == id);

            if (jobPosting == null)
                return NotFound();

            return Ok(jobPosting);
        }

        [HttpPost("Create")]
        public IActionResult Create(JobPosting jobposting)
        {
            if (jobposting.Id != 0)
                return BadRequest();
            _context.JobPostings.Add(jobposting);
            _context.SaveChanges();

            return Ok();
        }


        [HttpDelete("Delete")]

        public IActionResult Delete(int id)
        {
            var jobPosting = _context.JobPostings.SingleOrDefault(p => p.Id == id);

            if (jobPosting == null)
                return NotFound();
            _context.JobPostings.Remove(jobPosting);
            _context.SaveChanges();
            return Ok("Objekt wurde gelöscht.");

        }

        [HttpPut("Update")]

        public IActionResult Update(JobPosting jobPosting)
        {


            if (jobPosting.Id == 0)
                return BadRequest("Has no Value Id.");

            _context.JobPostings.Update(jobPosting);
            _context.SaveChanges();
            return Ok("Objekt geändert und gespeichert!");
        }

    }
}
