using CommonLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CategoryLibrary
{
    /// <summary>
    ///     Kategorie - działają jak foldery. Zawierają w sobie listy elementów typu Problem, Quiz oraz Exercise.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService, IAuthorService authorService)
        {
            _categoryService = categoryService;
            _authorService = authorService;
        }

        public int AuthorId => Tools.GetAuthorId(_authorService, HttpContext);

        /// <summary>
        ///     Pobiera element kategorii o podanym id.
        /// </summary>
        /// <param name="id">Identyfikator kategorii.</param>
        /// <returns>Element kategorii wraz z listą linków do podkategorii.</returns>
        [HttpGet("{id}")]
        public ActionResult Get(int id = 1)
        {
            var category = _categoryService.GetCategory(id);
            if (category == null) return NotFound();
            return Ok(category.ToView());
        }

        /// <summary>
        ///     Zwraca kategorię główną wraz z linkami do podkategorii.
        /// </summary>
        /// <returns>Kategoria główna.</returns>
        [HttpGet]
        public ActionResult GetRoot()
        {
            return Get();
        }

        /// <summary>
        ///     Tworzy nową kategorię potomną do kategorii o podanym identyfikatorze.
        /// </summary>
        /// <param name="id">Identyfikator kategorii nadrzędnej.</param>
        /// <param name="category">Nowa kategoria potomna.</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        [HttpPost]
        public ActionResult Post(CategoryUserModel category, int id = 1)
        {
            var newCategory = _categoryService.Create(category.ToModel(), id, AuthorId);
            if (newCategory != null) return Ok(newCategory.ToLink());
            return NotFound();
        }

        /// <summary>
        ///     Edytuje kategorię o danym identyfikatorze na podaną przez użytkownika.
        /// </summary>
        /// <param name="id">Identyfikator kategorii.</param>
        /// <param name="category">Nowa struktura kategorii.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Put(int id, CategoryUserModel category)
        {
            var element = _categoryService.Update(category.ToModel(), id, AuthorId);
            if (element == null) return NotFound();
            return Ok(element.ToLink());
        }

        /// <summary>
        ///     Kasuje kategorię o podanym identyfikatorze wraz ze wszystkimi elementami potomnymi.
        /// </summary>
        /// <param name="id">Identyfikator kategorii.</param>
        /// <returns>Kod statusu html.</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            if (id == 1) return Forbid();
            if (_categoryService.Delete(id, AuthorId)) return Ok();
            return NotFound();
        }

        /// <summary>
        ///     Zwraca linki problemów związanych z kategorią.
        /// </summary>
        /// <param name="id">Identyfikator kategorii.</param>
        /// <returns>Lista linków problemów.</returns>
        [HttpGet("{id}/problems")]
        public ActionResult GetProblems(int id)
        {
            var problems = _categoryService.GetProblems(id);
            if (problems == null) return NotFound();
            return Ok(problems);
        }

        /// <summary>
        ///     Zwraca linki exercises zwiazanych z kategorią.
        /// </summary>
        /// <param name="id">Identyfikator kategorii.</param>
        /// <returns>Lista linków ćwiczeń.</returns>
        [HttpGet("{id}/exercises")]
        public ActionResult GetExercises(int id)
        {
            var exercises = _categoryService.GetExercises(id);
            if (exercises == null) return NotFound();
            return Ok(exercises);
        }

        /// <summary>
        ///     Zwraca linki quizów związanych z kategorią.
        /// </summary>
        /// <param name="id">Identyfikator kategorii.</param>
        /// <returns>Lista linków quizów.</returns>
        [HttpGet("{id}/quizzes")]
        public ActionResult GetQuizTests(int id)
        {
            var quizzes = _categoryService.GetQuizzes(id);
            if (quizzes == null) return NotFound();
            return Ok(quizzes);
        }
    }
}