using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ResourceAPI.Models.Category;
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
            await Client.PostAsync("/api/v1/categories", new Category {Name = "xxx"}.ToHttpContent());
            await Client.PostAsync("/api/v1/categories", new Category {Name = "yyy"}.ToHttpContent());
            await Client.PostAsync("/api/v1/categories", new Category {Name = "zzz"}.ToHttpContent());
            
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
        public async void PostCategory()
        {
            var res = await Client.PostAsync("/api/v1/categories", new Category {Name = "xxx", Description = "yyy"}.ToHttpContent());
            var id = res.ToElement<Category>().Id;
            var getRes = await Client.GetAsync($"/api/v1/categories/{id}");
            var category = getRes.ToElement<Category>();
            Assert.Equal(1, category.ParentId);
            Assert.Equal("xxx", category.Name);
            Assert.Contains("yyy", category.DescriptionHtml);
        }

        [Fact]
        public async void PostIntoCategory()
        {
            var res0 = await Client.PostAsync("/api/v1/categories/1", new Category {Name = "aaa"}.ToHttpContent());
            var cid0 = res0.ToElement<Category>().Id;

            var category1 = new Category {Name = "xxx"};
            var category2 = new Category {Name = "yyy"};
            var category3 = new Category {Name = "zzz"};

            var res1 = await Client.PostAsync($"/api/v1/categories/{cid0}", category1.ToHttpContent());
            var res2 = await Client.PostAsync($"/api/v1/categories/{cid0}", category2.ToHttpContent());
            var res3 = await Client.PostAsync($"/api/v1/categories/{cid0}", category3.ToHttpContent());

            var cid1 = res1.ToElement<Category>().Id;
            var cid2 = res2.ToElement<Category>().Id;
            var cid3 = res3.ToElement<Category>().Id;

            var res = await Client.GetAsync($"/api/v1/categories/{cid0}");

            var cat = res.ToElement<Category>();
            var categories = cat.Categories.ToList();
            Assert.Contains(categories, c => c.Id == cid1);
            Assert.Contains(categories, c => c.Id == cid2);
            Assert.Contains(categories, c => c.Id == cid3);
        }
    }
}