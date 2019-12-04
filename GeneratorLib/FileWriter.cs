using System.IO;

namespace GeneratorLib
{
    public class FileWriter
    {
        public static async void Write(string fileName, string text)
        {
            using (var writer = new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate)))
            {
                await writer.WriteAsync(text);
            }
            
        }
    }
}