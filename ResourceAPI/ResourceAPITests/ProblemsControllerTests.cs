using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CategoryLibrary;
using ProblemLibrary;
using Xunit;

namespace ResourceAPITests
{
    public class ProblemsControllerTests
    {
        private readonly HttpClient _client = new TestClientProvider().Client;

        private async Task<Problem> GetProblem(int problemId)
        {
            var problemRes = await _client.GetAsync($"/api/v1/problems/{problemId}");
            var problem = problemRes.ToElement<Problem>();
            return problem;
        }

        [Fact]
        public async Task ApproveAnswer()
        {
            var problemId = await CreateProblem();

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
        public async Task BrowseProblems()
        {
            var response = await _client.GetAsync("/api/v1/problems/");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateAnswer()
        {
            var problemId = await CreateProblem();
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
            var contents = answers.Select(p => p.ContentHtml).ToList();
            Assert.Contains(true, contents.Select(c => c.Contains("xxx")));
            Assert.Contains(true, contents.Select(c => c.Contains("yyy")));
            Assert.Contains(true, contents.Select(c => c.Contains("zzz")));
        }

        [Fact]
        public async Task<int> CreateProblem()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde",
                ContentHtml = "cde"
            };

            // Utworzenie problemu powinno zwrócić wartość id.
            var response = await _client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<Problem>();

            // Problem powinien być pod wskazanym id.
            var response2 = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var probRes = response2.ToElement<Problem>();
            Assert.Equal("abc", probRes.Name);
            Assert.Contains("cde", probRes.ContentHtml);

            // Problem powinien wylądować w kategorii 1, Root.
            var catRes = await _client.GetAsync("/api/v1/categories/1");
            var cat = catRes.ToElement<Category>();
            Assert.Equal("abc", cat.Problems.First().Name);

            return resProblem.Id;
        }

        [Fact]
        public async Task DeleteAnswer()
        {
            var problemId = await CreateProblem();

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
            Assert.Contains(true, problem1.Answers.Select(a => a.ContentHtml.Contains("xxx")));
            Assert.Contains(true, problem1.Answers.Select(a => a.ContentHtml.Contains("yyy")));
            Assert.Contains(true, problem1.Answers.Select(a => a.ContentHtml.Contains("zzz")));

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
            Assert.Empty(problem3.Answers.Select(a => a.ContentHtml).Where(c => c.Contains("xxx")));
            Assert.Contains(true, problem3.Answers.Select(a => a.ContentHtml.Contains("yyy")));
            Assert.Contains(true, problem3.Answers.Select(a => a.ContentHtml.Contains("zzz")));
        }

        [Fact]
        public async Task DeleteProblem()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde"
            };

            // Utworzenie problemu powinno zwrócić wartość id.
            var response = await _client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<Problem>();

            // Problem powinien być pod wskazanym id.
            var response2 = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var probRes = response2.ToElement<Problem>();
            Assert.Equal("abc", probRes.Name);
            Assert.Contains("cde", probRes.ContentHtml);

            // Kasujemy zasób.
            var resDel = await _client.DeleteAsync($"/api/v1/problems/{resProblem.Id}");
            resDel.EnsureSuccessStatusCode();

            // Problemu powinno nie być pod wskazanym id.
            var response3 = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            Assert.Equal(HttpStatusCode.NotFound, response3.StatusCode);
        }

        [Fact]
        public async Task EditAnswer()
        {
            // Tworzymy środowisko.
            var problemId = await CreateProblem();
            var answer1 =
                await _client.PostAsync($"/api/v1/problems/{problemId}/answers", new Answer {Content = "xxx"});

            // Pobieramy problem i sprawdzamy czy udzielona odpowiedź spełnia warunki.
            var problem1 = await GetProblem(problemId);
            Assert.Contains(true, problem1.Answers.Select(a => a.Id == answer1.Id));
            Assert.Contains(true, problem1.Answers.Select(a => a.ContentHtml.Contains("xxx")));

            // Wysyłamy edycję na serwer.
            await _client.PutAsync($"/api/v1/problems/{problemId}/answers/{answer1.Id}",
                new Answer {Content = "yyy"}.ToHttpContent());

            // Odpowiedź powinna mieć dalej to samo id, ale zmienioną zawartość.
            var problem2 = await GetProblem(problemId);
            Assert.Contains(true, problem2.Answers.Select(a => a.Id == answer1.Id));
            Assert.Contains(true, problem2.Answers.Select(a => a.ContentHtml.Contains("yyy")));
        }

        [Fact]
        public async Task EditProblem()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde"
            };

            // Utworzenie problemu powinno zwrócić wartość id.
            var response = await _client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<Problem>();

            // Bieżący problem powinien zawierać nowe wartości.
            var problem1Res = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var problem1 = problem1Res.ToElement<Problem>();
            Assert.Equal("abc", problem1.Name);
            Assert.Contains("cde", problem1.ContentHtml);

            // Zmieniamy parametry problemu.
            problem.Name = "xyz";
            problem.Content = "zzz";

            // Wysyłamy nową wersję.
            await _client.PutAsync($"/api/v1/problems/{resProblem.Id}", problem.ToHttpContent());

            // Bieżący problem powinien zawierać nowe wartości.
            var problem2Res = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var problem2 = problem2Res.ToElement<Problem>();
            Assert.Equal("xyz", problem2.Name);
            Assert.Contains("zzz", problem2.ContentHtml);
        }
    }
}