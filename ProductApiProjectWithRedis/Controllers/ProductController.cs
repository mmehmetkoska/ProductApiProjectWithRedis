using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApiProjectWithRedis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        RedisContext _manager = new RedisContext();
        private const string ERROR_MESSAGE = "İlgili ID bulunamadı ID: {0}";

        [HttpGet]
        public IActionResult GetAll()
        {
            //redis'ten gelen veriyi Product listesi olarak alındı,başarılı kodu ile döndürüldü
            var product = _manager.Get<List<Product>>("products");
            if (product != null)
            {
                product = product.OrderBy(x => x.Id).ToList();
            }
            return StatusCode(200, product);

        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            //id ye göre Product nesnesi döndürüldü
            var product = _manager.Get<List<Product>>("products").Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
                return StatusCode(403, new ErrorModel { ErrorMessage = String.Format(ERROR_MESSAGE, id) });


            return StatusCode(200, product);


        }

        [HttpPost]
        public ActionResult Insert(Product product)
        {

            //id identity olarak kullandım.Sku unique bir değer olarak aldım
            var vProductList = _manager.Get<List<Product>>("products");

            if (vProductList != null)
            {
                var LastId = vProductList.Max(x => x.Id);
                product.Id = LastId + 1;
            }
            else
            {
                product.Id = 1;
            }

            //eğer liste boş ise hata vermemesi için kontrol gerçekleştirdim.
            if (vProductList != null)
            {
                var vProductSkuControl = vProductList.Where(c => c.Sku == product.Sku).FirstOrDefault();
                if (vProductSkuControl == null)
                {
                    var isCreated = _manager.Set("products", product);
                    if (!isCreated)
                        return StatusCode(500);
                }
                else
                {
                    return StatusCode(406, new ErrorModel { ErrorMessage = String.Format("{0} sku kodu farklı bir ürüne aittir. ", product.Sku) });
                }
            }
            else
            {
                var isCreated = _manager.Set("products", product);
                if (!isCreated)
                    return StatusCode(500);
            }


            return StatusCode(201);

        }


        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Product product)
        {

            //update işleminde girilen id'nin olup olmadığını kontrol ettim
            //eğer var ise yeni girilen bilgilerin sku değerini kontrol ettim unique olarak kabul ettiğim için
            //kontrollere göre status dönderdim
            var vProductList = _manager.Get<List<Product>>("products");

            if (vProductList != null)
            {
                var vProduct = vProductList.Where(c => c.Id == id).FirstOrDefault();

                if (product != null)
                {

                    var vProductSkuControl = _manager.Get<List<Product>>("products").Where(c => c.Id != id && c.Sku == product.Sku).FirstOrDefault();

                    if (vProductSkuControl == null)
                    {
                        var isRemoved = _manager.RemoveValue("products", vProduct);
                        if (isRemoved)
                        {
                            product.Id = id;
                            var isUpdated = _manager.Set("products", product);
                            if (isUpdated)
                            {
                                return StatusCode(204);
                            }
                            else
                            {
                                return StatusCode(403, new ErrorModel { ErrorMessage = String.Format(ERROR_MESSAGE, id) });
                            }
                        }
                        else
                        {
                            return StatusCode(403, new ErrorModel { ErrorMessage = String.Format(ERROR_MESSAGE, id) });
                        }
                    }
                    else
                    {
                        return StatusCode(406, new ErrorModel { ErrorMessage = String.Format("{0} sku kodu farklı bir ürüne aittir. ", product.Sku) });
                    }



                }
                else
                {
                    return StatusCode(403, new ErrorModel { ErrorMessage = String.Format(ERROR_MESSAGE, id) });
                }
            }

            else
            {
                return StatusCode(403, new ErrorModel { ErrorMessage = String.Format(ERROR_MESSAGE, id) });
            }


        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            //delete işleminde listede varsa silme işlemi gerçekleştirdim
            var vProductList = _manager.Get<List<Product>>("products");

            if (vProductList != null)
            {
                var vProduct = vProductList.Where(c => c.Id == id).FirstOrDefault();


                if (vProduct != null)
                {
                    var isRemoved = _manager.RemoveValue("products", vProduct);
                    if (isRemoved)
                    {
                        return StatusCode(204);

                    }
                    else
                    {
                        return StatusCode(403, new ErrorModel { ErrorMessage = String.Format(ERROR_MESSAGE, id) });
                    }

                }
                else
                {
                    return StatusCode(403, new ErrorModel { ErrorMessage = String.Format(ERROR_MESSAGE, id) });
                }
            }
            else
            {
                return StatusCode(403, new ErrorModel { ErrorMessage = String.Format(ERROR_MESSAGE, id) });
            }





        }
    }
}
