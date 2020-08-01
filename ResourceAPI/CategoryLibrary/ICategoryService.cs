namespace CategoryLibrary
{
    public interface ICategoryService
    {
        Category Create(Category category, int parentId = 1, int authorId = 1);
        Category Update(Category category, int categoryId);

        bool Delete(int id);

        //bool SetParent(int parentId, int childId);
        Category GetProblems(int id);
        Category GetQuizTests(int cid1);
        Category GetExercises(int cid1);
    }
}