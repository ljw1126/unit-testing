using Moq;

namespace unit_testing.Chapter6.Listing7_.Mocks2
{
    // 6.11 목을 이용한 감사 시스템의 동작 확인
    public class Tests
    {
        // sorted.Length == 0 조건문에 걸림
        [Fact]
        public void A_new_file_is_created_for_the_first_entry()
        {
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.GetFiles("audits"))
                .Returns([]); // 빈 배열
            var sut = new AuditManager(3, "audits", fileSystemMock.Object);

            sut.AddRecord("Peter", DateTime.Parse("2019-04-09T13:00:00"));

            fileSystemMock.Verify(x => x.WriteAllText(
                "audits/audit_1.txt",
                "Peter;2019-04-09T13:00:00"));
        }

        // else 분기에 걸림
        [Fact]
        public void A_new_file_is_created_when_the_current_file_overflows()
        {
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock
                .Setup(x => x.GetFiles("audits"))
                .Returns(new string[]
                {
                    "audits/audit_1.txt",
                    "audits/audit_2.txt"
                });
            fileSystemMock
                .Setup(x => x.ReadAllLines("audits/audit_2.txt"))
                .Returns(new List<string>
                {
                    "Peter; 2019-04-06T16:30:00",
                    "Jane; 2019-04-06T16:40:00",
                    "Jack; 2019-04-06T17:00:00"
                });
            var sut = new AuditManager(3, "audits", fileSystemMock.Object);

            sut.AddRecord("Alice", DateTime.Parse("2019-04-06T18:00:00"));

            fileSystemMock.Verify(x => x.WriteAllText(
                "audits/audit_3.txt",
                "Alice;2019-04-06T18:00:00"));
        }
    }


    // 6.9 생성자를 통한 파일 시스템의 명시적 주입 
    // 6.10 새로운 IFileSystem 인터페이스 사용
    public class AuditManager
    {
        private readonly int _maxEntriesPerFile;
        private readonly string _directoryName;
        private readonly IFileSystem _fileSystem; // 인터페이스, 생성자 주입

        public AuditManager(
            int maxEntriesPerFile,
            string directoryName,
            IFileSystem fileSystem)
        {
            _maxEntriesPerFile = maxEntriesPerFile;
            _directoryName = directoryName;
            _fileSystem = fileSystem;
        }

        public void AddRecord(string visitorName, DateTime timeOfVisit)
        {
            string[] filePaths = _fileSystem.GetFiles(_directoryName); // ✅
            (int index, string path)[] sorted = SortByIndex(filePaths);

            string newRecord = visitorName + ';' + timeOfVisit.ToString("s");

            if (sorted.Length == 0)
            {
                string newFile = Path.Combine(_directoryName, "audit_1.txt");
                _fileSystem.WriteAllText(newFile, newRecord); // ✅
                return;
            }

            (int currentFileIndex, string currentFilePath) = sorted.Last();
            List<string> lines = _fileSystem.ReadAllLines(currentFilePath).ToList(); // ✅
            if (lines.Count < _maxEntriesPerFile)
            {
                lines.Add(newRecord);
                string newContent = string.Join("\r\n", lines);
                _fileSystem.WriteAllText(currentFilePath, newContent); // ✅
            }
            else
            {
                int newIndex = currentFileIndex + 1;
                string newName = $"audit_{newIndex}.txt";
                string newFile = Path.Combine(_directoryName, newName);
                _fileSystem.WriteAllText(newFile, newRecord); // ✅
            }
        }

        private (int index, string path)[] SortByIndex(string[] files)
        {
            return files
                .Select(path => (index: GetIndex(path), path))
                .OrderBy(x => x.index)
                .ToArray();
        }

        private int GetIndex(string filePath)
        {
            // File name example: audit_1.txt
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            return int.Parse(fileName.Split('_')[1]);
        }
    }

    public interface IFileSystem
    {
        string[] GetFiles(string directoryName);
        void WriteAllText(string filePath, string content);
        List<string> ReadAllLines(string filePath);
    }
}