using Newtonsoft.Json;
using PhotoOrganizer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


public class Program
{
    public static void Main()
    {
        // Set the working directory
        var workingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        Directory.SetCurrentDirectory(workingDirectory ?? throw new InvalidOperationException());

        // create the full path of the JSON file
        var inputJsonPath = Path.Combine(workingDirectory, "Input.json");

        // Read and parse the JSON file
        InputParameters? defaultParameters = null;
        if (File.Exists(inputJsonPath))
        {
            try
            {
                defaultParameters = JsonConvert.DeserializeObject<InputParameters>(File.ReadAllText(inputJsonPath));
            }
            catch
            {
                Console.WriteLine("Error reading from Input.json, please ensure the file is correctly formatted!!!");
                return;
            }
        }

        Console.WriteLine("Enter the number of images to download (press Enter for default):");
        var countInput = Console.ReadLine();
        int count = string.IsNullOrEmpty(countInput) ? defaultParameters?.Count ?? 0 : int.Parse(countInput);

        Console.WriteLine("Enter the maximum parallel download limit (press Enter for default):");
        var parallelismInput = Console.ReadLine();
        int parallelism = string.IsNullOrEmpty(parallelismInput) ? defaultParameters?.Parallelism ?? 0 : int.Parse(parallelismInput);

        Console.WriteLine("Enter the save path (press Enter for default):");
        var savePath = Console.ReadLine();
        if (string.IsNullOrEmpty(savePath))
        {
            savePath = defaultParameters?.SavePath ?? "./outputs";
        }

        DownloadManager manager = new(count, parallelism, savePath);
        manager.ProgressEvent += (progress) =>
        {
            ConsoleHelper.WriteProgress($"Downloading {count} images ({parallelism} parallel downloads at most)\nProgress: {progress}/{count}", progress == 1);
        };

        manager.DownloadImages().GetAwaiter().GetResult();
    }
}

public class InputParameters
{
    public int Count { get; set; }
    public int Parallelism { get; set; }
    public string? SavePath { get; set; }
}

