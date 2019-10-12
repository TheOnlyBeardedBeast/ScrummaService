using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScrummaService.Data;
using ScrummaService.Models;
using ScrummaService.ViewModels;

namespace ScrummaService.Controllers
{
    public class PokerController : Controller
    {
        public readonly ApplicationDbContext db;
        public PokerController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return Json(new { Message = "Hello world" });
            //return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "build", "index.html"), "text/HTML");
        }

        [HttpPost("RegisterGroup")]
        public async Task<IActionResult> RegisterGroup([FromBody]string password)
        {
            IPasswordHasher<Group> hasher = new PasswordHasher<Group>();

            Group group = new Group();

            var hashedPassword = hasher.HashPassword(group, password);

            group.Password = hashedPassword;

            db.Groups.Add(group);
            await db.SaveChangesAsync();

            return Ok(group.Id);
        }

        [HttpPost("VerifyGroup")]
        public async Task<IActionResult> VerifyGroup([FromBody]GroupVerificationViewModel groupVM)
        {
            Console.WriteLine("######################## "+ groupVM.Id);
            var groupToVerify = db.Groups.FirstOrDefault(group => group.Id == groupVM.Id);

            if (groupToVerify == null)
            {
                Console.WriteLine("###################### no group");
                return BadRequest();
                
            }

            IPasswordHasher<Group> hasher = new PasswordHasher<Group>();

            var passwordVerification = hasher.VerifyHashedPassword(groupToVerify, groupToVerify.Password,groupVM.Password);

            if (passwordVerification != PasswordVerificationResult.Success)
            {
                Console.WriteLine("################## bad verify");
                return BadRequest();
            }

            groupToVerify.UpdatedAt = DateTime.Now;
            db.Groups.Update(groupToVerify);
            await db.SaveChangesAsync();

            return Ok();
        }
    }
}
