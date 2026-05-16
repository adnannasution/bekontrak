using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefineryContractAPI.Data;
using RefineryContractAPI.DTOs;
using RefineryContractAPI.Models;

namespace RefineryContractAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagihanController : ControllerBase
{
    private readonly AppDbContext _context;
    public TagihanController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.Tagihans
            .Include(t => t.Kontrak).ThenInclude(k => k!.Vendor)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TagihanDto
            {
                IdTagihan = t.IdTagihan, IdKontrak = t.IdKontrak,
                NomorTagihan = t.NomorTagihan, TanggalTagihan = t.TanggalTagihan,
                TipeKontrak = t.TipeKontrak, Termin = t.Termin,
                NilaiTagihan = t.NilaiTagihan, StatusTagihan = t.StatusTagihan,
                MemoRequired = t.MemoRequired, TanggalPengirimanMemo = t.TanggalPengirimanMemo,
                DokumenTagihan = t.DokumenTagihan, DokumenMemo = t.DokumenMemo,
                Catatan = t.Catatan, CreatedAt = t.CreatedAt, UpdatedAt = t.UpdatedAt,
                Kontrak = t.Kontrak == null ? null : new KontrakSummaryDto
                {
                    IdKontrak = t.Kontrak.IdKontrak, JudulKontrak = t.Kontrak.JudulKontrak,
                    TipeKontrak = t.Kontrak.TipeKontrak,
                    Vendor = t.Kontrak.Vendor == null ? null : new VendorDto
                    { IdVendor = t.Kontrak.Vendor.IdVendor, NamaVendor = t.Kontrak.Vendor.NamaVendor }
                }
            }).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var t = await _context.Tagihans
            .Include(t => t.Kontrak).ThenInclude(k => k!.Vendor)
            .FirstOrDefaultAsync(t => t.IdTagihan == id);
        if (t == null) return NotFound();
        return Ok(new TagihanDto
        {
            IdTagihan = t.IdTagihan, IdKontrak = t.IdKontrak,
            NomorTagihan = t.NomorTagihan, TanggalTagihan = t.TanggalTagihan,
            TipeKontrak = t.TipeKontrak, Termin = t.Termin,
            NilaiTagihan = t.NilaiTagihan, StatusTagihan = t.StatusTagihan,
            MemoRequired = t.MemoRequired, TanggalPengirimanMemo = t.TanggalPengirimanMemo,
            DokumenTagihan = t.DokumenTagihan, DokumenMemo = t.DokumenMemo,
            Catatan = t.Catatan, CreatedAt = t.CreatedAt, UpdatedAt = t.UpdatedAt,
            Kontrak = t.Kontrak == null ? null : new KontrakSummaryDto
            {
                IdKontrak = t.Kontrak.IdKontrak, JudulKontrak = t.Kontrak.JudulKontrak,
                TipeKontrak = t.Kontrak.TipeKontrak,
                Vendor = t.Kontrak.Vendor == null ? null : new VendorDto
                {
                    IdVendor = t.Kontrak.Vendor.IdVendor, NamaVendor = t.Kontrak.Vendor.NamaVendor,
                    Alamat = t.Kontrak.Vendor.Alamat, PicNama = t.Kontrak.Vendor.PicNama,
                    PicKontak = t.Kontrak.Vendor.PicKontak
                }
            }
        });
    }

    [HttpGet("kontrak/{idKontrak}")]
    public async Task<IActionResult> GetByKontrak(string idKontrak)
    {
        var list = await _context.Tagihans
            .Where(t => t.IdKontrak == idKontrak).OrderBy(t => t.CreatedAt)
            .Select(t => new TagihanDto
            {
                IdTagihan = t.IdTagihan, IdKontrak = t.IdKontrak,
                NomorTagihan = t.NomorTagihan, TanggalTagihan = t.TanggalTagihan,
                TipeKontrak = t.TipeKontrak, Termin = t.Termin,
                NilaiTagihan = t.NilaiTagihan, StatusTagihan = t.StatusTagihan,
                MemoRequired = t.MemoRequired, TanggalPengirimanMemo = t.TanggalPengirimanMemo,
                DokumenTagihan = t.DokumenTagihan, DokumenMemo = t.DokumenMemo,
                Catatan = t.Catatan, CreatedAt = t.CreatedAt, UpdatedAt = t.UpdatedAt,
            }).ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagihanDto dto)
    {
        var tagihan = new Tagihan
        {
            IdKontrak = dto.IdKontrak, NomorTagihan = dto.NomorTagihan,
            TanggalTagihan = dto.TanggalTagihan, TipeKontrak = dto.TipeKontrak,
            Termin = dto.Termin, NilaiTagihan = dto.NilaiTagihan,
            StatusTagihan = dto.StatusTagihan, MemoRequired = dto.MemoRequired,
            TanggalPengirimanMemo = dto.TanggalPengirimanMemo,
            DokumenTagihan = dto.DokumenTagihan, DokumenMemo = dto.DokumenMemo,
            Catatan = dto.Catatan
        };
        _context.Tagihans.Add(tagihan);
        await _context.SaveChangesAsync();
        return Ok(tagihan);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTagihanDto dto)
    {
        var tagihan = await _context.Tagihans.FindAsync(id);
        if (tagihan == null) return NotFound();
        tagihan.NomorTagihan = dto.NomorTagihan;
        tagihan.TanggalTagihan = dto.TanggalTagihan;
        tagihan.TipeKontrak = dto.TipeKontrak;
        tagihan.Termin = dto.Termin;
        tagihan.NilaiTagihan = dto.NilaiTagihan;
        tagihan.StatusTagihan = dto.StatusTagihan;
        tagihan.MemoRequired = dto.MemoRequired;
        tagihan.TanggalPengirimanMemo = dto.TanggalPengirimanMemo;
        tagihan.DokumenTagihan = dto.DokumenTagihan;
        tagihan.DokumenMemo = dto.DokumenMemo;
        tagihan.Catatan = dto.Catatan;
        tagihan.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(tagihan);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var tagihan = await _context.Tagihans.FindAsync(id);
        if (tagihan == null) return NotFound();
        _context.Tagihans.Remove(tagihan);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Tagihan berhasil dihapus" });
    }
}