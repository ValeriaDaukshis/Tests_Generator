﻿namespace GeneratorLib
{
    public class FileWithContent
    {
        public string Path { get; }
        
        public string Content { get; }

        public FileWithContent(string path, string content)
        {
            Path = path;
            Content = content;
        }
    }
}