using Melotrade.Data;
using Microsoft.AspNetCore.Mvc;
using Melotrade.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Melotrade.Services;

namespace Melotrade.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly YocoPaymentService _yocoPaymentService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            YocoPaymentService yocoPaymentService,
            ILogger<ItemController> logger)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _yocoPaymentService = yocoPaymentService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var items = _context.Items
                 .Where(i => i.active == "Y")
                .ToList();
            return View(items);


        }

        [HttpGet]
        public async Task<IActionResult> Buy(int id)
        {
            var item = await _context.Items
                .Include(i => i.ItemImages)
                .FirstOrDefaultAsync(i => i.Id == id);

            var user = await _userManager.GetUserAsync(User);

            if (item == null) return NotFound();

            ViewBag.User = user;

            return View(item); // Pass item to Buy.cshtml
        }

        public IActionResult Create()
        {
            var model = new ItemCreateViewModel
            {
                Item = new Item(),
                AvailableItems = _context.Items
                .Where(i => i.active == "Y")
                .OrderByDescending(i => i.Id)
                .ToList(), 

                ExistingImages = new List<ItemImage>() 
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Item item)
        {
            //if (ModelState.IsValid)
            item.ItemNr = GenerateItemNr(item.Category, _context);
            {
                item.FormId = Guid.NewGuid().ToString();
                item.active = "Y";

                _context.Items.Add(item);

                //add this
                _context.SaveChanges();

                await _context.SaveChangesAsync();

                return RedirectToAction("UploadImages", new { id = item.Id });
            }

            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> UploadImages(int id)
        {
            var item = await _context.Items
                .Include(i => i.ItemImages)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImages(int id, List<IFormFile> images)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();

            if (images != null && images.Count > 0)
            {
                string uploadPath = Path.Combine("C:\\IMPORTANT\\Melotrade\\wwwroot\\image_uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        string fullPath = Path.Combine(uploadPath, fileName);
                        string relativePath = "/image_uploads/" + fileName;

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var itemImage = new ItemImage
                        {
                            ItemId = item.Id,
                            FormId = item.FormId,
                            ImageUrl = relativePath
                        };

                        _context.ItemImages.Add(itemImage);
                    }
                }

                await _context.SaveChangesAsync();
            }

            // Reload item with images
            var itemWithImages = await _context.Items
                .Include(i => i.ItemImages)
                .FirstOrDefaultAsync(i => i.Id == id);

            return View(itemWithImages);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Items
                .Include(i => i.ItemImages)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            return View(item);
        }





        [HttpPost]
        public IActionResult UpdateInline(int id, string name, string category, decimal price, string description)
        {
            var item = _context.Items.Find(id);
            if (item != null)
            {
                item.Name = name;
                item.Category = category;
                item.Price = price;
                item.Description = description;

                _context.SaveChanges();
            }

            return RedirectToAction("Create"); // Or wherever you want to redirect after save
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int itemId, string fname, string sname, string email, string hospitalid)
        {
            try
            {
                var item = await _context.Items.FindAsync(itemId);
                if (item == null) return NotFound();

                // Generate order number
                var orderNo = $"ORD{Guid.NewGuid().ToString("N")[..8]}";

                // Save payment record
                var payment = new Payment
                {
                    TblName = fname,
                    TblSurname = sname,
                    TblHospitalId = hospitalid,
                    TblItemId = itemId.ToString(),
                    Amount = item.Price.ToString("0.00"),
                    Type = "Support Staff",
                    Active = "N",
                    DDay = DateTime.Now.Day,
                    MMonth = DateTime.Now.Month,
                    YYear = DateTime.Now.Year,
                    TblTime = DateTime.Now.ToString("HH:mm:ss"),
                    OrderNo = orderNo
                };

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                // Get redirect URL
                var baseUrl = _configuration["Yoco:RedirectBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
                var redirectUrl = $"{baseUrl}/Item/PaymentResult?orderNo={orderNo}";

                // Process payment with Yoco
                var paymentUrl = await _yocoPaymentService.CreateCheckoutAsync(
                    amount: item.Price,
                    currency: "ZAR",
                    customerName: $"{fname} {sname}",
                    description: item.Name,
                    redirectUrl: redirectUrl,
                    email: email,
                    reference: orderNo
                );

                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                TempData["ErrorMessage"] = "An error occurred while processing your payment. Please try again.";
                return RedirectToAction("Buy", new { id = itemId });
            }
        }


        [HttpGet]
        public async Task<IActionResult> PaymentResult(string orderNo)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderNo == orderNo);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }


        // For backward compatibility
        public async Task<IActionResult> PaymentResult(int id)
        {
            var payment = await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.id == id);

            return payment == null ? NotFound() : View(payment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            item.active = "N";
            await _context.SaveChangesAsync();

            return RedirectToAction("Create");
        }








        public string GenerateItemNr(string category, ApplicationDbContext dbContext)
        {
            // First 3 letters of category, uppercase
            string prefix = category.Length >= 3 ? category.Substring(0, 3).ToUpper() : category.ToUpper();

            // Find items with that prefix
            var existingNumbers = dbContext.Items
                .Where(i => i.ItemNr.StartsWith(prefix))
                .Select(i => i.ItemNr)
                .ToList();

            int maxNumber = 0;
            foreach (var itemNr in existingNumbers)
            {
                // Extract the numeric part
                var numberPart = itemNr.Substring(3);
                if (int.TryParse(numberPart, out int number))
                {
                    if (number > maxNumber)
                        maxNumber = number;
                }
            }

            // New number is max + 1
            int newNumber = maxNumber + 1;

            // Determine the number of digits (min 3, can expand)
            string newNumberStr = newNumber.ToString().PadLeft(3, '0');

            // Combine
            string newItemNr = prefix + newNumberStr;

            return newItemNr;
        }













    }
}
