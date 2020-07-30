using System;
using CategoryLibrary;
using CommonLibrary;
using ExerciseLibrary;
using Microsoft.EntityFrameworkCore;
using ResourceAPI;
using ResourceAPI.ApiServices;
using Xunit;

namespace ResourceAPITests.ExerciseTests
{
    public class ExerciseServiceTests
    {
        public ExerciseServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var context = new SqlContext(optionsBuilder.Options);
            _categoryService = new CategoryService(context);
            _exerciseService = new ExerciseService(context);
            _authorService = new AuthorService(context);
        }

        private readonly IExerciseService _exerciseService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;


        public int CreateScript(int exerciseId)
        {
            var scriptId =
                _exerciseService.CreateScript(exerciseId, new Script {Name = "xxx", Content = "yyy"});

            return scriptId;
        }

        [Fact]
        public int ExerciseCreate()
        {
            var exerciseId = _exerciseService.Create(new Exercise {Name = "abc", Content = "cde"});
            var exercise = _exerciseService.Get(exerciseId);
            Assert.Equal("abc", exercise.Name);
            Assert.Contains("cde", exercise.ContentHtml);
            return exerciseId;
        }

        [Fact]
        public void ExerciseDelete()
        {
            var exerciseid = ExerciseCreate();
            Assert.NotNull(_exerciseService.Get(exerciseid));
            _exerciseService.Delete(exerciseid);
            Assert.Null(_exerciseService.Get(exerciseid));
        }

        [Fact]
        public void ExerciseEdit()
        {
            var exerciseId = ExerciseCreate();
            _exerciseService.Edit(exerciseId, new Exercise {Name = "xxx", Content = "yyy"});
            var exercise = _exerciseService.Get(exerciseId);
            Assert.Equal("xxx", exercise.Name);
            Assert.Contains("yyy", exercise.ContentHtml);
        }

        [Fact]
        public int ScriptCreate()
        {
            var exerciseId = ExerciseCreate();
            var scriptId =
                _exerciseService.CreateScript(exerciseId, new Script {Name = "xxx", Content = "yyy"});

            return scriptId;
        }

        [Fact]
        public void ScriptDelete()
        {
            var exerciseId = ExerciseCreate();
            var scriptId = CreateScript(exerciseId);
            var script1 = _exerciseService.GetScript(exerciseId, scriptId);
            Assert.NotNull(script1);
            _exerciseService.DeleteScript(exerciseId, scriptId);
            var script2 = _exerciseService.GetScript(exerciseId, scriptId);
            Assert.Null(script2);
        }

        [Fact]
        public void ScriptEdit()
        {
            var exerciseId = ExerciseCreate();
            var scriptId = CreateScript(exerciseId);
            var script = _exerciseService.GetScript(exerciseId, scriptId);
            _exerciseService.EditScript(exerciseId, scriptId, script);
        }
    }
}