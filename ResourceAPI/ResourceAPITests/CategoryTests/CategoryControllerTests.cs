using System.Linq;
using System.Net;
using System.Net.Http;
using CategoryLibrary;
using ExerciseLibrary;
using ProblemLibrary;
using QuizLibrary;
using Xunit;

namespace ResourceAPITests.CategoryTests
{
    public class CategoryControllerTests
    {
        private HttpClient Client { get; } = new TestClientProvider().Client;

        [Fact]
        public async void BrowseCategories()
        {
            // Tworzymy nowe kategorie, w założeniu należące do pnia.
            await Client.PostAsync("/api/v1/categories/1", new Category {Name = "xxx"}.ToHttpContent());
            await Client.PostAsync("/api/v1/categories/1", new Category {Name = "yyy"}.ToHttpContent());
            await Client.PostAsync("/api/v1/categories/1", new Category {Name = "zzz"}.ToHttpContent());

            // Pobieramy kategorię pnia.
            var response = await Client.GetAsync("/api/v1/categories/");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Kategoria pnia powinna nazywać się root.
            var parentCategory = response.ToElement<Category>();
            Assert.Equal("Root", parentCategory.Name);

            // Wśród potomków kategorii pnia powinny być kategorie o wcześniej stworzonych nazwach.
            var names = parentCategory.Categories.Select(c => c.Name);
            Assert.Contains("xxx", names);
            Assert.Contains("yyy", names);
            Assert.Contains("zzz", names);
        }

        [Fact]
        public async void GetCategories()
        {
            var response = await Client.GetAsync("/api/v1/categories/1");
            response.EnsureSuccessStatusCode();
            var category = response.ToElement<Category>();
            Assert.Equal("Root", category.Name);
        }

        [Fact]
        public async void GetCategoryExercises()
        {
            var res0 = await Client.PostAsync("/api/v1/categories/1",
                new Category {Name = "aaa"}.ToHttpContent());
            var cid = res0.ToElement<Category>().Id;

            await Client.PostAsync("/api/v1/exercises", new Exercise {Name = "xxx", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/exercises", new Exercise {Name = "yyy", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/exercises", new Exercise {Name = "zzz", CategoryId = cid}.ToHttpContent());

            var res = await Client.GetAsync($"/api/v1/categories/{cid}/exercises");
            var category = res.ToElement<Category>();

            //Assert.Equal(cid, category.Id);
            //Assert.Equal(1, category.ParentId);
            Assert.Equal(3, category.Exercises.Count());

            var names = category.Exercises.Select(p => p.Name).ToList();

            Assert.Contains("xxx", names);
            Assert.Contains("yyy", names);
            Assert.Contains("zzz", names);
        }

        [Fact]
        public async void GetCategoryProblems()
        {
            var res0 = await Client.PostAsync("/api/v1/categories/1",
                new Category {Name = "aaa"}.ToHttpContent());
            var cid = res0.ToElement<Category>().Id;

            await Client.PostAsync("/api/v1/problems",
                new Problem {Name = "xxx", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/problems",
                new Problem {Name = "yyy", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/problems",
                new Problem {Name = "zzz", CategoryId = cid}.ToHttpContent());

            var res = await Client.GetAsync($"/api/v1/categories/{cid}/problems");
            var category = res.ToElement<Category>();

            Assert.Equal(cid, category.Id);
            Assert.Equal(1, category.ParentId);
            Assert.Equal(3, category.Problems.Count());

            var names = category.Problems.Select(p => p.Name).ToList();

            Assert.Contains("xxx", names);
            Assert.Contains("yyy", names);
            Assert.Contains("zzz", names);
        }

        [Fact]
        public async void GetCategoryQuizTests()
        {
            var res0 = await Client.PostAsync("/api/v1/categories/1",
                new Category {Name = "aaa"}.ToHttpContent());
            var cid = res0.ToElement<CategoryLink>().Id;

            await Client.PostAsync("/api/v1/quiz",
                new Quiz {Name = "xxx", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/quiz",
                new Quiz {Name = "yyy", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/quiz",
                new Quiz {Name = "zzz", CategoryId = cid}.ToHttpContent());

            var res = await Client.GetAsync($"/api/v1/categories/{cid}/quiz");
            var category = res.ToElement<Category>();

            Assert.Equal(cid, category.Id);
            Assert.Equal(1, category.ParentId);
            Assert.Equal(3, category.QuizTests.Count());

            var names = category.QuizTests.Select(p => p.Name).ToList();

            Assert.Contains("xxx", names);
            Assert.Contains("yyy", names);
            Assert.Contains("zzz", names);
        }

        [Fact]
        public async void PostCategory()
        {
            var res = await Client.PostAsync("/api/v1/categories/1",
                new Category {Name = "xxx", Description = "yyy"}.ToHttpContent());
            var url = res.ToElement<CategoryLink>().Url;
            var getRes = await Client.GetAsync(url);
            var category = getRes.ToElement<CategoryView>();
            Assert.Equal("xxx", category.Name);
            Assert.Contains("yyy", category.Description);
        }

        [Fact]
        public async void PostIntoCategory()
        {
            var res0 = await Client.PostAsync("/api/v1/categories/1",
                new Category {Name = "aaa"}.ToHttpContent());
            var url = res0.ToElement<CategoryLink>().Url;

            var category1 = new Category {Name = "xxx"};
            var category2 = new Category {Name = "yyy"};
            var category3 = new Category {Name = "zzz"};

            var res1 = await Client.PostAsync(url, category1.ToHttpContent());
            var res2 = await Client.PostAsync(url, category2.ToHttpContent());
            var res3 = await Client.PostAsync(url, category3.ToHttpContent());

            var url1 = res1.ToElement<CategoryView>().Url;
            var url2 = res2.ToElement<CategoryView>().Url;
            var url3 = res3.ToElement<CategoryView>().Url;

            var res = await Client.GetAsync(url);

            var cat = res.ToElement<CategoryView>();
            var categories = cat.Categories.ToList();
            Assert.Contains(categories, c => c.Url == url1);
            Assert.Contains(categories, c => c.Url == url2);
            Assert.Contains(categories, c => c.Url == url3);
            //Assert.Contains(categories, c => c.Id == cid2);
            //Assert.Contains(categories, c => c.Id == cid3);
        }
    }
}