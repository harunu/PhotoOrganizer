using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer.Helpers
{
    public class DownloadManager
    {
        private readonly int _totalImages;
        private readonly int _parallelism;
        private readonly string _savePath;
        private readonly CancellationTokenSource _cts;
        private readonly object _lock = new();
        private int _progress;
        public event Action<int> ProgressEvent;


        public DownloadManager(int totalImages, int parallelism, string savePath)
        {
            _totalImages = totalImages;
            _parallelism = parallelism;
            _savePath = savePath;
            _cts = new CancellationTokenSource();

            // CTRL+C cancel operation
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine(); // New line
                Console.WriteLine("Cancelling download ....");
                _cts.Cancel();
                Cleanup();
                Environment.Exit(0); // Ensure that application exits
            };
        }
        public async Task DownloadImages()
        {
            // Cleanup the folder before starting the download
            Cleanup();

            Directory.CreateDirectory(_savePath);

            using (HttpClient client = new HttpClient())
            {
                List<Task> tasks = new();
                for (int i = 0; i < _totalImages; i++)
                {
                    _cts.Token.ThrowIfCancellationRequested();
                    int current = i;
                    tasks.Add(DownloadImage(client, current));

                    if (tasks.Count >= _parallelism || i == _totalImages - 1)
                    {
                        var completed = await Task.WhenAny(tasks);
                        tasks.Remove(completed);
                    }
                }
                await Task.WhenAll(tasks);
            }
        }

        private async Task DownloadImage(HttpClient client, int current)
        {
            try
            {
                string localFile = Path.Combine(_savePath, $"{current + 1}.png");
                string url = $"https://picsum.photos/{new Random().Next(100, 500)}/{new Random().Next(100, 500)}";
                var response = await client.GetAsync(url, _cts.Token);
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(localFile, fileBytes, _cts.Token);

                lock (_lock)
                {
                    _progress++;
                    ProgressEvent?.Invoke(_progress);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Downloading image {current}: {ex.Message}");
            }
        }

        public void Cleanup()
        {
            try
            {
                // Check if directory exists
                if (!Directory.Exists(_savePath))
                {
                    return; // If the directory does not exist, nothing to clean up
                }

                DirectoryInfo di = new DirectoryInfo(_savePath);

                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.WriteError($"Error deleting file {file.FullName}: {ex.Message}");
                    }
                }

                Directory.Delete(_savePath);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error cleaning up directory {_savePath}: {ex.Message}");
            }
        }

    }
}
