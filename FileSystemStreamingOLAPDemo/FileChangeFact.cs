using System;
using System.IO;

namespace RXDemo
{
    public class FileChangeFact
    {
        public WatcherChangeTypes ChangeType { get; set; }
        public long Length { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public bool IsContainer { get; set; }
    }
}
