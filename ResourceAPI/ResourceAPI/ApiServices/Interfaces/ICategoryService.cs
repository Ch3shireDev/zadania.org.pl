using System.Collections.Generic;
using ResourceAPI.Models.Category;

namespace ResourceAPI.ApiServices.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<Category> Browse();
        Category Get(int id);
        int Create(int parentId, Category category);
        bool Update(int parentId, Category category);

        bool Delete(int id);
        //bool SetParent(int parentId, int childId);
    }
}