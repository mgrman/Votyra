using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Ionic.Zip;
using System.Text;
using System.IO;

public static class UnityZip
{
    public static void Unzip(string zipFilePath, string location)
    {
        Directory.CreateDirectory(location);

        using (ZipFile zip = ZipFile.Read(zipFilePath))
        {
            zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
        }
    }

    public static void Zip(string zipFileName, string directory)
    {
        string path = Path.GetDirectoryName(zipFileName);
        Directory.CreateDirectory(path);

        var directoryInfo = new DirectoryInfo(directory);
        var files = directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories);
        using (ZipFile zip = new ZipFile())
        {
            foreach (var file in files)
            {
                var relativePath = zip.AddFile(file.FullName, file.DirectoryName.Substring(directoryInfo.FullName.Length));
            }
            zip.Save(zipFileName);
        }
    }
}