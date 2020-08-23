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

        public FileData Create()
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

            return data;
        }

        [Fact]
        public void CheckElementCount()
        {
            _fileDataService.ClearFileSystem();

            var dataNum1 = _fileDataService.GetDataBaseFilesCount();
            var fileNum1 = _fileDataService.GetFileSystemFilesCount();

            Assert.Equal(dataNum1, fileNum1);

            var file = Create();

            var dataNum2 = _fileDataService.GetDataBaseFilesCount();
            var fileNum2 = _fileDataService.GetFileSystemFilesCount();

            Assert.Equal(dataNum1 + 1, dataNum2);
            Assert.Equal(fileNum1 + 1, fileNum2);
            Assert.Equal(dataNum2, fileNum2);

            _fileDataService.Delete(file.Id);

            var dataNum3 = _fileDataService.GetDataBaseFilesCount();
            var fileNum3 = _fileDataService.GetFileSystemFilesCount();

            Assert.Equal(dataNum3, dataNum1);
            Assert.Equal(fileNum3, fileNum1);
            Assert.Equal(dataNum3, fileNum3);
        }


        [Fact]
        public void CreateElement()
        {
            var data = Create();
            var fpath = _fileDataService.GetAbsolutePath(data);
            Assert.True(File.Exists(fpath));
            Assert.NotNull(_context.FileData.FirstOrDefault(f => f.Id == data.Id));
            _fileDataService.Delete(data.Id);
            Assert.False(File.Exists(fpath));
            Assert.Null(_context.FileData.FirstOrDefault(f => f.Id == data.Id));
        }
    }
}