using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace unit_testing.Chapter6.Listing7_.Before1
{
    /*
    예제 6.8 감사 시스템의 초기 구현
        - 작업 디렉터리에서 전체 파일 목록을 검색한다. 
        - 인덱스별로 정렬한다
        - 아직 감사 파일이 없으면 단일 레코드로 첫 번째 파일을 생성한다
        - 감사 파일이 있으면 최신 파일을 가져와서 
            파일의 항목 수가 한계에 도달했는지에 따라 새 레코드를 추가하거나 새 파일을 생성한다
    */
    public class AuditManager
    {
        private readonly int _maxEntriesPerFile;
        private readonly string _directoryName;

        public AuditManager(int maxEntriesPerFile, string directoryName)
        {
            _maxEntriesPerFile = maxEntriesPerFile;
            _directoryName = directoryName;
        }

        // 파일 시스템과 강결합💩
        public void AddRecord(string visitorName, DateTime timeOfVisit)
        {
            string[] filePaths = Directory.GetFiles(_directoryName); // 💩
            (int index, string path)[] sorted = SortByIndex(filePaths);

            string newRecord = visitorName + ';' + timeOfVisit.ToString("s");

            if (sorted.Length == 0)
            {
                string newFile = Path.Combine(_directoryName, "audit_1.txt");
                File.WriteAllText(newFile, newRecord); // 💩
                return;
            }

            (int currentFileIndex, string currentFilePath) = sorted.Last();
            List<string> lines = File.ReadAllLines(currentFilePath).ToList(); // 💩 
            if (lines.Count < _maxEntriesPerFile)
            {
                lines.Add(newRecord);
                string newContent = string.Join("\r\n", lines);
                File.WriteAllText(currentFilePath, newContent); // 💩
            }
            else
            {
                int newIndex = currentFileIndex + 1;
                string newName = $"audit_{newIndex}.txt";
                string newFile = Path.Combine(_directoryName, newName);
                File.WriteAllText(newFile, newRecord); // 💩
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
}