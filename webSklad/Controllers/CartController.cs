using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Web.Helpers;
using webSklad.Data;
using webSklad.Extensions;
using webSklad.Interfaces;
using webSklad.Models;


namespace webSklad.Controllers
{
    public class CartController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly WebSkladContext _context;


        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICartRegisterRepository _cartRegisterRepository;

        private const string Invoice = "invoice";
        private const string Cart = "cart";
        private const string Salesinvoice = "salesinvoice";
        private const string Score = "score";
        private const string Supplier = "supplier";
        private const string Order = "order";
        private const string Writeoff = "writeoff";
        private const string Inventory = "inventory";

        public CartController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, WebSkladContext context,
            IProductRepository productRepository, IUserRepository userRepository, ICartRepository cartRepository, IInvoiceRepository invoiceRepository, ICartRegisterRepository cartRegisterRepository)
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;


            _productRepository = productRepository;
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _invoiceRepository = invoiceRepository;
            _cartRegisterRepository = cartRegisterRepository;
        }


        // Buy
        public IActionResult Buy(string sku, int invoiceId, string sourcePage, int cartregisterid)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            if (cart == null)
            {
                cart = new List<Product_item>();
                if (sourcePage == Invoice)
                {
                    cart.Add(new Product_item
                    {
                        Product = product,
                        Quantity = 1,
                        IncomingPrice = product.IncomingPrice,
                        Price = product.Price,
                        Sku = product.Sku,
                        BarCode = product.BarCode,
                        BarCodeOwn = product.BarCodeOwn,
                        InvoiceId = invoiceId

                    });
                }
                else
                {
                    cart.Add(new Product_item
                    {
                        Product = product,
                        Quantity = 1,
                        IncomingPrice = product.IncomingPrice,
                        Price = product.Price,
                        Sku = product.Sku,
                        BarCode = product.BarCode,
                        BarCodeOwn = product.BarCodeOwn,
                        CartRegisterId = cartregisterid

                    });
                }

            }
            else
            {
                int index = cart.FindIndex(w => w.Product.Sku == sku);

                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    if (sourcePage == Invoice)
                    {
                        cart.Add(new Product_item
                        {
                            Product = product,
                            Quantity = 1,
                            IncomingPrice = product.IncomingPrice,
                            Price = product.Price,
                            Sku = product.Sku,
                            BarCode = product.BarCode,
                            BarCodeOwn = product.BarCodeOwn,
                            InvoiceId = invoiceId


                        });
                    }
                    else
                    {
                        cart.Add(new Product_item
                        {
                            Product = product,
                            Quantity = 1,
                            IncomingPrice = product.IncomingPrice,
                            Price = product.Price,
                            Sku = product.Sku,
                            BarCode = product.BarCode,
                            BarCodeOwn = product.BarCodeOwn,
                            CartRegisterId = cartregisterid


                        });
                    }

                }

            }
            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            switch (sourcePage)
            {
                case Invoice:
                    return RedirectToAction("InvoiceInfo", "Product");
                case Cart:
                    return RedirectToAction("ShowAllProductCartRegister", "CartRegister");
                case Salesinvoice:
                    return RedirectToAction("SalesInvoiceInfo", "Product");
                case Score:
                    return RedirectToAction("ScoreInfo", "Product");

                case Supplier:
                    return RedirectToAction("ReturnToSupplierInfo", "Product");
                case Order:
                    return RedirectToAction("CreateAnOrderinfo", "Product");
                case Writeoff:
                    return RedirectToAction("WriteOffTheProduct", "CartRegister");
                case Inventory:
                    return RedirectToAction("Inventory", "CartRegister");
            }
            return RedirectToAction("Index", "Home");


        }

        public IActionResult SaveCart(int invoiceId, string sourcePage, int cartregisterid)
        {
            var cartItems = HttpContext.Session.Get<List<Product_item>>("cart");

            int lastInvoiceId = _context.Invoices
               .OrderByDescending(i => i.Id)
               .Select(i => i.Id)
               .FirstOrDefault();

            int lastCartRegisterId = _context.CartRegisters
               .OrderByDescending(i => i.Id)
               .Select(i => i.Id)
               .FirstOrDefault();

            if (lastInvoiceId != 0)
            {
                invoiceId = lastInvoiceId;
            }
            else
            {
                invoiceId = 1;
            }


            if (lastCartRegisterId != 0)
            {
                cartregisterid = lastCartRegisterId;
            }
            else
            {
                cartregisterid = 1;
            }


            if (sourcePage == "invoice")
            {
                if (cartItems != null)
                {
                    int cartId = GenerateUniqueId(); 

                    foreach (var cartItem in cartItems)
                    {
                        AddAmountInDB(cartItem.Product.Id, (int)cartItem.Quantity);


                        var cart = new Cart
                        {
                            CartId = cartId, 
                            Sku = cartItem.Sku,
                            IncomingPrice = cartItem.IncomingPrice,
                            ProductId = cartItem.Product.Id,
                            Quantity = cartItem.Quantity,
                            Subtotal = cartItem.SubTotal,
                            Price = cartItem.Product.Price,
                            BarCode = cartItem.Product.BarCode,
                            BarCodeOwn = cartItem.Product.BarCodeOwn,
                            TypeCart = "invoice",
                            InvoiceId = invoiceId

                        };

                        _context.CartItems.Add(cart);
                    }

                    _context.SaveChanges();
                    HttpContext.Session.Remove("cart");
                }
                return RedirectToAction("InvoiceArchive");
            }

            if (sourcePage == "cart")
            {
                if (cartItems != null)
                {
                    int cartId = GenerateUniqueId(); // Генерація унікального ідентифікатора для замовлення

                    foreach (var cartItem in cartItems)
                    {
                        RemoveAmountfromDB(cartItem.Product.Id, (int)cartItem.Quantity);


                        var cart = new Cart
                        {
                            CartId = cartId, // Присвоєння ідентифікатора замовлення
                            Sku = cartItem.Sku,
                            IncomingPrice = cartItem.IncomingPrice,
                            ProductId = cartItem.Product.Id,
                            Quantity = cartItem.Quantity,
                            SubTotalCart = cartItem.SubTotalCart,
                            Price = cartItem.Product.Price,
                            BarCode = cartItem.Product.BarCode,
                            BarCodeOwn = cartItem.Product.BarCodeOwn,
                            TypeCart = "cart",
                            CartRegisterId = cartregisterid

                        };

                        _context.CartItems.Add(cart);
                    }

                    _context.SaveChanges();
                    HttpContext.Session.Remove("cart");
                }
                if (User.IsInRole("Cashier"))
                {
                    return RedirectToAction("Index", "Account");
                }
                return RedirectToAction("CartArchive");
            }
            if (sourcePage == "salesinvoice")
            {
                if (cartItems != null)
                {
                    int cartId = GenerateUniqueId(); // Генерація унікального ідентифікатора для замовлення

                    foreach (var cartItem in cartItems)
                    {
                        AddAmountInDB(cartItem.Product.Id, (int)cartItem.Quantity);


                        var cart = new Cart
                        {
                            CartId = cartId, // Присвоєння ідентифікатора замовлення
                            Sku = cartItem.Sku,
                            IncomingPrice = cartItem.IncomingPrice,
                            ProductId = cartItem.Product.Id,
                            Quantity = cartItem.Quantity,
                            Subtotal = cartItem.SubTotal,
                            Price = cartItem.Product.Price,
                            BarCode = cartItem.Product.BarCode,
                            BarCodeOwn = cartItem.Product.BarCodeOwn,
                            TypeCart = "salesinvoice",
                            InvoiceId = invoiceId

                        };

                        _context.CartItems.Add(cart);
                    }

                    _context.SaveChanges();
                    HttpContext.Session.Remove("cart");
                }
                return RedirectToAction("SalesInvoiceArchive");
            }
            if (sourcePage == "score")
            {
                if (cartItems != null)
                {
                    int cartId = GenerateUniqueId(); // Генерація унікального ідентифікатора для замовлення

                    foreach (var cartItem in cartItems)
                    {
                        AddAmountInDB(cartItem.Product.Id, (int)cartItem.Quantity);


                        var cart = new Cart
                        {
                            CartId = cartId, // Присвоєння ідентифікатора замовлення
                            Sku = cartItem.Sku,
                            IncomingPrice = cartItem.IncomingPrice,
                            ProductId = cartItem.Product.Id,
                            Quantity = cartItem.Quantity,
                            Subtotal = cartItem.SubTotal,
                            Price = cartItem.Product.Price,
                            BarCode = cartItem.Product.BarCode,
                            BarCodeOwn = cartItem.Product.BarCodeOwn,
                            TypeCart = "score",
                            InvoiceId = invoiceId

                        };

                        _context.CartItems.Add(cart);
                    }

                    _context.SaveChanges();
                    HttpContext.Session.Remove("cart");
                }
                return RedirectToAction("ScoreArchive");
            }
            if (sourcePage == "supplier")
            {
                if (cartItems != null)
                {
                    int cartId = GenerateUniqueId(); // Генерація унікального ідентифікатора для замовлення

                    foreach (var cartItem in cartItems)
                    {
                        AddAmountInDB(cartItem.Product.Id, (int)cartItem.Quantity);


                        var cart = new Cart
                        {
                            CartId = cartId, // Присвоєння ідентифікатора замовлення
                            Sku = cartItem.Sku,
                            IncomingPrice = cartItem.IncomingPrice,
                            ProductId = cartItem.Product.Id,
                            Quantity = cartItem.Quantity,
                            Subtotal = cartItem.SubTotal,
                            Price = cartItem.Product.Price,
                            BarCode = cartItem.Product.BarCode,
                            BarCodeOwn = cartItem.Product.BarCodeOwn,
                            TypeCart = "supplier",
                            InvoiceId = invoiceId

                        };

                        _context.CartItems.Add(cart);
                    }

                    _context.SaveChanges();
                    HttpContext.Session.Remove("cart");
                }
                return RedirectToAction("RemoveToSupplierArchive");
            }
            if (sourcePage == "order")
            {
                if (cartItems != null)
                {
                    int cartId = GenerateUniqueId(); // Генерація унікального ідентифікатора для замовлення

                    foreach (var cartItem in cartItems)
                    {
                        AddAmountInDB(cartItem.Product.Id, (int)cartItem.Quantity);


                        var cart = new Cart
                        {
                            CartId = cartId, // Присвоєння ідентифікатора замовлення
                            Sku = cartItem.Sku,
                            IncomingPrice = cartItem.IncomingPrice,
                            ProductId = cartItem.Product.Id,
                            Quantity = cartItem.Quantity,
                            Subtotal = cartItem.SubTotal,
                            Price = cartItem.Product.Price,
                            BarCode = cartItem.Product.BarCode,
                            BarCodeOwn = cartItem.Product.BarCodeOwn,
                            TypeCart = "order",
                            InvoiceId = invoiceId

                        };

                        _context.CartItems.Add(cart);
                    }

                    _context.SaveChanges();
                    HttpContext.Session.Remove("cart");
                }
                return RedirectToAction("CreateAnOrderArchive");
            }
            if (sourcePage == "writeoff")
            {
                if (cartItems != null)
                {
                    int cartId = GenerateUniqueId(); // Генерація унікального ідентифікатора для замовлення

                    foreach (var cartItem in cartItems)
                    {
                        AddAmountInDB(cartItem.Product.Id, (int)cartItem.Quantity);


                        var cart = new Cart
                        {
                            CartId = cartId, // Присвоєння ідентифікатора замовлення
                            Sku = cartItem.Sku,
                            IncomingPrice = cartItem.IncomingPrice,
                            ProductId = cartItem.Product.Id,
                            Quantity = cartItem.Quantity,
                            Subtotal = cartItem.SubTotal,
                            Price = cartItem.Product.Price,
                            BarCode = cartItem.Product.BarCode,
                            BarCodeOwn = cartItem.Product.BarCodeOwn,
                            TypeCart = "writeoff",
                            InvoiceId = invoiceId

                        };

                        _context.CartItems.Add(cart);
                    }

                    _context.SaveChanges();
                    HttpContext.Session.Remove("cart");
                }
                return RedirectToAction("WrireOffTheProductArchive");
            }
            if (sourcePage == "inventory")
            {
                if (cartItems != null)
                {
                    int cartId = GenerateUniqueId(); // Генерація унікального ідентифікатора для замовлення

                    foreach (var cartItem in cartItems)
                    {
                        AddAmountInDB(cartItem.Product.Id, (int)cartItem.Quantity);


                        var cart = new Cart
                        {
                            CartId = cartId, // Присвоєння ідентифікатора замовлення
                            Sku = cartItem.Sku,
                            IncomingPrice = cartItem.IncomingPrice,
                            ProductId = cartItem.Product.Id,
                            Quantity = cartItem.Quantity,
                            Subtotal = cartItem.SubTotal,
                            Price = cartItem.Product.Price,
                            BarCode = cartItem.Product.BarCode,
                            BarCodeOwn = cartItem.Product.BarCodeOwn,
                            TypeCart = "inventory",
                            InvoiceId = invoiceId,
                            

                        };

                        _context.CartItems.Add(cart);
                    }

                    _context.SaveChanges();
                    HttpContext.Session.Remove("cart");
                }
                return RedirectToAction("InventoryArchive");
            }


            return RedirectToAction("InvoiceArchive");
        }




        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);
            if (cart != null)
            {
                ViewBag.total = cart.Sum(s => s.Quantity * s.Product.IncomingPrice);
            }
            else
            {
                cart = new List<Product_item>();
                ViewBag.total = 0;
            }

            return View(cart);

        }

        public IActionResult IndexCart()
        {

            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);
            if (cart != null)
            {
                ViewBag.total = cart.Sum(s => s.Quantity * s.Product.Price);
            }
            else
            {
                cart = new List<Product_item>();
                ViewBag.total = 0;
            }

            return View(cart);


        }
        public IActionResult IndexWriteOffTheProduct()
        {

            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);
            if (cart != null)
            {
                ViewBag.total = cart.Sum(s => s.Quantity * s.Product.Price);
            }
            else
            {
                cart = new List<Product_item>();
                ViewBag.total = 0;
            }

            return View(cart);


        }
        public IActionResult SalesInvoiceIndex()
        {

            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);
            if (cart != null)
            {
                ViewBag.total = cart.Sum(s => s.Quantity * s.Product.Price);
            }
            else
            {
                cart = new List<Product_item>();
                ViewBag.total = 0;
            }

            return View(cart);


        }
        public IActionResult ScoreIndex()
        {

            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);
            if (cart != null)
            {
                ViewBag.total = cart.Sum(s => s.Quantity * s.Product.Price);
            }
            else
            {
                cart = new List<Product_item>();
                ViewBag.total = 0;
            }

            return View(cart);


        }
        public IActionResult ReturnToSupplierIndex()
        {

            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);
            if (cart != null)
            {
                ViewBag.total = cart.Sum(s => s.Quantity * s.Product.Price);
            }
            else
            {
                cart = new List<Product_item>();
                ViewBag.total = 0;
            }

            return View(cart);


        }

        public IActionResult CreateAnOrderIndex()
        {
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);
            if (cart != null)
            {
                ViewBag.total = cart.Sum(s => s.Quantity * s.Product.Price);
            }
            else
            {
                cart = new List<Product_item>();
                ViewBag.total = 0;
            }

            return View(cart);
        }

        public IActionResult InventoryIndex()
        {
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);
            if (cart != null)
            {
                ViewBag.total = cart.Sum(s => s.Quantity * s.Product.Price);
            }
            else
            {
                cart = new List<Product_item>();
                ViewBag.total = 0;
            }

            return View(cart);
        }



        public async Task<IActionResult> InvoiceArchive()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(currentUser.Id, Invoice);
                return View(cartItems);

            }
            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(createdByUser.Id, Invoice);
                return View(cartItems);
            }


            return NotFound();
        }

        public async Task<IActionResult> SalesInvoiceArchive()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(currentUser.Id, Salesinvoice);
                return View(cartItems);
            }
            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(createdByUser.Id, Salesinvoice);
                return View(cartItems);
            }


            return NotFound();
        }

        public async Task<IActionResult> ScoreArchive()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(currentUser.Id, Score);

                return View(cartItems);
            }
            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(createdByUser.Id, Score);

                return View(cartItems);
            }


            return NotFound();
        }

        public async Task<IActionResult> CartArchive()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartRegisterForUser(currentUser.Id, Cart);

                return View(cartItems);
            }
            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartRegisterForUser(createdByUser.Id, Cart);

                return View(cartItems);
            }


            return NotFound();
        }
        public async Task<IActionResult> RemoveToSupplierArchive()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(currentUser.Id, Supplier);

                return View(cartItems);
            }
            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(createdByUser.Id, Supplier);

                return View(cartItems);
            }


            return NotFound();
        }

        public async Task<IActionResult> WrireOffTheProductArchive()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(currentUser.Id, Writeoff);

                return View(cartItems);
            }
            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(createdByUser.Id, Writeoff);

                return View(cartItems);
            }



            return NotFound();
        }

        public async Task<IActionResult> InventoryArchive()
        {

            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(currentUser.Id, Inventory);

                return View(cartItems);
            }
            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(createdByUser.Id, Inventory);

                return View(cartItems);
            }

            return NotFound();
           
        }

        public async Task<IActionResult> CreateAnOrderArchive()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(currentUser.Id, Order);

                return View(cartItems);
            }
            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsForUser(createdByUser.Id, Order);

                return View(cartItems);
            }


            return NotFound();
        }


        public async Task<IActionResult> IncomeStatement(DateTime? selectedDate)
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUserId = await _userRepository.GetCurrentyUser();
                if (currentUserId == null)
                {
                    return NotFound();
                }

                var cartItems = await _cartRepository.GetCartItemsByUserAndDate(currentUserId.Id, Cart, selectedDate);

                ViewBag.SelectedDate = selectedDate;

                return View(cartItems);
            }
            else if (User.IsInRole("Accountant") || User.IsInRole("Executor"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var cartItems = await _cartRepository.GetCartItemsByUserAndDate(createdByUser.Id, Cart, selectedDate);


                ViewBag.SelectedDate = selectedDate;

                return View(cartItems);
            }

            return NotFound();
        }


        public async Task<IActionResult> AddAmountInDB(int productId, decimal quantity)
        {

            var product = await _productRepository.GetProductById(productId);

            if (product != null)
            {
                if (product.Amount == null)
                {
                    product.Amount = 0;
                }

                product.Amount += quantity;
                await _productRepository.Save();
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> RemoveAmountfromDB(int productId, decimal quantity)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product != null)
            {
                if (product.Amount == null)
                {
                    product.Amount = 0;
                }

                product.Amount -= quantity;
                await _productRepository.Save();
            }

            return RedirectToAction("Index");
        }


        private int GenerateUniqueId()
        {

            var lastCart = _context.CartItems.OrderByDescending(c => c.Id).FirstOrDefault();
            int lastId = lastCart != null ? lastCart.Id : 0;
            int newId = lastId + 1;

            return newId;
        }

        [HttpPost]
        public async Task<IActionResult> SavePaymentStatus(int invoiceId, bool isPayment)
        {

            var invoice = await _invoiceRepository.GetInvoiceById(invoiceId);
            if (invoice != null)
            {
                invoice.IsPayment = isPayment;
                _invoiceRepository.Save();
                return Ok();
            }

            return BadRequest("Invoice not found.");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCart(int cartId)
        {

            var cartItems = await _cartRepository.GetCartItemsByCartId(cartId);


            if (cartItems.Any())
            {
                await _cartRepository.DeleteCartRange(cartItems);
                _cartRepository.SaveChangesAsync();
            }


            return Json(new { success = true });
        }


        public IActionResult Add(string sku, string sourcePage)
        {

            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart[index].Quantity++;

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("Index");
        }

        public IActionResult Minus(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);

            if (cart[index].Quantity == 1)
            {
                cart.RemoveAt(index);
            }
            else
            {
                cart[index].Quantity--;
            }


            HttpContext.Session.Set<List<Product_item>>(Cart, cart);
            return RedirectToAction("Index");
        }


        public IActionResult Remove(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart.RemoveAt(index);

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("Index");
        }

        public IActionResult AddCart(string sku, string sourcePage)
        {

            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart[index].Quantity++;

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("IndexCart");
        }

        public IActionResult MinusCart(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);

            if (cart[index].Quantity == 1)
            {
                cart.RemoveAt(index);
            }
            else
            {
                cart[index].Quantity--;
            }


            HttpContext.Session.Set<List<Product_item>>(Cart, cart);
            return RedirectToAction("IndexCart");
        }


        public IActionResult RemoveCart(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart.RemoveAt(index);

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("IndexCart");
        }

        public IActionResult AddSalesInvoice(string sku, string sourcePage)
        {

            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart[index].Quantity++;

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("SalesInvoiceIndex");
        }

        public IActionResult MinusSalesInvoice(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);

            if (cart[index].Quantity == 1)
            {
                cart.RemoveAt(index);
            }
            else
            {
                cart[index].Quantity--;
            }


            HttpContext.Session.Set<List<Product_item>>(Cart, cart);
            return RedirectToAction("SalesInvoiceIndex");
        }


        public IActionResult RemoveSalesInvoice(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart.RemoveAt(index);

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("SalesInvoiceIndex");
        }

        public IActionResult AddScore(string sku, string sourcePage)
        {

            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart[index].Quantity++;

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("ScoreIndex");
        }

        public IActionResult MinusScore(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);

            if (cart[index].Quantity == 1)
            {
                cart.RemoveAt(index);
            }
            else
            {
                cart[index].Quantity--;
            }


            HttpContext.Session.Set<List<Product_item>>(Cart, cart);
            return RedirectToAction("ScoreIndex");
        }


        public IActionResult RemoveScore(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart.RemoveAt(index);

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("ScoreIndex");
        }

        public IActionResult AddReturnToSupplier(string sku, string sourcePage)
        {

            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart[index].Quantity++;

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("ReturnToSupplierIndex");
        }

        public IActionResult MinusReturnToSupplier(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);

            if (cart[index].Quantity == 1)
            {
                cart.RemoveAt(index);
            }
            else
            {
                cart[index].Quantity--;
            }


            HttpContext.Session.Set<List<Product_item>>(Cart, cart);
            return RedirectToAction("ReturnToSupplierIndex");
        }


        public IActionResult RemoveReturnToSupplier(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart.RemoveAt(index);

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("ReturnToSupplierIndex");
        }


        public IActionResult AddToOrder(string sku, string sourcePage)
        {

            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart[index].Quantity++;

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("CreateAnOrderIndex");
        }

        public IActionResult MinusToOrder(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);

            if (cart[index].Quantity == 1)
            {
                cart.RemoveAt(index);
            }
            else
            {
                cart[index].Quantity--;
            }


            HttpContext.Session.Set<List<Product_item>>(Cart, cart);
            return RedirectToAction("CreateAnOrderIndex");
        }


        public IActionResult RemoveToOrder(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart.RemoveAt(index);

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("CreateAnOrderIndex");
        }


        public IActionResult AddToWriteOff(string sku, string sourcePage)
        {

            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart[index].Quantity++;

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("IndexWriteOffTheProduct");
        }

        public IActionResult MinusToWriteOff(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);

            if (cart[index].Quantity == 1)
            {
                cart.RemoveAt(index);
            }
            else
            {
                cart[index].Quantity--;
            }


            HttpContext.Session.Set<List<Product_item>>(Cart, cart);
            return RedirectToAction("IndexWriteOffTheProduct");
        }


        public IActionResult RemoveToWriteOff(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart.RemoveAt(index);

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("IndexWriteOffTheProduct");
        }

        public IActionResult AddInventory(string sku, string sourcePage)
        {

            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart[index].Quantity++;

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("InventoryIndex");
        }

        public IActionResult MinusInventory(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);

            if (cart[index].Quantity == 1)
            {
                cart.RemoveAt(index);
            }
            else
            {
                cart[index].Quantity--;
            }


            HttpContext.Session.Set<List<Product_item>>(Cart, cart);
            return RedirectToAction("InventoryIndex");
        }


        public IActionResult RemoveInventory(string sku, string sourcePage)
        {
            var product = _productRepository.GetProduct(sku);
            var cart = HttpContext.Session.Get<List<Product_item>>(Cart);

            int index = cart.FindIndex(w => w.Product.Sku == sku);
            cart.RemoveAt(index);

            HttpContext.Session.Set<List<Product_item>>(Cart, cart);

            return RedirectToAction("InventoryIndex");
        }

    }
}