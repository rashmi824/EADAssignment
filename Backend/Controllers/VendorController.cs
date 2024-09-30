using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class VendorsController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorsController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateVendor([FromBody] VendorDto dto)
    {
        var vendor = await _vendorService.CreateVendor(dto.Name);
        return Ok(vendor);
    }

    [HttpPost("{vendorId}/comment")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> AddComment(string vendorId, [FromBody] CommentDto dto)
    {
        await _vendorService.AddComment(vendorId, User.FindFirst(ClaimTypes.NameIdentifier).Value, dto.CommentText, dto.Rating);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetVendors()
    {
        var vendors = await _vendorService.GetVendors();
        return Ok(vendors);
    }
}
