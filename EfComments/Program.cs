using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfComments.Utils;

namespace EfComments
{
    class Program
    {
        static readonly ConnectionStringSettings config = ConfigurationManager.ConnectionStrings["ConnectionString"];

        static void Main(string[] args)
        {
            Test1();
        }

        static void Test1()
        {
            Console.WriteLine("input folder");
            var dir = Console.ReadLine();
            var files = Directory.GetFiles(dir, "*.cs", SearchOption.TopDirectoryOnly);
            var dbUtils = DbUtils.DbHelper.GetUtils(config);

            Parallel.ForEach(files, file =>
            {
                CodeUtils.AddComments(dbUtils, file);
            });

            Console.WriteLine("success!");
            Console.ReadLine();
        }
    }
}
