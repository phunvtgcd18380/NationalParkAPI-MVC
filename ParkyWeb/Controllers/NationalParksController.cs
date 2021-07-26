using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepository;
        public NationalParksController(INationalParkRepository npRepository)
        {
            _npRepository = npRepository;
        }
        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }
        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await _npRepository.GetAllAsync(SD.NationalParkAPIPath) });
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();
            if(id == null)
            {
                // this will be for Create
                return View(obj);
            }
            // this flow for Update
            obj = await _npRepository.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault());
            if(obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    byte[] p1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using(var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    obj.Picture = p1;
                }
                else
                {
                    var objFromDb = await _npRepository.GetAsync(SD.NationalParkAPIPath, obj.id);
                    obj.Picture = objFromDb.Picture;
                }
                if(obj.id == 0)
                {
                    await _npRepository.CreateAsync(SD.NationalParkAPIPath, obj);
                }
                else
                {
                    await _npRepository.UpdateAsync(SD.NationalParkAPIPath + obj.id, obj);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _npRepository.DeleteAsync(SD.NationalParkAPIPath, id);
            if (status)
            {
                return Json(new { success = true, message = "Delete Successful" });
            }
            return Json(new { success = false, message = "Something wrong when delete" });
        }

    }
}
