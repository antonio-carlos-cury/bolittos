using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BolittosApi.Data;
using BolittosApi.Models;

namespace BolittosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _db;
    public ClientesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Clientes.ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var c = await _db.Clientes.FindAsync(id);
        if (c == null) return NotFound();
        return Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        if (cliente.Telefone.Length < 11 || cliente.Telefone.Length > 14)
            return BadRequest("Telefone deve conter entre 11 e 14 caracteres.");
        if (cliente.Senha.Length < 6 || cliente.Senha.Length > 20)
            return BadRequest("Senha deve ter entre 6 e 20 caracteres.");

        _db.Clientes.Add(cliente);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Cliente updated)
    {
        if (id != updated.Id) return BadRequest();
        var existing = await _db.Clientes.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Nome = updated.Nome;
        existing.Email = updated.Email;
        existing.Telefone = updated.Telefone;
        existing.Senha = updated.Senha;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Clientes.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Clientes.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
