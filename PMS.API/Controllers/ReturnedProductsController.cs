using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Common.Dto;
//using PMS.Data.Models;
using PMS.LogoService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnedProductsController : ControllerBase
    {
        //private readonly BGTRANSFER_V2Context _context;

        //public ReturnedProductsController(BGTRANSFER_V2Context context)
        //{
        //    _context = context;
        }

        // GET: api/ReturnedProducts
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ReturnedProduct>>> GetReturnedProducts()
        //{
        //    return await _context.ReturnedProducts.ToListAsync();
        //}

        // GET: api/ReturnedProducts/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<ReturnedProduct>> GetReturnedProducts(long id)
        //{
        //    var returnedProducts = await _context.ReturnedProducts.FindAsync(id);

        //    if (returnedProducts == null)
        //    {
        //        return NotFound();
        //    }

        //    return returnedProducts;
        //}

        //// PUT: api/ReturnedProducts/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutReturnedProducts(long id, ReturnedProducts returnedProducts)
        //{
        //    if (id != returnedProducts.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(returnedProducts).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ReturnedProductsExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/ReturnedProducts
        //[HttpPost]
        //public async Task<ActionResult<ReturnedProduct>> PostReturnedProducts(ReturnedProduct returnedProduct)
        //{
        //    _context.ReturnedProducts.Add(returnedProduct);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetReturnedProducts", new { id = returnedProduct.Id }, returnedProduct);
        //}

        //[HttpPost]
        //public async Task<IActionResult> ProcessReturnedProducts(ReturnedItemDto returnedItem)
        //{
        //    try
        //    {
        //        if (returnedItem.Operation == Operation.Cancel)
        //        {
        //            DoCancel(returnedItem);
        //        }
        //        else
        //        {
        //            DoReshipment(returnedItem);
        //        }

        //        LogProcessedProduct(returnedItem);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //private void LogProcessedProduct(ReturnedItemDto returnedItem)
        //{
        //    var returnedProducts = _context.ReturnedProducts.Find(returnedItem.Id);
        //}

        //private void DoReshipment(ReturnedItemDto returnedItem)
        //{


        //    FlagReturnedItem(returnedItem.LogoReferans);

        //}

        //private void DoCancel(ReturnedItemDto returnedItem)
        //{
        //    FlagReturnedItem(returnedItem.LogoReferans);

        //}

        //private static void FlagReturnedItem(string logoReferans)
        //{
        //    ReturnedGoodsService ls = new ReturnedGoodsService();
        //    var res = ls.FlagReturnedItemAsProcessed(Convert.ToInt32(logoReferans));
        //}

        //// DELETE: api/ReturnedProducts/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ReturnedProducts>> DeleteReturnedProducts(long id)
        //{
        //    var returnedProducts = await _context.ReturnedProducts.FindAsync(id);
        //    if (returnedProducts == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.ReturnedProducts.Remove(returnedProducts);
        //    await _context.SaveChangesAsync();

        //    return returnedProducts;
        //}

        //private bool ReturnedProductsExists(long id)
        //{
        //    return _context.ReturnedProducts.Any(e => e.Id == id);
        //}
    
}