using System.Collections.Generic;
using ResourceAPI.Models.Category;

namespace ResourceAPI.ApiServices.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<Category> Browse();
        Category Get(int id);
        int Create(int id, Category category);
        bool Update(int id, Category category);

        bool Delete(int id);
        //bool SetParent(int parentId, int childId);
    }
}