using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BolittosApi.Data;
using BolittosApi.Models;

namespace BolittosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProdutosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Produtos.ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var p = await _db.Produtos.FindAsync(id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Produto produto)
    {
        if (produto.PrecoCusto >= produto.PrecoVenda)
            return BadRequest("PrecoCusto deve ser menor que PrecoVenda.");

        _db.Produtos.Add(produto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Produto updated)
    {
        if (id != updated.Id) return BadRequest();
        var existing = await _db.Produtos.FindAsync(id);
        if (existing == null) return NotFound();

        if (updated.PrecoCusto >= updated.PrecoVenda)
            return BadRequest("PrecoCusto deve ser menor que PrecoVenda.");

        existing.Nome = updated.Nome;
        existing.Descricao = updated.Descricao;
        existing.PrecoCusto = updated.PrecoCusto;
        existing.PrecoVenda = updated.PrecoVenda;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Produtos.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Produtos.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
