using System;
using System.Net.Http;
using System.Threading.Tasks;
using ExerciseLibrary;
using FileDataLibrary;
using Xunit;

namespace ResourceAPITests.ExerciseTests
{
    public class ExerciseControllerTests
    {
        private HttpClient Client { get; } = new TestClientProvider().Client;

        public async Task<Exercise> ExerciseGet(int exerciseId)
        {
            var res = await Client.GetAsync($"/api/v1/exercises/{exerciseId}");
            if (!res.IsSuccessStatusCode) return null;
            return res.ToElement<Exercise>();
        }

        public async Task<ExerciseVariableData> ScriptGet(int exerciseId, int scriptId)
        {
            var res = await Client.GetAsync($"/api/v1/exercises/{exerciseId}/scripts/{scriptId}");
            if (!res.IsSuccessStatusCode) return null;
            var element = res.ToElement<ExerciseVariableData>();
            return element;
        }

        //[Theory]
        public async Task<int> ScriptNew(int exerciseId)
        {
            //var exerciseId = await ExerciseCreate();
            var res = await Client.PostAsync($"/api/v1/exercises/{exerciseId}/scripts",
                new ExerciseVariableData {Content = "xxx", Name = "yyy"}.ToHttpContent());
            var script = res.ToElement<ExerciseVariableData>();
            var script1 = await ScriptGet(exerciseId, script.Id);
            Assert.Equal("yyy", script1.Name);
            Assert.Equal("xxx", script1.Content);
            return script.Id;
        }


        [Fact]
        public async Task<int> ExerciseCreate()
        {
            var postRes = await Client.PostAsync("/api/v1/exercises",
                new Exercise
                {
                    Name = "xxx", Content = "![](a.png) ![](b.png) ![](c.png)",
                    Files = new[]
                    {
                        new FileDataView
                        {
                            FileBytes = Convert.FromBase64String("aaaa"),
                            FileName = "a.png"
                        },
                        new FileDataView
                        {
                            FileBytes = Convert.FromBase64String("bbbb"),
                            FileName = "b.png"
                        },
                        new FileDataView
                        {
                            FileBytes = Convert.FromBase64String("cccc"),
                            FileName = "c.png"
                        }
                    }
                }.ToHttpContent());
            var postEx = postRes.ToElement<Exercise>();
            var exerciseRes = await Client.GetAsync($"/api/v1/exercises/{postEx.Id}");
            var exercise = exerciseRes.ToElement<Exercise>();
            Assert.Equal("xxx", exercise.Name);
            Assert.Contains("aaaa", exercise.Content);
            Assert.Contains("bbbb", exercise.Content);
            Assert.Contains("cccc", exercise.Content);
            return exercise.Id;
        }

        [Fact]
        public async void ExerciseDelete()
        {
            var exerciseId = await ExerciseCreate();
            var exercise1 = await ExerciseGet(exerciseId);
            Assert.NotNull(exercise1);
            await Client.DeleteAsync($"/api/v1/exercises/{exerciseId}");
            var exercise2 = await ExerciseGet(exerciseId);
            Assert.Null(exercise2);
        }

        [Fact]
        public async void ExerciseEdit()
        {
            var exerciseId = await ExerciseCreate();


            var exercise = await ExerciseGet(exerciseId);
            exercise.Name = "abc";
            exercise.Content = "![](x.png) ![](y.png) ![](z.png)";
            exercise.Files = new[]
            {
                new FileDataView {FileBytes = Convert.FromBase64String("xxxx"), FileName = "x.png"},
                new FileDataView {FileBytes = Convert.FromBase64String("yyyy"), FileName = "y.png"},
                new FileDataView {FileBytes = Convert.FromBase64String("zzzz"), FileName = "z.png"}
            };
            await Client.PutAsync($"/api/v1/exercises/{exerciseId}", exercise.ToHttpContent());
            var exercise2 = await ExerciseGet(exerciseId);
            Assert.Equal("abc", exercise2.Name);
            Assert.Contains("xxxx", exercise2.Content);
            Assert.Contains("yyyy", exercise2.Content);
            Assert.Contains("zzzz", exercise2.Content);
        }

        [Fact]
        public async void ScriptCreate()
        {
            var exerciseId = await ExerciseCreate();
            await ScriptNew(exerciseId);
        }

        [Fact]
        public async void ScriptDelete()
        {
            var exerciseId = await ExerciseCreate();
            var scriptId = await ScriptNew(exerciseId);
            var script1 = await ScriptGet(exerciseId, scriptId);
            Assert.NotNull(script1);
            await Client.DeleteAsync($"/api/v1/exercises/{exerciseId}/scripts/{scriptId}");
            var script2 = await ScriptGet(exerciseId, scriptId);
            Assert.Null(script2);
        }

        [Fact]
        public async void ScriptEdit()
        {
            var exerciseId = await ExerciseCreate();
            var scriptId = await ScriptNew(exerciseId);
            var script = await ScriptGet(exerciseId, scriptId);
            script.Name = "aaa";
            script.Content = "bbb";
            await Client.PutAsync($"/api/v1/exercises/{exerciseId}/scripts/{scriptId}", script.ToHttpContent());
            var exercise2 = await ScriptGet(exerciseId, scriptId);
            Assert.Equal("aaa", exercise2.Name);
            Assert.Equal("bbb", exercise2.Content);
        }
    }
}