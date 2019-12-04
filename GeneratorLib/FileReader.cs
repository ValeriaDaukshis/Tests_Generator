using System.IO;
using System.Threading.Tasks;

namespace GeneratorLib
{
    public class FileReader
    {
        public static async Task<string> Read(string fileName)
        {
            using (var reader = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}