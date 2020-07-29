using System.Linq;
using System.Net;
using System.Net.Http;
using ExerciseLibrary;
using MultipleChoiceLibrary;
using Xunit;

namespace ResourceAPITests.Category
{
    public class CategoryControllerTests
    {
        private HttpClient Client { get; } = new TestClientProvider().Client;

        [Fact]
        public async void BrowseCategories()
        {
            // Tworzymy nowe kategorie, w założeniu należące do pnia.
            await Client.PostAsync("/api/v1/categories", new CategoryLibrary.Category {Name = "xxx"}.ToHttpContent());
            await Client.PostAsync("/api/v1/categories", new CategoryLibrary.Category {Name = "yyy"}.ToHttpContent());
            await Client.PostAsync("/api/v1/categories", new CategoryLibrary.Category {Name = "zzz"}.ToHttpContent());

            // Pobieramy kategorię pnia.
            var response = await Client.GetAsync("/api/v1/categories/");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Kategoria pnia powinna nazywać się root.
            var parentCategory = response.ToElement<CategoryLibrary.Category>();
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
            var category = response.ToElement<CategoryLibrary.Category>();
            Assert.Equal("Root", category.Name);
        }

        [Fact]
        public async void GetCategoryExercises()
        {
            var res0 = await Client.PostAsync("/api/v1/categories",
                new CategoryLibrary.Category {Name = "aaa"}.ToHttpContent());
            var cid = res0.ToElement<CategoryLibrary.Category>().Id;

            await Client.PostAsync("/api/v1/exercises", new Exercise {Name = "xxx", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/exercises", new Exercise {Name = "yyy", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/exercises", new Exercise {Name = "zzz", CategoryId = cid}.ToHttpContent());

            var res = await Client.GetAsync($"/api/v1/categories/{cid}/exercises");
            var category = res.ToElement<CategoryLibrary.Category>();

            Assert.Equal(cid, category.Id);
            Assert.Equal(1, category.ParentId);
            Assert.Equal(3, category.Exercises.Count());

            var names = category.Exercises.Select(p => p.Name).ToList();

            Assert.Contains("xxx", names);
            Assert.Contains("yyy", names);
            Assert.Contains("zzz", names);
        }

        [Fact]
        public async void GetCategoryMultipleChoiceTests()
        {
            var res0 = await Client.PostAsync("/api/v1/categories",
                new CategoryLibrary.Category {Name = "aaa"}.ToHttpContent());
            var cid = res0.ToElement<CategoryLibrary.Category>().Id;

            await Client.PostAsync("/api/v1/multiple-choice-tests",
                new MultipleChoiceTest {Name = "xxx", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/multiple-choice-tests",
                new MultipleChoiceTest {Name = "yyy", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/multiple-choice-tests",
                new MultipleChoiceTest {Name = "zzz", CategoryId = cid}.ToHttpContent());

            var res = await Client.GetAsync($"/api/v1/categories/{cid}/multiple-choice-tests");
            var category = res.ToElement<CategoryLibrary.Category>();

            Assert.Equal(cid, category.Id);
            Assert.Equal(1, category.ParentId);
            Assert.Equal(3, category.MultipleChoiceTests.Count());

            var names = category.MultipleChoiceTests.Select(p => p.Name).ToList();

            Assert.Contains("xxx", names);
            Assert.Contains("yyy", names);
            Assert.Contains("zzz", names);
        }

        [Fact]
        public async void GetCategoryProblems()
        {
            var res0 = await Client.PostAsync("/api/v1/categories",
                new CategoryLibrary.Category {Name = "aaa"}.ToHttpContent());
            var cid = res0.ToElement<CategoryLibrary.Category>().Id;

            await Client.PostAsync("/api/v1/problems",
                new ProblemLibrary.Problem {Name = "xxx", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/problems",
                new ProblemLibrary.Problem {Name = "yyy", CategoryId = cid}.ToHttpContent());
            await Client.PostAsync("/api/v1/problems",
                new ProblemLibrary.Problem {Name = "zzz", CategoryId = cid}.ToHttpContent());

            var res = await Client.GetAsync($"/api/v1/categories/{cid}/problems");
            var category = res.ToElement<CategoryLibrary.Category>();

            Assert.Equal(cid, category.Id);
            Assert.Equal(1, category.ParentId);
            Assert.Equal(3, category.Problems.Count());

            var names = category.Problems.Select(p => p.Name).ToList();

            Assert.Contains("xxx", names);
            Assert.Contains("yyy", names);
            Assert.Contains("zzz", names);
        }

        [Fact]
        public async void PostCategory()
        {
            var res = await Client.PostAsync("/api/v1/categories",
                new CategoryLibrary.Category {Name = "xxx", Description = "yyy"}.ToHttpContent());
            var id = res.ToElement<CategoryLibrary.Category>().Id;
            var getRes = await Client.GetAsync($"/api/v1/categories/{id}");
            var category = getRes.ToElement<CategoryLibrary.Category>();
            Assert.Equal(1, category.ParentId);
            Assert.Equal("xxx", category.Name);
            Assert.Contains("yyy", category.DescriptionHtml);
        }

        [Fact]
        public async void PostIntoCategory()
        {
            var res0 = await Client.PostAsync("/api/v1/categories/1",
                new CategoryLibrary.Category {Name = "aaa"}.ToHttpContent());
            var cid0 = res0.ToElement<CategoryLibrary.Category>().Id;

            var category1 = new CategoryLibrary.Category {Name = "xxx"};
            var category2 = new CategoryLibrary.Category {Name = "yyy"};
            var category3 = new CategoryLibrary.Category {Name = "zzz"};

            var res1 = await Client.PostAsync($"/api/v1/categories/{cid0}", category1.ToHttpContent());
            var res2 = await Client.PostAsync($"/api/v1/categories/{cid0}", category2.ToHttpContent());
            var res3 = await Client.PostAsync($"/api/v1/categories/{cid0}", category3.ToHttpContent());

            var cid1 = res1.ToElement<CategoryLibrary.Category>().Id;
            var cid2 = res2.ToElement<CategoryLibrary.Category>().Id;
            var cid3 = res3.ToElement<CategoryLibrary.Category>().Id;

            var res = await Client.GetAsync($"/api/v1/categories/{cid0}");

            var cat = res.ToElement<CategoryLibrary.Category>();
            var categories = cat.Categories.ToList();
            Assert.Contains(categories, c => c.Id == cid1);
            Assert.Contains(categories, c => c.Id == cid2);
            Assert.Contains(categories, c => c.Id == cid3);
        }
    }
}