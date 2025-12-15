// src/Controllers/PacksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using MongoDB.Driver;
using ApiMDb.Models.package;
using ApiMDb.Services;

namespace ApiMDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacksController : ControllerBase
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Package> _packages;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<PacksController> _logger;

    public PacksController(
        IMongoDBService mongoDBService,
        IWebHostEnvironment env,
        ILogger<PacksController> logger )
    {
        _database = mongoDBService.Database;
        _packages = _database.GetCollection<Package>("Package");
        _env = env;
         _logger = logger;
    }

    [HttpPost("create")]
    
    /*****public async Task<IActionResult> CreateProduct(
        [FromForm] string name, 
        [FromForm] decimal price, 
        [FromForm] IFormFile image)**********/
    public async Task<IActionResult> Create([FromForm] PackageModel model)
    {
         _logger.LogInformation("Iam at Package.controller- create - line 41 - model.name: " + model.Nameplan);
        if (model.Image == null || model.Image.Length == 0)
        return BadRequest("No image uploaded");
        var imagePath = "";
        if (model.Image != null)
        _logger.LogInformation("Iam at Package.controller-register - line 46 - model.Image: " +  model.Image);
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder); // Creates folder if it doesn't exist
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
              
            
            var fileName = Path.GetFileName(model.Image.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await model.Image.CopyToAsync(stream);
            imagePath = "/Uploads/" + fileName;
        }

        var package = new Package
        {
              Nameplan =          model.Nameplan,
              Description =       model.Description, 
              Features =          model.Features,
              Code     =          model.Code,
              Status   =          model.Status,
              Trialdays = model.Trialdays,
              Timedays  = model.Timedays,
              Cost      = model.Cost,
              Image = imagePath,
              CreateAt = DateTime.UtcNow
        };

        await _packages.InsertOneAsync(package);
          _logger.LogInformation("Iam at Package.controller- create - line 74 - Package created Successfully: " +  package);
        //return Ok("Product created.");
        // Creates an anonymous object with a property Package whose value is the package variable.
        return Ok(new { Package = package });
    }

    [HttpGet("listAll")]
      public async Task<IActionResult> GetAll()
    {  
        var packs = await _packages.Find(_ => true).ToListAsync();
        _logger.LogInformation("Iam at Package.controller- GetAll - line 86 - Packages: " + packs);
        return Ok(packs);
    }

    [HttpGet("get-single-pack/{id}")]
    
    public async Task<IActionResult> GetById(string id)
    {
         _logger.LogInformation("Iam at Package.controller- GetPack - line 91 - Id: " +  id);
        var package = await _packages.Find(p => p.Id == id).FirstOrDefaultAsync();
        _logger.LogInformation("Iam at Package.controller- GetPackage - line 95 - pack: " +  package);
        return package == null ? NotFound() : Ok(package);
    }

    [HttpPut("update-pack/{id}")]
   
    public async Task<IActionResult> Update(string id, [FromForm] PackageModel model)

    {
         _logger.LogInformation("Iam at PAckage.controller- update/{id} - line 104 - Id: " +  id);
        var package = await _packages.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (package == null) return NotFound();

        string newImagePath = "";
        if (model.Image != null)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads");
            var fileName = Path.GetFileName(model.Image.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await model.Image.CopyToAsync(stream);
            newImagePath = "/Uploads/" + fileName;
        }
              package.Nameplan =    model.Nameplan;
              package.Description = model.Description; 
              package.Features =    model.Features;
              package.Code     =    model.Code;
              package.Status   =    model.Status;
              package.Trialdays =   model.Trialdays;
              package.Timedays  =   model.Timedays;
              package.Cost      =   model.Cost;
              package.Image = newImagePath;

        await _packages.ReplaceOneAsync(p => p.Id == id, package);

        //return Ok("Package updated.");
        return Ok( package );
    }

    [HttpDelete("delete-pack/{id}")]
    
    public async Task<IActionResult> Delete(string id)
    {
         _logger.LogInformation("Iam at Package.controller- delete/{id} - line 135 - Id: " +  id);
        await _packages.DeleteOneAsync(p => p.Id == id);
        return Ok("Package deleted.");
    }

    [HttpPost("DeleteImage")]
     public IActionResult DeleteImage([FromForm]string image)
    {
          _logger.LogInformation("Iam at Package.controller- deleteImage - line 150 - imageName: " + image);
        if ( string.IsNullOrEmpty(image))
            return BadRequest("Image name is required");

        string filename = Path.GetFileName(image); // Extracts "member-3.png"
        var filePath = Path.Combine(_env.WebRootPath, "uploads", image);
        
        try
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                var message = "Image deleted successfully";
                 _logger.LogInformation("Iam at Package.controller- deleteImage - line 161 - message: " + message);
                return Ok(message);
            }
            var messageNotFound = "Image file not found";
            _logger.LogInformation("Iam at Package.controller- deleteImage - line 165 - message: " + messageNotFound);
            return Ok(messageNotFound);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    
    // DateTime currentDateOnly = DateTime.Today;
    // Add days to date-only value
    // DateTime futureDateOnly = currentDateOnly.AddDays(req.Body.Timedays);
    
    }

}


