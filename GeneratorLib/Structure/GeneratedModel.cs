namespace GeneratorLib
{
    public class GeneratedModel
    {
        public string Name { get; }
        public string Content { get; }

        public GeneratedModel(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}