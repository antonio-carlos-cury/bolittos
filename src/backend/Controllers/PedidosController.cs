using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendasApi.Data;
using VendasApi.Models;

namespace BolittosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _db;
    public PedidosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Pedidos.Include(p => p.Itens).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var p = await _db.Pedidos.Include(p => p.Itens).ThenInclude(i => i.Produto).FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Pedido pedido)
    {
        // validar cliente
        var cliente = await _db.Clientes.FindAsync(pedido.ClienteId);
        if (cliente == null) return BadRequest("Cliente não encontrado.");

        // calcular valor total baseado nos itens se fornecidos
        if (pedido.Itens == null || !pedido.Itens.Any())
            return BadRequest("Pedido precisa conter ao menos um item.");

        decimal total = 0m;
        foreach (var it in pedido.Itens)
        {
            var produto = await _db.Produtos.FindAsync(it.ProdutoId);
            if (produto == null) return BadRequest($"Produto {it.ProdutoId} não encontrado.");
            if (it.Quantidade < 1) return BadRequest("Quantidade deve ser >= 1.");

            it.ValorUnitario = produto.PrecoVenda;
            it.ValorTotal = it.ValorUnitario * it.Quantidade;
            total += it.ValorTotal;
        }

        pedido.ValorTotal = total;
        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = pedido.Id }, pedido);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Pedido updated)
    {
        if (id != updated.Id) return BadRequest();
        var existing = await _db.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id);
        if (existing == null) return NotFound();

        // Update basic fields
        existing.ClienteId = updated.ClienteId;

        // For simplicity: update items by removing existing and adding updated ones
        _db.PedidosProdutos.RemoveRange(existing.Itens);
        existing.Itens = updated.Itens;

        // Recalculate totals
        decimal total = 0m;
        foreach (var it in existing.Itens!)
        {
            var produto = await _db.Produtos.FindAsync(it.ProdutoId);
            if (produto == null) return BadRequest($"Produto {it.ProdutoId} não encontrado.");
            if (it.Quantidade < 1) return BadRequest("Quantidade deve ser >= 1.");

            it.ValorUnitario = produto.PrecoVenda;
            it.ValorTotal = it.ValorUnitario * it.Quantidade;
            total += it.ValorTotal;
        }

        existing.ValorTotal = total;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id);
        if (existing == null) return NotFound();
        _db.Pedidos.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
