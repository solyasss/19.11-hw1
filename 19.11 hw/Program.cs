using System;
using System.IO;
using System.Text.RegularExpressions;

namespace hw_19_11_hw
{
    class search_1
    {
        static void Main()
        {
            Console.Write("Enter the directory path: ");
            string path = Console.ReadLine();
            
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                Console.WriteLine("Directory doesn't exist");
                return;
            }

            Console.Write("Enter the file mask: ");
            string mask = Console.ReadLine();

            Console.Write("Enter the start date: ");
            DateTime start_date;
            if (!DateTime.TryParse(Console.ReadLine(), out start_date))
            {
                Console.WriteLine("Invalid format");
                return;
            }

            Console.Write("Enter the end date: ");
            DateTime end_date;
            if (!DateTime.TryParse(Console.ReadLine(), out end_date))
            {
                Console.WriteLine("Invalid format");
                return;
            }
            
            string mask_pattern = MaskToRegex(mask);
            Regex regular_mask = new Regex(mask_pattern, RegexOptions.IgnoreCase);

            try
            {
                ulong count = files_by_date(dir, regular_mask, start_date, end_date);
                Console.WriteLine("Number of files found: {0}", count);
                Console.WriteLine("Results saved to report.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        static string MaskToRegex(string mask)
        {
            mask = mask.Replace(".", @"\.");
            mask = mask.Replace("?", ".");
            mask = mask.Replace("*", ".*");
            mask = "^" + mask + "$";
            return mask;
        }
        
        static ulong files_by_date(DirectoryInfo di, Regex reg_mask, DateTime start_date, DateTime end_date)
        {
            ulong count = 0;

            FileInfo[] files = null;
            try
            {
                files = di.GetFiles();
            }
            catch
            {
                return count;
            }

            using (StreamWriter sw = new StreamWriter("report.txt", true))
            {
                foreach (FileInfo file in files)
                {
                    if (reg_mask.IsMatch(file.Name))
                    {
                        DateTime last_write_time = file.LastWriteTime;
                        if (last_write_time >= start_date && last_write_time <= end_date)
                        {
                            count++;
                            sw.WriteLine(file.FullName);
                            Console.WriteLine("File: " + file.FullName);
                        }
                    }
                }
            }
            
            DirectoryInfo[] sub_dirs = di.GetDirectories();
            foreach (DirectoryInfo sub_dir in sub_dirs)
            {
                count += files_by_date(sub_dir, reg_mask, start_date, end_date);
            }

            return count;
        }
    }
}
