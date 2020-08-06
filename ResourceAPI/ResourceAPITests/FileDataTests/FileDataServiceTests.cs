﻿using System;
using System.Text;
using AutoMapper;
using FileDataLibrary;
using Microsoft.EntityFrameworkCore;
using ResourceAPI;
using Xunit;

namespace ResourceAPITests.FileDataTests
{
    public class FileDataServiceTests
    {
        public FileDataServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());

            var conf = new MapperConfiguration(c => { });

            _context = new SqlContext(optionsBuilder.Options);
            _categoryService = new FileDataService(_context);
        }

        private readonly IFileDataDbContext _context;
        private readonly IFileDataService _categoryService;

        [Fact]
        public void Test()
        {
            var element = new FileDataView
            {
                FileName = "abc.png",
                FileBytes = Encoding.UTF8.GetBytes("abc")
            };

            var data = _categoryService.Create(element);

            var file = _categoryService.Get(data.Id);

            var str = Encoding.UTF8.GetString(file.FileBytes);

            Assert.Equal("abc", str);
        }
    }
}