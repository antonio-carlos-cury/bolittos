using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendasApi.Data;
using VendasApi.Models;

namespace BolittosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosProdutosController : ControllerBase
{
    private readonly AppDbContext _db;
    public PedidosProdutosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.PedidosProdutos.Include(pp => pp.Produto).Include(pp => pp.Pedido).ToListAsync());

    [HttpGet("{pedidoId:int}/{produtoId:int}")]
    public async Task<IActionResult> Get(int pedidoId, int produtoId)
    {
        var pp = await _db.PedidosProdutos.Include(x => x.Produto).Include(x => x.Pedido)
            .FirstOrDefaultAsync(x => x.PedidoId == pedidoId && x.ProdutoId == produtoId);
        if (pp == null) return NotFound();
        return Ok(pp);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PedidoProduto pp)
    {
        var pedido = await _db.Pedidos.FindAsync(pp.PedidoId);
        if (pedido == null) return BadRequest("Pedido não encontrado.");
        var produto = await _db.Produtos.FindAsync(pp.ProdutoId);
        if (produto == null) return BadRequest("Produto não encontrado.");
        if (pp.Quantidade < 1) return BadRequest("Quantidade deve ser >= 1.");

        var exists = await _db.PedidosProdutos.FindAsync(pp.PedidoId, pp.ProdutoId);
        if (exists != null) return Conflict("Este produto já está no pedido (chave composta).");

        pp.ValorUnitario = produto.PrecoVenda;
        pp.ValorTotal = pp.ValorUnitario * pp.Quantidade;

        // atualizar valor total do pedido
        pedido.ValorTotal += pp.ValorTotal;

        _db.PedidosProdutos.Add(pp);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { pedidoId = pp.PedidoId, produtoId = pp.ProdutoId }, pp);
    }

    [HttpPut("{pedidoId:int}/{produtoId:int}")]
    public async Task<IActionResult> Update(int pedidoId, int produtoId, PedidoProduto updated)
    {
        if (pedidoId != updated.PedidoId || produtoId != updated.ProdutoId) return BadRequest();
        var existing = await _db.PedidosProdutos.FindAsync(pedidoId, produtoId);
        if (existing == null) return NotFound();

        if (updated.Quantidade < 1) return BadRequest("Quantidade deve ser >= 1.");

        var produto = await _db.Produtos.FindAsync(produtoId);
        if (produto == null) return BadRequest("Produto não encontrado.");

        // ajustar total do pedido
        var pedido = await _db.Pedidos.FindAsync(pedidoId);
        if (pedido == null) return BadRequest("Pedido não encontrado.");

        // subtrai antigo e soma novo
        pedido.ValorTotal -= existing.ValorTotal;

        existing.Quantidade = updated.Quantidade;
        existing.ValorUnitario = produto.PrecoVenda;
        existing.ValorTotal = existing.ValorUnitario * existing.Quantidade;

        pedido.ValorTotal += existing.ValorTotal;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{pedidoId:int}/{produtoId:int}")]
    public async Task<IActionResult> Delete(int pedidoId, int produtoId)
    {
        var existing = await _db.PedidosProdutos.FindAsync(pedidoId, produtoId);
        if (existing == null) return NotFound();

        var pedido = await _db.Pedidos.FindAsync(pedidoId);
        if (pedido != null)
        {
            pedido.ValorTotal -= existing.ValorTotal;
        }

        _db.PedidosProdutos.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
