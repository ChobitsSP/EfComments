using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using DbUtils;

namespace EfComments.Utils
{
    public static class CodeUtils
    {
        static Regex reg_prop = new Regex(@"^public ([^ ]+) ([^ ]+) { get; set; }");

        public class LineInfo
        {
            public LineType type { get; set; } = LineType.other;
            public string source { get; set; }
            public string prop_name { get; set; }
        }

        public class CommentInfo
        {
            public int index { get; set; }
            public string[] lines { get; set; }
        }

        public enum LineType
        {
            other = 0,
            prop = 1,
            attr = 2,
            // comment = 3,
        }

        public static IEnumerable<LineInfo> GetLines(string filePath)
        {
            using (var sr = new StreamReader(filePath, Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    var info = new LineInfo();

                    info.source = sr.ReadLine();

                    var trim_str = info.source.Trim();

                    if (trim_str.StartsWith("["))
                    {
                        info.type = LineType.attr;
                    }
                    else if (trim_str.StartsWith("//"))
                    {
                        continue;
                        // info.type = LineType.comment;
                    }
                    else
                    {
                        var pm = reg_prop.Match(trim_str);

                        if (pm.Success)
                        {
                            info.type = LineType.prop;
                            info.prop_name = pm.Groups[2].Value;
                        }
                    }

                    yield return info;
                }
            }
        }

        public static void AddComments(IDbUtils db, string filePath)
        {
            var tableName = Path.GetFileNameWithoutExtension(filePath);
            var cols = db.GetColumns(tableName).ToArray();
            AddComments(filePath, cols);
        }

        public static void AddComments(string filePath, TableColumn[] cols)
        {
            var lines = GetLines(filePath).ToList();

            var clist = new List<CommentInfo>();

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line.type == LineType.prop)
                {
                    var col = cols.Where(t => t.name == line.prop_name).FirstOrDefault();

                    if (col != null && !string.IsNullOrEmpty(col.comments))
                    {
                        var kong = Regex.Match(line.source, @"^ +").Value;

                        clist.Add(new CommentInfo()
                        {
                            index = GetInsertIndex(lines, i),
                            lines = GetComments(col.comments, kong).ToArray(),
                        });
                    }
                }
            }

            for (var i = clist.Count - 1; i >= 0; i--)
            {
                var citem = clist[i];

                lines.InsertRange(citem.index, citem.lines.Select(t => new LineInfo()
                {
                    source = t,
                    type = LineType.other,
                }));
            }

            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                sb.AppendLine(line.source);
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        static int GetInsertIndex(List<LineInfo> list, int index)
        {
            LineInfo info;

            do
            {
                info = list[--index];
            } while (info.type == LineType.attr);

            return index + 1;
        }

        private static Regex r3 = new Regex("(?:\\r\\n|\\r|\\n)");

        private static IEnumerable<string> GetComments(string str, string kong)
        {
            yield return kong + "/// <summary>";
            string[] strArray = r3.Split(str);
            for (int index = 0; index < strArray.Length; ++index)
                yield return kong + "/// " + strArray[index];
            strArray = null;
            yield return kong + "/// </summary>";
        }
    }
}
