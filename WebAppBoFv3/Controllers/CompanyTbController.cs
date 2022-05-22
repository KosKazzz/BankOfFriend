using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppBoFv3.Models;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace WebAppBoFv3.Controllers
{
    public class CompanyTbController : Controller
    {
        public ApplicationContext _context;
        public CompanyTbController(ApplicationContext context)
        {
            _context = context;
        }
        //Index
        [Authorize(Roles = "admin")]
        [HttpGet]//
        public IActionResult IndexComp()
        {
            return View(_context.Companies);
        }
        public IList<Company> SearchCompanies { get; set; }
        public string SearchString { get; set; }
        public List<Company> SearchCompList { get; set; }

        //[HttpPost]// заработает ли???
        [HttpGet]
        public async Task<IActionResult> SearchResult(string SearchString)
        {
            
            //List<Company> infosList = new List<Company>();

            //var infos = from m in _context.Companies //
            //            select m;
            if (!string.IsNullOrEmpty(SearchString))
            {
                //infos = infos.Where(s => s.Info.Contains(SearchString));
                //infosList = await infos.ToListAsync();
                //SearchCompList.AddRange(infosList);
                 SeachFromDB(SearchString);
                return View(SearchCompList);
            }
            //SearchCompanies = await infos.ToListAsync();
            return  RedirectToAction("EmptySearchString", "Home");//SearchCompanies.ToList() 
        }

        public  List<Company> SeachFromDB(string SearchString) // переделать в асинх???
        {
            List<Company> SomeCompsList = new List<Company>();
           
            var SomeComps = from m in _context.Companies select m;
            SomeComps = SomeComps.Where(s => s.Org_name.Contains(SearchString));
            SomeCompsList = SomeComps.ToList();
            SearchCompList=SomeCompsList;

            var SomeComps1 = from m in _context.Companies select m;
            SomeComps1 = SomeComps1.Where(s => s.Org_phone.Contains(SearchString));
            SomeCompsList = SomeComps1.ToList();
            SearchCompList = SearchCompList.Union(SomeCompsList).ToList();

            var SomeComps2 = from m in _context.Companies select m;
            SomeComps2 = SomeComps2.Where(s => s.Org_email.Contains(SearchString));
            SomeCompsList = SomeComps2.ToList();
            SearchCompList = SearchCompList.Union(SomeCompsList).ToList();

            var SomeComps3 = from m in _context.Companies select m;
            SomeComps3 = SomeComps3.Where(s => s.Info.Contains(SearchString));
            SomeCompsList = SomeComps3.ToList();
            SearchCompList = SearchCompList.Union(SomeCompsList).ToList();

            return SearchCompList;
        }
        //--------------------------------------------------------------------------------------
        // Edit -  
        [Authorize(Roles = "admin, user")]
        [HttpGet]
        public async Task<IActionResult> EditComp(int? id, string? UserName)
        {
            if (id == null & UserName == null)
            {
                return NotFound();
            }
            Company SomeCompany = await _context.Companies.FirstOrDefaultAsync(m => m.Id == id);// нашли в бд компани
            if (SomeCompany == null)
            {
                return NotFound();
            }
            User SomeUser = await _context.Users.FirstOrDefaultAsync(u=>u.Email == UserName); // нашли в бд пользователя
            if (SomeUser != SomeCompany.User)
            {
                return RedirectToAction("NoAccess", "Home");
            }
            return View(SomeCompany);
        }
        [Authorize(Roles = "admin, user")]
        [HttpPost] 
        public async Task<IActionResult> EditComp(Company SomeCompany)
        {
            #region
            Company model = new Company
            {
                Id = SomeCompany.Id,// Надо оно тут???
                Org_name = SomeCompany.Org_name,
                Region = SomeCompany.Region,
                INN = SomeCompany.INN,
                KPP = SomeCompany.KPP,
                OGRN = SomeCompany.OGRN,
                Account_number = SomeCompany.Account_number,
                Org_phone = SomeCompany.Org_phone,
                Org_email = SomeCompany.Org_email,
                Info = SomeCompany.Info,
                Surname = SomeCompany.Surname,
                Name = SomeCompany.Name,
                Patronymic = SomeCompany.Patronymic,
                Phone = SomeCompany.Phone,
                Email = SomeCompany.Email,
                Image = SomeCompany.Image
            };
            #endregion
            if (!ModelState.IsValid)
            {
                return View();
            }
            _context.Attach(model).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Ready", "CompanyTb"); //
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }

        //-------------------------------------------------------------------------------------

        //Create
        [Authorize(Roles = "admin, user")]
        [HttpGet] 
        public IActionResult CreateComp()
        {
            return View();
        }

        [Authorize(Roles = "admin, user")]
        [HttpPost]
        public async Task<IActionResult> CreateComp(CompAvatar compAvatar, string UserName) //
        {
            if (UserName == null)
            {
                return RedirectToAction("Error", "Home");// 
            }
            User SomeUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == UserName);
            Company model = new Company
            {
                Id = compAvatar.Id,// Надо оно тут???
                Org_name = compAvatar.Org_name,
                Region = compAvatar.Region,
                INN = compAvatar.INN,
                KPP = compAvatar.KPP,
                OGRN = compAvatar.OGRN,
                Account_number = compAvatar.Account_number,
                Org_phone = compAvatar.Org_phone,
                Org_email = compAvatar.Org_email,
                Info = compAvatar.Info,
                Surname = compAvatar.Surname,
                Name = compAvatar.Name,
                Patronymic = compAvatar.Patronymic,
                Phone = compAvatar.Phone,
                Email = compAvatar.Email,// 
                Image = null,
                User = SomeUser
            };

            // var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                                .Where(x => x.Value.Errors.Count > 0)
                                .Select(x => new { x.Key, x.Value.Errors })
                                .ToArray();
                return RedirectToAction("Error", "Home");// 
            }
            if (compAvatar.Avatar != null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(compAvatar.Avatar.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)compAvatar.Avatar.Length);
                }
                model.Image = imageData;
            }
            _context.Companies.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Ready", "CompanyTb");// 
        }

        #region
        // Autofilling --------------------------------------------------------------

        public async Task<IActionResult> Autofilling()
        {
            // Test 
            // Создаём поток , заталкиваем в него картинку , отдаём её в IFormFile

            string path = @"H:\Csharp\WebAppBoFv3(МАЙ)\OLD\WebAppBoFv3\wwwroot\Test.jpg";

            var stream = System.IO.File.OpenRead(path);

            var pic = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

            CompAvatar testModel = new CompAvatar
            {
                // Id = compAvatar.Id,// Надо оно тут???
                Org_name = "Test",
                Region = "Test",
                INN = 11111111111111,
                KPP = 11111111111111,
                OGRN = 11111111111111,
                Account_number = 11111111111111,
                Org_phone = "0-000-000-00-00",
                Org_email = "Test@test.fu",
                Info = "При использовании встроенных шаблонов returnUrl заполняется автоматически только в случае, если вы пытаетесь получить доступ к авторизованному ресурсу",
                Surname = "Test",
                Name = "Test",
                Patronymic = "Test",
                Phone = "0-000-000-00-00",
                Email = "Test@test.fu",
                Avatar = pic
            };

            await CreateComp(testModel,"admin@mail.ru");
            stream.Close();
            return RedirectToAction("IndexComp", "CompanyTb");
        }
        #endregion

        // Deteils ----------------------------------------------------------------
        //[Authorize(Roles = "admin, user")]
        public async Task<IActionResult> DetailsComp(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Company ThisCompany = await _context.Companies.FirstOrDefaultAsync(p => p.Id == id);
            if (ThisCompany == null)
            {
                return NotFound();
            }
            return View(ThisCompany);
        }
        // Delete ---------------------------------
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> DeleteComp(int? id, string UserName) // 
        {
            if (id == null & UserName==null)
            {
                return NotFound();
            }
            User SomeUser = await _context.Users.FirstOrDefaultAsync(u=>u.Email==UserName);
            Company ThisCompany = await _context.Companies.FirstOrDefaultAsync(p => p.Id == id);
            if (ThisCompany == null & SomeUser==null)
            {
                return NotFound();
            }
            if (SomeUser!=ThisCompany.User) 
            {
                return NotFound();
            }
            return View(ThisCompany);
        }
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> DeleteCompHax(int? id) // 
        {
            if (id == null)
            {
                return NotFound();
            }
            Company NoMoreCompany = await _context.Companies.FindAsync(id);
            if (NoMoreCompany != null)
            {
                _context.Companies.Remove(NoMoreCompany);
                await _context.SaveChangesAsync();
                return View("Deleted");
            }
            return RedirectToAction("IndexComp", "CompanyTb");
        }
        //}
        //[Authorize(Roles = "admin, user")]
        //[HttpGet]
        public async Task<IActionResult> EditAvatar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Company SomeCompany = await _context.Companies.FirstOrDefaultAsync(m => m.Id == id);//
            if (SomeCompany == null)
            {
                return NotFound();
            }
            CompAvatar compAvatar = new CompAvatar
            {
                Id = SomeCompany.Id,// Надо оно тут???
                Org_name = SomeCompany.Org_name,
                Region = SomeCompany.Region,
                INN = SomeCompany.INN,
                KPP = SomeCompany.KPP,
                OGRN = SomeCompany.OGRN,
                Account_number = SomeCompany.Account_number,
                Org_phone = SomeCompany.Org_phone,
                Org_email = SomeCompany.Org_email,
                Info = SomeCompany.Info,
                Surname = SomeCompany.Surname,
                Name = SomeCompany.Name,
                Patronymic = SomeCompany.Patronymic,
                Phone = SomeCompany.Phone,
                Email = SomeCompany.Email,
                Avatar = null
            };
            return View(compAvatar); 
        }

        public async Task<IActionResult> SaveAvatar(CompAvatar compAvatar)
        {
            Company model = new Company
            {
                Id = compAvatar.Id,// Надо оно тут???
                Org_name = compAvatar.Org_name,
                Region = compAvatar.Region,
                INN = compAvatar.INN,
                KPP = compAvatar.KPP,
                OGRN = compAvatar.OGRN,
                Account_number = compAvatar.Account_number,
                Org_phone = compAvatar.Org_phone,
                Org_email = compAvatar.Org_email,
                Info = compAvatar.Info,
                Surname = compAvatar.Surname,
                Name = compAvatar.Name,
                Patronymic = compAvatar.Patronymic,
                Phone = compAvatar.Phone,
                Email = compAvatar.Email,
                Image = null
            };
            if (compAvatar.Avatar != null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(compAvatar.Avatar.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)compAvatar.Avatar.Length);
                }
                model.Image = imageData;
            }
            _context.Attach(model).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Ready", "CompanyTb");
        }
        //------------------------ Метод индентификации User'a . Потом ещё раз подкмать о необходимости этого метода.
        
        public async Task<IActionResult> UserIdentity(string UserName) 
        {
            if (UserName == null) 
            {
                return RedirectToAction("Error", "Home");// 
            }

            User SomeUser = await _context.Users.FirstOrDefaultAsync(p=>p.Email == UserName);// нашли в базе этого User'a 
            //Company UserCompany = _context.Companies.FirstOrDefault(p=>p.User == SomeUser);// нашли в базе компанию User'a
            IQueryable<Company> UserCompanies = _context.Companies.Where(p => p.User == SomeUser);

            return View(UserCompanies);
        }
        // ------------- Вьюха готово! 

        public IActionResult Ready()
        {
            return View();
        }

        //--------------Медот вывода трёх последних зарегистрированных компании на главной
        //public  List<Company> SeachLastFromDB() // переделать в асинх???
        //public IActionResult Index()
        //{
        //    List<Company> SomeCompsList = new List<Company>();

        //    var SomeComps = _context.Companies.Where(Company=>Company.Id ==_context.Companies.Max(Company=>Company.Id)).First();
        //    SomeCompsList.Add(SomeComps);
        //    SomeComps = _context.Companies.Where(Company=>Company.Id == SomeComps.Id-1).First();
        //    SomeCompsList.Add(SomeComps);
        //    SomeComps= _context.Companies.Where(Company => Company.Id == SomeComps.Id - 1).First();
        //    SomeCompsList.Add(SomeComps);

        //    return View(SomeCompsList);
        //}



    }
}