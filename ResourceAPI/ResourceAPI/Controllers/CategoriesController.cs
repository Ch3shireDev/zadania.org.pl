using Microsoft.AspNetCore.Mvc;
using ResourceAPI.ApiServices.Interfaces;
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

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var category = _categoryService.Get(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost("{id}")]
        public ActionResult Post(int id, Category category)
        {
            var cid = _categoryService.Create(id, category);
            if (cid != 0) return Ok(new Category {Id = cid});
            return NotFound();
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, Category category)
        {
            if (_categoryService.Update(id, category)) return Ok();
            return NotFound();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (_categoryService.Delete(id)) return Ok();
            return NotFound();
        }
    }
}