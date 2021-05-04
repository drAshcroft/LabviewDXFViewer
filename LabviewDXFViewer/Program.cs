using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabviewDXFViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            var refFiles = Directory.GetFiles(@"C:\Users\bashc\Downloads\sorted\content\sorted2\different", "*.*", SearchOption.AllDirectories);
            var filenames = new Dictionary<string, string>();
            var repeated = new List<string>();


            foreach (var file in refFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(file).ToLower();
                filename = filename.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (filenames.ContainsKey(filename))
                {
                    repeated.Add(filename);
                    File.Delete(file);
                }
                else
                    filenames.Add(filename, file);
               
            }

            foreach (var file in refFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(file).ToLower();
                filename = filename.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (repeated.Contains(filename) == false)
                    File.Delete(file);

            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
