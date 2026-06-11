namespace DeleteLogFiles;

public sealed class CleanupResult
{
    public int DirectoriesChecked { get; set; }

    public int FilesChecked { get; set; }

    public int FilesDeleted { get; set; }

    public int FilesWouldDelete { get; set; }

    public int FilesSkipped { get; set; }

    public long BytesFreed { get; set; }

    public long BytesWouldFree { get; set; }
}
