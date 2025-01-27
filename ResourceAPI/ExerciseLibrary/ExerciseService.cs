﻿using System.Collections.Generic;
using System.Linq;
using FileDataLibrary;

namespace ExerciseLibrary
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseDbContext _context;
        private readonly IFileDataService _fileDataService;

        public ExerciseService(IExerciseDbContext context, IFileDataService fileDataService = null)
        {
            _context = context;
            _fileDataService = fileDataService;
        }

        public bool Delete(int exerciseId)
        {
            var exercise = _context.Exercises.FirstOrDefault(e => e.Id == exerciseId);
            if (exercise == null) return false;
            _context.Exercises.Remove(exercise);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteVariableData(int exerciseId, int scriptId)
        {
            var element = _context.ExerciseVariablesData.FirstOrDefault(s => s.Id == scriptId);
            if (element == null) return false;
            _context.ExerciseVariablesData.Remove(element);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<Exercise> Browse()
        {
            var exercises = _context.Exercises.ToList();
            return exercises;
        }

        public int Create(Exercise exercise, int categoryId, int authorId = 1)
        {
            var element = new Exercise
            {
                Name = exercise.Name,
                Content = exercise.Content,
                AuthorId = authorId,
                CategoryId = exercise.CategoryId
            };
            _context.Exercises.Add(element);
            _context.SaveChanges();

            foreach (var fileData in exercise.Files) _fileDataService.CreateForExercise(fileData, element.Id);

            return element.Id;
        }

        public Exercise Get(int exerciseId)
        {
            var exercise = _context.Exercises.Select(e => new Exercise
            {
                Id = e.Id,
                AuthorId = e.AuthorId,
                Content = e.Content,
                Name = e.Name
            }).FirstOrDefault(e => e.Id == exerciseId);

            if (exercise == null) return null;

            exercise.Files = _fileDataService.GetFilesForExercise(exerciseId);
            exercise.Render();
            return exercise;
        }

        public bool Edit(int exerciseId, Exercise exercise)
        {
            var element = _context.Exercises.FirstOrDefault(e => e.Id == exerciseId);
            if (element == null) return false;
            element.Name = exercise.Name;
            element.Content = exercise.Content;

            foreach (var file in element.Files) _fileDataService.DeleteForExercise(exercise.Id, file.FileName);

            _context.Exercises.Update(element);
            _context.SaveChanges();

            foreach (var file in exercise.Files) _fileDataService.CreateForExercise(file, exerciseId);

            return true;
        }

        public int CreateVariableData(int exerciseId, ExerciseVariableData exerciseVariableData)
        {
            var exercise = _context.Exercises.FirstOrDefault(e => e.Id == exerciseId);
            if (exercise == null) return 0;

            var element = new ExerciseVariableData
            {
                Name = exerciseVariableData.Name,
                Content = exerciseVariableData.Content,
                //FloatMin = exerciseVariableData.FloatMin,
                //FloatMax = exerciseVariableData.FloatMax,
                //IntMin = exerciseVariableData.IntMin,
                //IntMax = exerciseVariableData.IntMax,
                ExerciseId = exerciseVariableData.Id
            };

            _context.ExerciseVariablesData.Add(element);
            _context.SaveChanges();

            return element.Id;
        }

        public ExerciseVariableData GetVariableData(int exerciseId, int scriptId)
        {
            var script = _context.ExerciseVariablesData.FirstOrDefault(s => s.Id == scriptId);
            return script;
        }

        public bool EditVariableData(int exerciseId, int scriptId, ExerciseVariableData exerciseVariableData)
        {
            var element = _context.ExerciseVariablesData.FirstOrDefault(s => s.Id == scriptId);
            if (element == null) return false;
            element.Name = exerciseVariableData.Name;
            element.Content = exerciseVariableData.Content;
            _context.ExerciseVariablesData.Update(element);
            _context.SaveChanges();
            return true;
        }
    }
}