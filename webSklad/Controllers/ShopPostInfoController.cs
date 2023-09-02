using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models;
using webSklad.Models.ViewModels;

namespace webSklad.Controllers
{
    public class ShopPostInfoController : Controller
    {
        private readonly ISRRepository _srRepository;
        private readonly IFopRepository _fopRepository;
        private readonly IUserRepository _userRepository;
        private readonly IShopPostInfoRepository _shopPostInfoRepository;


        private static string ShopInfo = "shopinfo";
        private static string PostInfo = "postinfo";

        public ShopPostInfoController(ISRRepository srRepositroy
        , IFopRepository fopRepository, IUserRepository userRepository, IShopPostInfoRepository shopPostInfoRepository)
        {

            _srRepository = srRepositroy;
            _fopRepository = fopRepository;
            _userRepository = userRepository;
            _shopPostInfoRepository = shopPostInfoRepository;
        }


        [HttpGet]
        public IActionResult CreateShopPostInfo(string type)
        {
            var model = new CreateShopPostInfoViewModel();

            if (type == ShopInfo)
            {
                model.Type = ShopInfo;
            }
            else if (type == PostInfo)
            {
                model.Type = PostInfo;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShopPostInfo(CreateShopPostInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userRepository.GetCurrentyUser();

            if (currentUser == null)
            {
                return NotFound();
            }

            var shopPostInfo = new ShopPostInfo
            {
                Name = model.Name,
                UserId = currentUser.Id,
                User = currentUser,
                Type = model.Type == ShopInfo ? ShopInfo : PostInfo
            };


            await _shopPostInfoRepository.CreateShopPostInfo(shopPostInfo);


            await _shopPostInfoRepository.GetShopPostInfoById(shopPostInfo.Id);

            return RedirectToAction("CreateFOP", "ShopPostInfo", new { id = shopPostInfo.Id });

        }

        [HttpGet]
        public IActionResult CreateFOP(int id)
        {

            var model = new CreateFOPViewModel
            {
                ShopPostInfoId = id
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFOP(CreateFOPViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var shopPostInfo = await _fopRepository.GetShopPostInfoWithFOPByIdAsync(model);


            if (shopPostInfo == null)
            {
                return NotFound();
            }

            foreach (var fopData in model.Fops)
            {
                var fop = new Fop
                {
                    NameOfFop = fopData.FullOfFop,
                    PhoneNumberOfFop = fopData.PhoneNumberOfFop,
                    FopIPN = fopData.FopIPN,
                    FopMFO = fopData.FopMFO,
                    FopCODE = fopData.FopCODE,
                    Account = fopData.Account,
                    Address = fopData.Adress,
                    ShopPostInfoId = model.ShopPostInfoId
                };

                shopPostInfo.FOPS.Add(fop);
            }


            await _shopPostInfoRepository.SaveAsync();

            var currentShopPostInfo = await _shopPostInfoRepository.GetShopPostInfoById(shopPostInfo.Id);


            if (currentShopPostInfo == null)
            {
                return NotFound();
            }


            if (shopPostInfo.Type == PostInfo)
            {
                return RedirectToAction("CreateSR", "ShopPostInfo", new { id = shopPostInfo.Id });
            }
            else
            {
                return RedirectToAction("Index", "Settings");
            }



        }

        [HttpGet]
        public IActionResult AddFop(int id)
        {

            var model = new CreateFOPViewModel
            {
                ShopPostInfoId = id
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddFop(CreateFOPViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var shopPostInfo = await _fopRepository.GetShopPostInfoWithFOPByIdAsync(model);


            if (shopPostInfo == null)
            {
                return NotFound();
            }

            foreach (var fopData in model.Fops)
            {
                var fop = new Fop
                {
                    NameOfFop = fopData.FullOfFop,
                    PhoneNumberOfFop = fopData.PhoneNumberOfFop,
                    FopIPN = fopData.FopIPN,
                    FopMFO = fopData.FopMFO,
                    FopCODE = fopData.FopCODE,
                    Account = fopData.Account,
                    Address = fopData.Adress,
                    ShopPostInfoId = model.ShopPostInfoId
                };

                shopPostInfo.FOPS.Add(fop);
            }


            await _shopPostInfoRepository.SaveAsync();

            var currentShopPostInfo = await _shopPostInfoRepository.GetShopPostInfoById(shopPostInfo.Id);


            if (currentShopPostInfo == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Settings");

            



        }

        [HttpGet]
        public IActionResult CreateSR(int id)
        {


            var model = new CreateSRViewModel
            {
                ShopPostInfoId = id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSR(CreateSRViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var shopPostInfo = await _srRepository.GetShopPostInfoWithSRByIdAsync(model);

            if (shopPostInfo == null)
            {
                return NotFound();
            }

            foreach (var srData in model.SalesRepresentatives)
            {
                var sr = new SalesRepresentative
                {
                    SRName = srData.SRName,
                    SRSurname = srData.SRSurname,
                    SREmail = srData.SREmail,
                    SRPhoneNumber = srData.SRPhoneNumber,
                    ShopPostInfoId = model.ShopPostInfoId
                };

                shopPostInfo.SalesRepresentatives.Add(sr);
            }

            await _shopPostInfoRepository.SaveAsync();


            return RedirectToAction("Index", "Settings");

        }



        [HttpPost]
        public async Task<IActionResult> DeleteFOP(int fopId)
        {
            var deleted = await _fopRepository.DeleteFop(fopId);

            if (deleted)
            {
                return RedirectToAction("Index", "Settings");
            }
            else
            {
                return RedirectToAction("ErrorActionName", "ErrorController");
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteSR(int srId)
        {
            var deleted = await _srRepository.DeleteSR(srId);

            if (deleted)
            {
                return RedirectToAction("Index", "Settings");
            }
            else
            {
                return RedirectToAction("ErrorActionName", "ErrorController");
            }

        }


        [HttpPost]
        public async Task<IActionResult> DeleteShopPostInfo(int id)
        {
            var deleted = await _shopPostInfoRepository.DeleteShopPostInfo(id);

            if (deleted)
            {
                return RedirectToAction("Index", "Settings");
            }
            else
            {
                return RedirectToAction("ErrorActionName", "ErrorController");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditFOP(int id)
        {
            var fop = await _fopRepository.GetFopById(id);

            if (fop == null)
            {
                return NotFound();
            }

            var model = new EditFopViewModel
            {
                FOPName = fop.NameOfFop,
                FopCODE = fop.FopCODE,
                FOPAccount = fop.Account,
                FopIPN = fop.FopIPN,
                FopMFO = fop.FopMFO,
                FOPPhoneNumber = fop.PhoneNumberOfFop
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditFOP(int id, EditFopViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var fop = await _fopRepository.GetFopById(id);
            if (fop == null)
            {
                return NotFound();
            }

            fop.NameOfFop = model.FOPName;
            fop.FopCODE = model.FopCODE;
            fop.Account = model.FOPAccount;
            fop.PhoneNumberOfFop = model.FOPPhoneNumber;
            fop.FopIPN = model.FopIPN;
            fop.FopMFO = model.FopMFO;

            await _fopRepository.UpdateFop(fop);

            return RedirectToAction("Index", "Settings");
        }
       
        

        [HttpGet]
        public async Task<IActionResult> EditSR(int id)
        {
           

            var varSR = await _srRepository.GetSRById(id);

            if (varSR == null)
            {
                return NotFound();
            }

            var model = new EDITSRViewModel
            {
                SRName = varSR.SRName,
                SRSurname = varSR.SRSurname,
                SREmail = varSR.SREmail,
                SRPhoneNumber = varSR.SRPhoneNumber

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditSR(int id, EDITSRViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var varSR = await _srRepository.GetSRById(id);

            if (varSR == null)
            {
                return NotFound();
            }

            varSR.SRName = model.SRName;
            varSR.SRSurname = model.SRSurname;
            varSR.SRPhoneNumber = model.SRPhoneNumber;
            varSR.SREmail = model.SREmail;



            await _srRepository.UpdateSR(varSR);

            return RedirectToAction("Index", "Settings");
        }


        [HttpGet]
        public async Task<IActionResult> EditShopPostInfo(int id)
        {
          

            var shopPostInfo = await _shopPostInfoRepository.GetShopPostInfoById(id);
            if (shopPostInfo == null)
            {
                return NotFound();
            }

            var model = new EditShopPostInfoViewModel
            {
                Name = shopPostInfo.Name,

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditShopPostInfo(int id, EditShopPostInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var shopPostInfo = await _shopPostInfoRepository.GetShopPostInfoById(id);

            if (shopPostInfo == null)
            {
                return NotFound();
            }

            shopPostInfo.Name = model.Name;


           await _shopPostInfoRepository.UpdateShopPostInfo(shopPostInfo);

            return RedirectToAction("Index", "Settings");
        }


    }
}


