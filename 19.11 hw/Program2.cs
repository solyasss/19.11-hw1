using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace hw_19_11_hw
{
    class search_2
    {
        static void Main2()
        {
            Console.Write("Enter the mask for files and directories: ");
            string mask = Console.ReadLine();
            
            string mask_pattern = MaskToRegex(mask);
            Regex regular_mask = new Regex(mask_pattern, RegexOptions.IgnoreCase);
            
            List<string> found_items = new List<string>();
            
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    DirectoryInfo root_dir = drive.RootDirectory;
                    try
                    {
                        find(root_dir, regular_mask, found_items);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error {0}: {1}", drive.Name, ex.Message);
                    }
                }
            }
            
            for (int i = 0; i < found_items.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, found_items[i]);
            }

            if (found_items.Count == 0)
            {
                Console.WriteLine("No items found");
                return;
            }
            
            Console.WriteLine("Choose an action:");
            Console.WriteLine("1. Delete all found items");
            Console.WriteLine("2. Delete a specified item");
            Console.WriteLine("3. Delete a range of items");
            Console.Write("Enter the action number: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    delete(found_items);
                    break;
                case "2":
                    delete_one(found_items);
                    break;
                case "3":
                    delete_range(found_items);
                    break;
                default:
                    Console.WriteLine("Invalid selection");
                    break;
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
        
        static void find(DirectoryInfo di, Regex reg_mask, List<string> found_items)
        {
            FileInfo[] files = null;
            try
            {
                files = di.GetFiles();
            }
            catch
            {
                return;
            }

            foreach (FileInfo file in files)
            {
                if (reg_mask.IsMatch(file.Name))
                {
                    found_items.Add(file.FullName);
                }
            }
            
            DirectoryInfo[] dirs = null;
            try
            {
                dirs = di.GetDirectories();
            }
            catch
            {
                return;
            }

            foreach (DirectoryInfo dir in dirs)
            {
                if (reg_mask.IsMatch(dir.Name))
                {
                    found_items.Add(dir.FullName);
                }
                find(dir, reg_mask, found_items);
            }
        }
        
        static void delete(List<string> items)
        {
            foreach (string item in items)
            {
                delete_item(item);
            }
            Console.WriteLine("All items have been deleted");
        }
        
        static void delete_one(List<string> items)
        {
            Console.Write("Enter the number of the item to delete: ");
            int index;
            if (int.TryParse(Console.ReadLine(), out index) && index > 0 && index <= items.Count)
            {
                delete_item(items[index - 1]);
                Console.WriteLine("Item deleted");
            }
            else
            {
                Console.WriteLine("Invalid item");
            }
        }
        
        static void delete_range(List<string> items)
        {
            Console.Write("Enter the start number of the range: ");
            int start_index;
            if (!int.TryParse(Console.ReadLine(), out start_index) || start_index < 1 || start_index > items.Count)
            {
                Console.WriteLine("Invalid start number");
                return;
            }

            Console.Write("Enter the end number of the range: ");
            int end_index;
            if (!int.TryParse(Console.ReadLine(), out end_index) || end_index < start_index || end_index > items.Count)
            {
                Console.WriteLine("Invalid end number");
                return;
            }

            for (int i = start_index - 1; i < end_index; i++)
            {
                delete_item(items[i]);
            }
            Console.WriteLine("Range of items deleted");
        }
        
        static void delete_item(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error{0}: {1}", path, ex.Message);
            }
        }
    }
}
