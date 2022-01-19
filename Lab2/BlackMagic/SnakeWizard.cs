using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Lab2.Automata;
using Newtonsoft.Json;

namespace Lab2.BlackMagic
{
    public class SnakeWizard
    {
        private const string PathToWizard = @"C:\ProgramData\Anaconda3\envs\BlackMagic\python.exe";
        private const string PathToMagicScript = @"D:\dev\c#\TALC\Lab2\BlackMagic\magic.py";
        private const string SerializationPath = @"D:\dev\c#\TALC\Lab2\BlackMagic\states.txt";
        private const string GraphPath = @"D:\dev\c#\TALC\Lab2\BlackMagic\graph.png";

        public static async Task<BitmapImage> CastMagic(List<State> states)
        {
            var serialized = JsonConvert.SerializeObject(states);
            
            Console.WriteLine(serialized);

            await using (var stream = File.Create(SerializationPath))
            {
                stream.Write(Encoding.ASCII.GetBytes(serialized));
            }
            
            var processStartInfo = new ProcessStartInfo
            {
                FileName = PathToWizard,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Arguments = $"{PathToMagicScript}"
            };

            using (var process = Process.Start(processStartInfo))
            {
                using (var reader = process.StandardOutput)
                {
                    string stderr = await process.StandardError.ReadToEndAsync();
                    string result = await reader.ReadToEndAsync();

                    while (!process.HasExited)
                    {
                    }
                    
                    
                }
            }

            BitmapImage resultImage = null;
            
            using (var stream = File.OpenRead(GraphPath))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                resultImage = image;
            }

            return resultImage;
        }
    }
}