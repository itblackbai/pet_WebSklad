using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using webSklad.Data;
using webSklad.Models;
using webSklad.Models.ViewModels;
using static System.Net.WebRequestMethods;

namespace webSklad.Controllers
{
    public class ProductController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly WebSkladContext _context;

        private const string ShopInfoType = "shopinfo";
        private const string PostInfoType = "postinfo";

        public ProductController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, WebSkladContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;


        }
        [HttpPost]
        public IActionResult LoadShopFOPs(int shopInfoId)
        {
            var fops = _context.FOPS.Where(f => f.ShopPostInfoId == shopInfoId).ToList();
            return Json(new SelectList(fops, "Id", "NameOfFop"));
        }

        [HttpPost]
        public IActionResult LoadFOPs(int shopPostInfoId)
        {
            var fops = _context.FOPS.Where(f => f.ShopPostInfoId == shopPostInfoId).ToList();
            return Json(new SelectList(fops, "Id", "NameOfFop"));
        }

        [HttpPost]
        public IActionResult LoadSRsForPost(int srId)
        {
            var fops = _context.SalesRepresentatives.Where(f => f.ShopPostInfoId == srId).ToList();
            return Json(new SelectList(fops, "Id", "SRName"));
        }

        [HttpGet]
        public async Task<IActionResult> CreateInvoice()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUserId = _userManager.GetUserId(User);

                var postInfos = _context.ShopPostInfos
                    .Where(s => s.UserId == currentUserId && s.Type == PostInfoType)
                    .ToList();

                var shopInfos = _context.ShopPostInfos
                    .Where(s => s.UserId == currentUserId && s.Type == ShopInfoType)
                    .ToList();

                var viewModel = new InvoiceViewModel
                {
                    PostInfos = new SelectList(postInfos, "Id", "Name"),
                    PostFops = new SelectList(new List<Fop>(), "Id", "NameOfFop"),
                    PostSR = new SelectList(new List<SalesRepresentative>(), "Id", "SRName"),

                    ShopInfos = new SelectList(shopInfos, "Id", "Name"),
                    ShopFops = new SelectList(new List<Fop>(), "Id", "NameOfFop"),
                };

                return View(viewModel);
            }
            else if (User.IsInRole("Accountant") || User.IsInRole("Executor"))
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var createdByUserId = currentUser.CreatedByUserId;

                var createdByUser = await _userManager.FindByIdAsync(createdByUserId);
                if (createdByUser == null)
                {
                    return NotFound();
                }

                var postInfos = _context.ShopPostInfos
                    .Where(s => s.UserId == createdByUserId && s.Type == PostInfoType)
                    .ToList();

                var shopInfos = _context.ShopPostInfos
                    .Where(s => s.UserId == createdByUserId && s.Type == ShopInfoType)
                    .ToList();

                var viewModel = new InvoiceViewModel
                {
                    PostInfos = new SelectList(postInfos, "Id", "Name"),
                    PostFops = new SelectList(new List<Fop>(), "Id", "NameOfFop"),
                    PostSR = new SelectList(new List<SalesRepresentative>(), "Id", "SRName"),

                    ShopInfos = new SelectList(postInfos, "Id", "Name"),
                    ShopFops = new SelectList(new List<Fop>(), "Id", "NameOfFop"),
                };

                return View(viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvoice(InvoiceViewModel viewModel)
        {

            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return NotFound();
                }

                var invoice = new Invoice
                {
                    Name = viewModel.Name,
                    DateTime = viewModel.DateTime,
                    DateTimeNow = DateTime.Now,
                    DatePostponement = viewModel.DatePostponement,
                    PostInfoId = viewModel.PostInfoId,
                    PostFopId = viewModel.PostFopId,
                    PostSRId = viewModel.PostSRId,
                    ShopInfoId = viewModel.ShopInfoId,
                    ShopFopId = viewModel.ShopFopId,
                    UserInvoiceId = currentUser.Id
                    
                    

                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction("InvoiceInfo");
            }

            else if (User.IsInRole("Accountant") && (User.IsInRole("Executor")))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return NotFound();
                }

                var createdByUserId = currentUser.CreatedByUserId;

                var createdByUser = await _userManager.FindByIdAsync(createdByUserId);
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var invoice = new Invoice
                {
                    Name = viewModel.Name,
                    DateTime = viewModel.DateTime,
                    DateTimeNow = DateTime.Now,
                    DatePostponement = viewModel.DatePostponement,
                    PostInfoId = viewModel.PostInfoId,
                    PostFopId = viewModel.PostFopId,
                    PostSRId = viewModel.PostSRId,
                    ShopInfoId = viewModel.ShopInfoId,
                    ShopFopId = viewModel.ShopFopId,
                    UserInvoiceId = currentUser.Id

                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction("InvoiceInfo");
            }
            return NotFound();
        }
    }
}