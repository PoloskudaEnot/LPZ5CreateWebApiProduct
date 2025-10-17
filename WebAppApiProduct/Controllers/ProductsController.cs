using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using WebAppApiProduct.Model;

namespace WebAppApiProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static List<Product> products = new List<Product>(new[]     //Создаем список продуктов
        {
            new Product() {ID = 1, Name = "iPhone 15 Pro",Price = 999.99m, Date = DateTime.Parse("2024-01-15"), Description = "Флагманский смартфон с титановым корпусом, камерой 48 МП и процессором A17 Pro" },
            new Product() {ID = 2, Name = "MacBook Air M2",Price = 1199.99m, Date = DateTime.Parse("2024-02-20"), Description = "Ультратонкий ноутбук с чипом Apple M2, 13-дюймовым дисплеем Retina и 18 часами автономной работы" },
            new Product() {ID = 3, Name = "Sony WH-1000XM5",Price = 349.99m, Date = DateTime.Parse("2024-01-08"), Description = "Беспроводные наушники с шумоподавлением, автономностью до 30 часов и премиальным звуком" },
            new Product() {ID = 4, Name = "Samsung Odyssey G9",Price = 1299.99m, Date = DateTime.Parse("2024-03-10"), Description = "Игровой монитор 49 дюймов с изогнутым экраном, разрешением 5120x1440 и частотой 240 Гц" },
            new Product() {ID = 5, Name = "Dyson V15 Detect",Price = 749.99m, Date = DateTime.Parse("2024-02-05"), Description = "Беспылеводный пылесос с лазерной подсветкой пыли, автоматическим определением типа поверхности и HEPA-фильтром" },
        });

        [HttpGet]       //атрибут, указывающий что метод обрабатывает HTTP GET запросы/ метод для чтения данных
        public IEnumerable<Product> Get() => products;      //Возвращает коллекцию Product

        [HttpGet("{id}")]       //Атрибут маршрутизации
        public IActionResult Get(int id)        //id автоматически извлекается из URL
        {
            var product = products.FirstOrDefault(x => x.ID == id);     //В коллекции находит первый элемент с таким id, либо возвращает null
            if (product == null)
            { 
                return NotFound();      //Если продукт не найден, клиенту возвращает пустой ответ с статусом 404
            }
            return Ok(product);     //Если продукт найден, клиенту возвращает отвеет HTTP 200 OK и автоматически сериализует объект product в Json
        }

        [HttpDelete("{id}")]        //Атрибут маршрутизации
        public IActionResult Delete(int id)     //id автоматически извлекается из URL
        {
            var product = products.FirstOrDefault(x => x.ID == id);     //В коллекции находится первйы элемент с таким id, либо возвращает значение null

            if (product == null)
            {
                return NotFound();      //Если продукт не найден, клиенту возвращает пустой ответ с статусом 404
            }
            products.Remove(product);       //Удалить продукт из коллекции
            return NoContent();        //Вернуть ответ клиенту с статусом 204 об успешно удалении
        }


        private int NextProductId => products.Count() == 0 ? 1 : products.Max(x => x.ID) + 1;

        [HttpGet("GetNextProductId")]
        public int GetNextProductId()
        { 
            return NextProductId;
        }

        //Ниже представлено 2 реализации добавления данных


        //Для web-форм (могут отправлять form-data), источником так же могут быть (query string, route), ASP.Net сам решает откуда брать данные
        [HttpPost]
        public IActionResult Post(Product product)
        {
            //ModelState - это объект ASP.NET Core, который содержит состояние модели после привязки данных, хранит ошибки валидации, отслеживает прошла ли модель все проверки
            if (!ModelState.IsValid)        //Проверяет все правила валидации модели
            { 
                return BadRequest(ModelState);      //Если есть ошибки, возвращает ошибку 404 с подробной информацией об ошибке
            }

            if (product == null)
            {
                return BadRequest("Продукт не может быть пустым, заполните все поля.");
            }

            product.ID = NextProductId;
            products.Add(product);

            return CreatedAtAction(nameof(Get), new { id = product.ID }, product);      //Метод возвращает HTTP 201 Created с дополнительной информацией, является стандратным и наиболее правильным подходом в ASP.NET Core
            //return Ok(product);   Метод возвращает HTTP 200, без ссылки на ресурс, также можно использовать
            

        }


        //Для SPA/Мобильных приложений (отправляют Json), данные берутся только из тела ([FromBody])
        [HttpPost("AddProduct")]
        public IActionResult PostBody([FromBody] Product product) => Post(product);


        [HttpPut]
        public IActionResult Put(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var editProduct = products.FirstOrDefault(p => p.ID == product.ID);
            if (editProduct == null)
            {
                return NotFound();
            }
            editProduct.Name = product.Name;
            editProduct.Price = product.Price;
            editProduct.Date = product.Date;
            editProduct.Description = product.Description;

            return Ok(editProduct);
        }
    }
}
