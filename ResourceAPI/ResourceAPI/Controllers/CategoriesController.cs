using Microsoft.AspNetCore.Mvc;
using ResourceAPI.ApiServices;
using ResourceAPI.Models.Category;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult Browse()
        {
            var categories = _categoryService.Browse();
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult Get(int id)
        {
            var category = _categoryService.Get(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Route("{id}")]
        public ActionResult Create(int id, Category category)
        {
            if (_categoryService.Create(id, category) != 0) return Ok();
            return NotFound();
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult Edit(int id, Category category)
        {
            if (_categoryService.Update(id, category)) return Ok();
            return NotFound();
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult Delete(int id)
        {
            if (_categoryService.Delete(id)) return Ok();
            return NotFound();
        }
    }
}