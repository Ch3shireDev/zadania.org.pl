using System;
using System.IO;
using System.Linq;
using System.Text;
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

            _context = new SqlContext(optionsBuilder.Options);
            _fileDataService = new FileDataService(_context);
        }

        private readonly IFileDataDbContext _context;
        private readonly IFileDataService _fileDataService;

        [Fact]
        public void CreateElement()
        {
            var element = new FileDataView
            {
                FileName = "abc.png",
                FileBytes = Encoding.UTF8.GetBytes("abc")
            };

            var data = _fileDataService.CreateFile(element);
            var file = _fileDataService.Get(data.Id);
            var str = Encoding.UTF8.GetString(file.FileBytes);
            Assert.Equal("abc", str);

            var fpath = _fileDataService.GetAbsolutePath(data);
            Assert.True(File.Exists(fpath));
            Assert.NotNull(_context.FileData.FirstOrDefault(f => f.Id == data.Id));
            _fileDataService.Delete(data.Id);
            Assert.False(File.Exists(fpath));
            Assert.Null(_context.FileData.FirstOrDefault(f => f.Id == data.Id));
        }
    }
}