using Newtonsoft.Json;
using System.Xml.Linq;
using static LMSAPI.DTO.LessonConverter;

namespace LMSAPI.DTO
{
    public class LessonConverter
    {
        public class Subject
        {
            public string? label { get; set; }
            public Lesson[]? Lessons { get; set; }
        }

        public class Lesson
        {
            public string? label { get; set; }
            public Chapter[]? Chapters { get; set; }
        }

        public class Chapter
        {
            public string? label { get; set; }
            public string? url { get; set; }
        }

        public static async Task<Subject?> GetLessonConverterAsync(string SubjectSyllabusPath)
        {
            if (string.IsNullOrEmpty(SubjectSyllabusPath))
                return null;

            try
            {
                using var client = new HttpClient();

                string xml = await client.GetStringAsync(SubjectSyllabusPath);
                var xDoc = XDocument.Parse(xml);
                var subjectElement = xDoc.Root;

                if (subjectElement == null)
                    return null;

                var subject = new Subject
                {
                    label = subjectElement.Attribute("label")?.Value,
                    Lessons = subjectElement.Elements("Lessons").Select(l => new Lesson{
                label = l.Attribute("label")?.Value,
                 Chapters = l.Elements("Chapters")
                    .Select(c => new Chapter
                    {
                        label = c.Attribute("label")?.Value,
                        url = c.Attribute("url")?.Value
                    })
                    .ToArray()
            })
            .ToArray()
                };

                return subject;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error converting XML: {ex.Message}");
                return null;
            }
        }
    }
}
