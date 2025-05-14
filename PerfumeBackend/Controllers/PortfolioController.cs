using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfumeBackend.Data;
using PerfumeBackend.Models;

namespace PerfumeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly PerfumeContext _context;

    public PortfolioController(PerfumeContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Portfolio>>> GetPortfolios()
    {
        return await _context.Portfolio.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Portfolio>> AddPortfolio([FromBody] Portfolio portfolio)
    {
        _context.Portfolio.Add(portfolio);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPortfolios), new { id = portfolio.Id }, portfolio);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerfume(int id)
    {
        var perfume = await _context.Portfolio.FindAsync(id);
        if (perfume == null)
        {
            return NotFound();
        }

        _context.Portfolio.Remove(perfume);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}