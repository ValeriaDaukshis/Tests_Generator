using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GeneratorLib
{
    public class FileWriter
    {
        public async void Write(string fileName, IEnumerable<GeneratedModel> generatedClasses)
        {
            foreach (var generated in generatedClasses)
            {
                var name = $"{fileName}_{generated.Name}";
                using (var writer = new StreamWriter(new FileStream(name, FileMode.OpenOrCreate)))
                {
                    await writer.WriteAsync(generated.Content);
                }
            }
        }
    }
}