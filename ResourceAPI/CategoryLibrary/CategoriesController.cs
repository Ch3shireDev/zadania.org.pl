using Microsoft.AspNetCore.Mvc;

namespace CategoryLibrary
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
        public ActionResult GetRoot()
        {
            var categories = _categoryService.GetProblems(1);
            return Ok(categories);
        }

        [HttpPost]
        public ActionResult PostRoot(Category category)
        {
            var element = new Category
            {
                Name = category.Name,
                Description = category.Description
            };

            var categoryId = _categoryService.Create(1, element);
            return Ok(new Category {Id = categoryId});
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var category = _categoryService.GetProblems(id);
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

        [HttpGet("{id}/problems")]
        public ActionResult GetProblems(int id)
        {
            var category = _categoryService.GetProblems(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpGet("{id}/exercises")]
        public ActionResult GetExercises(int id)
        {
            var category = _categoryService.GetExercises(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpGet("{id}/multiple-choice-tests")]
        public ActionResult GetMultipleChoiceTests(int id)
        {
            var category = _categoryService.GetMultipleChoiceTests(id);
            if (category == null) return NotFound();
            return Ok(category);
        }
    }
}