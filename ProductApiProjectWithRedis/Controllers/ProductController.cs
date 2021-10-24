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
            var product = _manager.Get<List<Product>>("products").Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
                return StatusCode(403, new ErrorModel { ErrorMessage = String.Format(ERROR_MESSAGE, id) });


            return StatusCode(200, product);


        }

        [HttpPost]
        public ActionResult Insert(Product product)
        {


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

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {

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
