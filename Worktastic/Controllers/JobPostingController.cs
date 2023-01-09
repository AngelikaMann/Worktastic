using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using Worktastic.Data;
using Worktastic.Models;

namespace Worktastic.Controllers
{
    [Authorize]
    public class JobPostingController : Controller
    {
        //_context ist eine Variable type ApplicationDbContext
        private readonly ApplicationDbContext _context;

        //Wert bekommen über DependencyInjection, der in Startup konfiguriert
        // services.AddDbContext<ApplicationDbContext>(options =>
        //      options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        //und wird über Konstruktor übergeben

        //context- von DJ kommt,unsere tatsätzlichen datenbank context
        public JobPostingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var allPostings = _context.JobPostings.ToList();

                return View(allPostings);
            }


            var jobPostingsFromDb = _context.JobPostings.Where(x => x.OwnerUsername == User.Identity.Name).ToList();

            return View(jobPostingsFromDb);
        }

        public IActionResult CreateEditJobPosting(int id)
        {
            if (id != 0)
            {
                var jobPostingFromDb = _context.JobPostings.SingleOrDefault(x => x.Id == id);

                if ((jobPostingFromDb.OwnerUsername != User.Identity.Name) && !User.IsInRole("Admin"))
                {
                    return Unauthorized();
                }


                if (jobPostingFromDb != null)
                {
                    return View(jobPostingFromDb);
                }
                else
                {
                    return NotFound();
                }
            }
            return View();
        }

        public IActionResult CreateEditJob(JobPosting jobPosting, IFormFile file)
        {

            jobPosting.OwnerUsername = User.Identity.Name;

            if (file != null)
            {
                using (var ms = new MemoryStream())
                {

                    //bekommende file2 in der Stream kopieren
                    file.CopyTo(ms);
                    var bytes = ms.ToArray();
                    jobPosting.CompanyImage = bytes;
                }
            }
            //fählt Datenbankzugrief->
            //Datenbankzugrief haben über ApplicationDbContext,wo DbSet<JobPosting> steht und
            //mit using Microsoft.Extensions.DependencyInjection;
            //bekommt unsere App Worktastic Datenbankzugrief
            // services.AddDbContext<ApplicationDbContext>(options =>
            //      options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            // Denn holen über Konstruktor des Controller
            //Als Konstruktor configuriert, haben Datenbankzugrief und jetzt können neu jobPodting nachfühlen oder ändern 


            //add new job if not editing (new Id=0 hat, by Edit har PK Id)

            // new job oder editing
            if (jobPosting.Id == 0)
            {
                //wenn kein Id hat, dann added new job 

                _context.JobPostings.Add(jobPosting); //nur temporäl geändert, immer speichern

            }
            else
            {
                //_context.JobPostings.Update(jobPosting); leichte variante

                //ein jobPosting in der Tabelle JobPosting finden, der nur einzig (hat Wert oder null),
                //der, natürlich Id hat und dieser Id gleich ist mit Id, der als Parammeter bekam mit JobPosting jobPostind
                // jeder element in der Tabelle JobPosting nennen x
                var jobFromDb = _context.JobPostings.SingleOrDefault(x => x.Id == jobPosting.Id);

                if (jobFromDb == null)
                {
                    return NotFound();
                }
                jobFromDb.CompanyImage = jobPosting.CompanyImage;
                jobFromDb.CompanyName = jobPosting.CompanyName;
                jobFromDb.ContactMail = jobPosting.ContactMail;
                jobFromDb.ContactPhone = jobPosting.ContactPhone;
                jobFromDb.ContactWebsite = jobPosting.ContactWebsite;
                jobFromDb.Descriotion = jobPosting.Descriotion;
                jobFromDb.JobLocation = jobPosting.JobLocation;
                jobFromDb.JobTitle = jobPosting.JobTitle;
                jobFromDb.Salary = jobPosting.Salary;
                jobFromDb.StartDate = jobPosting.StartDate;
                jobFromDb.OwnerUsername = jobPosting.OwnerUsername;
            }

            //Todo write jobposting to db


            _context.SaveChanges();
            return RedirectToAction("Index"); // zu Index
        }
        [HttpPost]
        public IActionResult DeleteJobPostingById(int id)
        {
            if (id == 0)

                return BadRequest();
            var jobPostingFromDb = _context.JobPostings.SingleOrDefault(x => x.Id == id);
            if (jobPostingFromDb == null)

                return NotFound();

            _context.JobPostings.Remove(jobPostingFromDb);
            _context.SaveChanges();

            return Ok();
        }

    }
}
