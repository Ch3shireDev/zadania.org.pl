using System;
using CategoryLibrary;
using CommonLibrary;
using ExerciseLibrary;
using FileDataLibrary;
using Microsoft.EntityFrameworkCore;
using ResourceAPI;
using Xunit;

namespace ResourceAPITests.ExerciseTests
{
    public class ExerciseServiceTests
    {
        public ExerciseServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var context = new SqlContext(optionsBuilder.Options);
            _fileDataService = new FileDataService(context);
            _categoryService = new CategoryService(context);
            _exerciseService = new ExerciseService(context, _fileDataService);
            _authorService = new AuthorService(context);
        }

        private readonly IExerciseService _exerciseService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;
        private readonly IFileDataService _fileDataService;


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
            Assert.Contains("cde", exercise.Content);
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
            Assert.Contains("yyy", exercise.Content);
        }

        [Fact]
        public void FileExerciseCreate()
        {
            var exerciseId = _exerciseService.Create(new Exercise
                {
                    Content = "![](a.png) ![](b.png) ![](c.png)", Name = "xxx", Files = new[]
                    {
                        new FileDataView {FileName = "a.png", FileBytes = Convert.FromBase64String("aaaa")},
                        new FileDataView {FileName = "b.png", FileBytes = Convert.FromBase64String("bbbb")},
                        new FileDataView {FileName = "c.png", FileBytes = Convert.FromBase64String("cccc")}
                    }
                }
            );

            var exercise = _exerciseService.Get(exerciseId);
            Assert.Contains("aaaa", exercise.Content);
            Assert.Contains("bbbb", exercise.Content);
            Assert.Contains("cccc", exercise.Content);
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