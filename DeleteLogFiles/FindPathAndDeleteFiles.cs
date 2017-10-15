using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DeleteLogFiles
{
    class FindPathAndDeleteFiles
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        List<string> allDirectories = new List<string>();

        public void GetPaths()
        {
            try
            {
                int.TryParse(ConfigurationManager.AppSettings["deleteafterdays"], out int deleteafterdays);
                bool.TryParse(ConfigurationManager.AppSettings["deletesubfolder"], out bool deletesubfolder);

                var appPath = Path.GetDirectoryName(Application.ExecutablePath);

                if (!File.Exists(appPath + "\\extensions.txt") && !File.Exists(appPath + "\\directories.txt"))
                {
                    log.Error("Config file is missing or no permission");
                }
                else
                {
                    List<string> extensions = new List<string>();
                    List<string> directories = new List<string>();

                    StreamReader fileextensions = new StreamReader(appPath + "\\extensions.txt");
                    StreamReader directoryfile = new StreamReader(appPath + "\\directories.txt");
                    string ext = "";
                    string path = "";
                    long filesize = 0;

                    while ((ext = fileextensions.ReadLine()) != null)
                    {
                        if (!ext.StartsWith("//"))
                        {
                            if (!ext.StartsWith("."))
                            {
                                ext = "." + ext;
                            }
                            extensions.Add(ext.ToUpper());
                            log.Debug("Extension: " + ext.ToUpper());
                        }
                    }
                    while ((path = directoryfile.ReadLine()) != null)
                    {
                        if (!path.StartsWith("//"))
                        {
                            directories.Add(path);
                            log.Debug("Directory: " + path);
                        }
                    }

                    foreach (var directory in directories)
                    {
                        try
                        {
                            DirectoryInfo ParentDirectory = new DirectoryInfo(directory);

                            log.Debug("Aktuelles ParentDirectory: " + ParentDirectory.FullName);

                            foreach (FileInfo f in ParentDirectory.GetFiles())
                            {
                                log.Debug("Datei in Verzeichnis: " + f.Name + " letzter Zugriff: " + f.LastWriteTime + " Dateierweiterung: " + f.Extension);
                                if (f.LastWriteTime <= DateTime.Now.AddDays(-deleteafterdays))
                                {
                                    foreach (var extension in extensions)
                                    {
                                        if (extension == f.Extension.ToUpper())
                                        {
                                            filesize = filesize + f.Length;
                                            log.Info(DateTime.Now.ToShortDateString() + " " + f.FullName + " wurde gelöscht");

                                            try
                                            {
                                                f.Delete();
                                            }
                                            catch (Exception ex)
                                            {
                                                log.Error("ERROR: " + ex);
                                            }
                                        }
                                    }
                                }
                            }
                            if (deletesubfolder)
                            {
                                GetSubFolders(ParentDirectory);

                                foreach (var onedir in allDirectories)
                                {
                                    DirectoryInfo subdir = new DirectoryInfo(onedir);

                                    log.Debug("Aktuelles SubDirectory: " + subdir.FullName);

                                    foreach (FileInfo f in subdir.GetFiles())
                                    {
                                        log.Debug("Datei in Verzeichnis: " + f.Name + " letzter Zugriff: " + f.LastWriteTime + " Dateierweiterung: " + f.Extension);
                                        if (f.LastWriteTime <= DateTime.Now.AddDays(-deleteafterdays))
                                        {
                                            foreach (var extension in extensions)
                                            {
                                                if (extension == f.Extension.ToUpper())
                                                {
                                                    filesize = filesize + f.Length;
                                                    log.Info(DateTime.Now.ToShortDateString() + " " + f.FullName + " wurde gelöscht");

                                                    try
                                                    {
                                                        f.Delete();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        log.Error("ERROR: " + ex);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("ERROR: --> Check Paths: " + ex);
                        }
                    }
                    filesize = (filesize / 1024) / 1024;
                    log.Info("Bei diesem Lauf wurden " + filesize.ToString("0.###") + " MB bereiningt");
                    directoryfile.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("ERROR: --> GetPaths: " + ex);
            }
        }
        private void GetSubFolders(DirectoryInfo ParentDirectory)
        {
            foreach (DirectoryInfo d in ParentDirectory.GetDirectories())
            {
                if (d != null)
                {
                    allDirectories.Add(d.FullName);
                    GetSubFolders(d);
                }
            }
        }
    }
}
