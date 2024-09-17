using System.IO;
using System.Threading.Tasks;
class file_sync {

    static async Task Main(string[] args) {

        if (args.Length != 4) { 

            Console.WriteLine("We need a source folder path, a replica folder path, the interval in seconds and a log file path");
            return;
        }


        string source_folder_path = args[0];
        string replica_folder_path = args[1];
        int sync_interval;
        string log_file_path = args[3];

        if (!int.TryParse(args[2], out sync_interval)) {  // tried convert.toint32() function but errors

            Console.WriteLine("the interval must be a valid intenger"); 
            return;

        }

        Console.WriteLine("Source Folder: " + source_folder_path);
        Console.WriteLine("Replica Folder: " + replica_folder_path);
        Console.WriteLine("Sync Interval: " + sync_interval + " seconds");
        Console.WriteLine("Log File: " + log_file_path);

        while (true) {

            SyncFolders(source_folder_path, replica_folder_path, log_file_path);
            await Task.Delay(sync_interval * 1000);
        }

    }

    static void SyncFolders(string source_folder, string replica_folder, string log_file) {
    
        try {

            using (StreamWriter log = new StreamWriter(log_file, true)) { 
            
                foreach (var source_file_path in Directory.GetFiles(source_folder, "*", SearchOption.AllDirectories)) {

                    string relative_path = Path.GetRelativePath(source_folder, source_file_path); // maintains same directory structuture 
                    string replica_file_path = Path.Combine(replica_folder, relative_path);

                    Directory.CreateDirectory(Path.GetDirectoryName(replica_file_path)); // create directories if they don't exist

                    if (!File.Exists(replica_file_path) || File.GetLastWriteTime(source_file_path) > File.GetLastWriteTime(replica_file_path)) {
                
                        File.Copy(source_file_path, replica_file_path, true);

                        // creation handled (same execution)
                        log.WriteLine($"Copied: {source_file_path} to {replica_file_path}"); // records the copy operation made
                        Console.WriteLine($"Copied: {source_file_path} to {replica_file_path}");

                    }

                }

                foreach (var replica_file_path in Directory.GetFiles(replica_folder, "*", SearchOption.AllDirectories)) {

                    string relative_path = Path.GetRelativePath(replica_folder, replica_file_path);
                    string source_file_path = Path.Combine(source_folder, relative_path);

                    if (!File.Exists(source_file_path)) {

                        File.Delete(replica_file_path);

                        // Log the removal of the file
                        log.WriteLine($"Removed: {replica_file_path}");
                        Console.WriteLine($"Removed: {replica_file_path}");

                    }

                }


            }

        }

        catch (Exception ex) { //logs an error with specific type in log_file.txt

            Console.WriteLine($"Error: {ex.Message}");

            using (StreamWriter log = new StreamWriter(log_file, true)) {

            log.WriteLine($"Error: {ex.Message} - StackTrace: {ex.StackTrace}");

            }

        }

    }

}

