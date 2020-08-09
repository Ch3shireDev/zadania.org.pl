using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CategoryLibrary;
using FileDataLibrary;
using ProblemLibrary;
using Xunit;

namespace ResourceAPITests.ProblemTests
{
    public class ProblemControllerTests
    {
        private readonly HttpClient _client = new TestClientProvider().Client;

        private async Task<ProblemView> GetProblem(int problemId)
        {
            var problemRes = await _client.GetAsync($"/api/v1/problems/{problemId}");
            var problem = problemRes.ToElement<ProblemView>();
            return problem;
        }

        private async Task<ProblemView> GetProblemAsync(int problemId)
        {
            var problem = await _client.GetAsync($"/api/v1/problems/{problemId}");
            return problem.ToElement<ProblemView>();
        }

        [Fact]
        public async Task AnswerApprove()
        {
            var problemId = await ProblemCreate();

            var answer1 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "xxx"});
            var answer2 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "yyy"});
            var answer3 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "zzz"});

            // Pobieramy problem.
            var problem = await GetProblem(problemId);

            // Problem powinien zawierać informację że nie znalazło się rozwiązanie.
            Assert.False(problem.IsSolved);
            Assert.Empty(problem.Answers.Select(a => a.IsApproved).Where(approved => approved));

            // Zatwierdzamy odpowiedź jako poprawną.
            await _client.PatchAsync($"/api/v1/problems/{problemId}/answers/{answer1.Id}",
                new Answer {IsApproved = true}.ToHttpContent());

            // Problem powinien mieć teraz status rozwiązanego.
            var problem1 = await GetProblem(problemId);
            Assert.True(problem1.IsSolved);

            // Powinna być tylko jedna odpowiedź będąca rozwiązaniemm.
            Assert.Contains(true, problem1.Answers.Select(a => a.IsApproved));
            Assert.Equal(1, problem1.Answers.Count(a => a.IsApproved));

            // Zatwierdzamy drugą odpowiedź jako poprawną.
            await _client.PatchAsync($"/api/v1/problems/{problemId}/answers/{answer2.Id}",
                new Answer {IsApproved = true}.ToHttpContent());

            // W dalszym ciągu problem powinien być rozwiązany i tylko jedna odpowiedź powinna być właściwym rozwiazaniem.
            var problem2 = await GetProblem(problemId);
            Assert.True(problem2.IsSolved);
            Assert.Contains(true, problem2.Answers.Select(a => a.IsApproved));
            Assert.Equal(1, problem2.Answers.Count(a => a.IsApproved));

            // Anulujemy drugą odpowiedź jako poprawną.
            await _client.PatchAsync($"/api/v1/problems/{problemId}/answers/{answer2.Id}",
                new Answer {IsApproved = false}.ToHttpContent());

            // Problem powinien mieć ponownie status nierozwiazanego.
            var problem3 = await GetProblem(problemId);
            Assert.False(problem3.IsSolved);
            Assert.Empty(problem3.Answers.Where(a => a.IsApproved));
        }

        [Fact]
        public async Task AnswerCreate()
        {
            var problemId = await ProblemCreate();
            var answer1 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "xxx"});
            var answer2 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "yyy"});
            var answer3 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "zzz"});

            var problem = await GetProblem(problemId);

            // Pobrane odpowiedzi powinny mieć ustalone wcześniej id.
            var answers = problem.Answers.ToList();
            Assert.Contains(true, answers.Select(a => a.Id == answer1.Id));
            Assert.Contains(true, answers.Select(a => a.Id == answer2.Id));
            Assert.Contains(true, answers.Select(a => a.Id == answer3.Id));

            // Wewnątrz pobranego problemu powinny być wartości utworzonych wcześniej odpowiedzi.
            var contents = answers.Select(p => p.Content).ToList();
            Assert.Contains(true, contents.Select(c => c.Contains("xxx")));
            Assert.Contains(true, contents.Select(c => c.Contains("yyy")));
            Assert.Contains(true, contents.Select(c => c.Contains("zzz")));
        }

        [Fact]
        public async Task AnswerDelete()
        {
            var problemId = await ProblemCreate();

            var answer1 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "xxx"});
            var answer2 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "yyy"});
            var answer3 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "zzz"});

            // Pobieramy problem z bazy.
            var problem1 = await GetProblem(problemId);

            // Sprawdzamy czy odpowiedzi mają id jakich się spodziewamy.
            Assert.Contains(true, problem1.Answers.Select(a => a.Id == answer1.Id));
            Assert.Contains(true, problem1.Answers.Select(a => a.Id == answer2.Id));
            Assert.Contains(true, problem1.Answers.Select(a => a.Id == answer3.Id));

            // Sprawdzamy czy odpowiedzi mają zawartości jakich się spodziewamy.
            Assert.Contains(true, problem1.Answers.Select(a => a.Content.Contains("xxx")));
            Assert.Contains(true, problem1.Answers.Select(a => a.Content.Contains("yyy")));
            Assert.Contains(true, problem1.Answers.Select(a => a.Content.Contains("zzz")));

            // Usuwamy odpowiedź.
            await _client.DeleteAsync($"/api/v1/problems/{problemId}/answers/{answer1.Id}");

            // Pobieramy problem z bazy.
            var problem2 = await GetProblem(problemId);

            // Sprawdzamy czy odpowiedzi mają id jakich się spodziewamy.
            Assert.Empty(problem2.Answers.Select(a => a.Id).Where(id => id == answer1.Id));
            Assert.Contains(true, problem2.Answers.Select(a => a.Id == answer2.Id));
            Assert.Contains(true, problem2.Answers.Select(a => a.Id == answer3.Id));

            // Pobieramy problem z bazy.
            var problem3 = await GetProblem(problemId);

            // Sprawdzamy czy odpowiedzi mają zawartości jakich się spodziewamy.
            Assert.Empty(problem3.Answers.Select(a => a.Content).Where(c => c.Contains("xxx")));
            Assert.Contains(true, problem3.Answers.Select(a => a.Content.Contains("yyy")));
            Assert.Contains(true, problem3.Answers.Select(a => a.Content.Contains("zzz")));
        }

        [Fact]
        public async Task AnswerEdit()
        {
            // Tworzymy środowisko.
            var problemId = await ProblemCreate();
            var answer1 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "xxx"});

            // Pobieramy problem i sprawdzamy czy udzielona odpowiedź spełnia warunki.
            var problem1 = await GetProblem(problemId);
            Assert.Contains(true, problem1.Answers.Select(a => a.Id == answer1.Id));
            Assert.Contains(true, problem1.Answers.Select(a => a.Content.Contains("xxx")));

            // Wysyłamy edycję na serwer.
            await _client.PutAsync($"/api/v1/problems/{problemId}/answers/{answer1.Id}",
                new Answer {Content = "yyy"}.ToHttpContent());

            // Odpowiedź powinna mieć dalej to samo id, ale zmienioną zawartość.
            var problem2 = await GetProblem(problemId);
            Assert.Contains(true, problem2.Answers.Select(a => a.Id == answer1.Id));
            Assert.Contains(true, problem2.Answers.Select(a => a.Content.Contains("yyy")));
        }

        [Fact]
        public async Task FileAnswerCreate()
        {
            throw new Exception();
        }


        [Fact]
        public async Task FileAnswerEdit()
        {
            throw new Exception();
        }


        [Fact]
        public async Task FileProblemCreate()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde ![](aaa.png) ![](bbb.png) ![](ccc.png)",

                Files = new[]
                {
                    new FileDataView {FileName = "aaa.png", FileBytes = Convert.FromBase64String("aaaa")},
                    new FileDataView {FileName = "bbb.png", FileBytes = Convert.FromBase64String("bbbb")},
                    new FileDataView {FileName = "ccc.png", FileBytes = Convert.FromBase64String("cccc")}
                }
            };

            var problemInfo = await _client.PostAsync("/api/v1/problems", problem);
            var problemView = await GetProblemAsync(problemInfo.Id);

            Assert.Contains("aaaa", problemView.Content);
            Assert.Contains("bbbb", problemView.Content);
            Assert.Contains("cccc", problemView.Content);

            await _client.DeleteAsync($"/api/v1/problems/{problemInfo.Id}");

            var resDelete = await _client.GetAsync($"/api/v1/problems/{problemInfo.Id}");
            Assert.Equal(HttpStatusCode.NotFound, resDelete.StatusCode);
        }

        [Fact]
        public async Task FileProblemEdit()
        {
            throw new Exception();
        }

        [Fact]
        public async Task ProblemBrowse()
        {
            var response = await _client.GetAsync("/api/v1/problems/");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task<int> ProblemCreate()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde"
            };

            // Utworzenie problemu powinno zwrócić wartość id.
            var response = await _client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<ProblemView>();

            // Problem powinien być pod wskazanym id.
            var response2 = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var probRes = response2.ToElement<ProblemView>();
            Assert.Equal("abc", probRes.Name);
            Assert.Contains("cde", probRes.Content);

            // Problem powinien wylądować w kategorii 1, Root.
            var catRes = await _client.GetAsync("/api/v1/categories/1");
            var cat = catRes.ToElement<CategoryView>();
            //Assert.Equal("abc", cat.Problems.First().Name);

            return resProblem.Id;
        }

        [Fact]
        public async Task ProblemDelete()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde"
            };

            // Utworzenie problemu powinno zwrócić wartość id.
            var response = await _client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<ProblemView>();

            // Problem powinien być pod wskazanym id.
            var response2 = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var probRes = response2.ToElement<ProblemView>();
            Assert.Equal("abc", probRes.Name);
            Assert.Contains("cde", probRes.Content);

            // Kasujemy zasób.
            var resDel = await _client.DeleteAsync($"/api/v1/problems/{resProblem.Id}");
            resDel.EnsureSuccessStatusCode();

            // Problemu powinno nie być pod wskazanym id.
            var response3 = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            Assert.Equal(HttpStatusCode.NotFound, response3.StatusCode);
        }

        [Fact]
        public async Task ProblemEdit()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde"
            };

            // Utworzenie problemu powinno zwrócić wartość id.
            var response = await _client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<ProblemView>();

            // Bieżący problem powinien zawierać nowe wartości.
            var problem1Res = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var problem1 = problem1Res.ToElement<ProblemView>();
            Assert.Equal("abc", problem1.Name);
            Assert.Contains("cde", problem1.Content);

            // Zmieniamy parametry problemu.
            problem.Name = "xyz";
            problem.Content = "zzz";

            // Wysyłamy nową wersję.
            await _client.PutAsync($"/api/v1/problems/{resProblem.Id}", problem.ToHttpContent());

            // Bieżący problem powinien zawierać nowe wartości.
            var problem2Res = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var problem2 = problem2Res.ToElement<ProblemView>();
            Assert.Equal("xyz", problem2.Name);
            Assert.Contains("zzz", problem2.Content);
        }
    }
}