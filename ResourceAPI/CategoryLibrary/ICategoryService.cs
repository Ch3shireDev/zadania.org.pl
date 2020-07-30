namespace CategoryLibrary
{
    public interface ICategoryService
    {
        int Create(int parentId, Category category);
        bool Update(int parentId, Category category);

        bool Delete(int id);

        //bool SetParent(int parentId, int childId);
        Category GetProblems(int id);
        Category GetQuizTests(int cid1);
        Category GetExercises(int cid1);
    }
}