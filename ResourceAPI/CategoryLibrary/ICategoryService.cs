using System.Collections.Generic;
using ExerciseLibrary;
using ProblemLibrary;
using QuizLibrary;

namespace CategoryLibrary
{
    public interface ICategoryService
    {
        Category Create(Category category, int parentId = 1, int authorId = 1);
        Category Update(Category category, int categoryId);

        bool Delete(int id);

        //bool SetParent(int parentId, int childId);
        IEnumerable<ProblemLink> GetProblems(int id);
        IEnumerable<QuizLink> GetQuizzes(int cid1);
        IEnumerable<ExerciseLink> GetExercises(int cid1);

        Category GetCategory(int id);
        //IEnumerable<CategoryLink> GetCategories(int id);
    }
}