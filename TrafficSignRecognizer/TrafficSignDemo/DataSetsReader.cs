﻿using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignDemo
{
    public static class DataSetsReader
    {
        public static IEnumerable<TrafficSignInfo> Read(string relativePath, IHostingEnvironment env)
        {
            var directories = Directory.EnumerateDirectories($"{env.WebRootPath}{relativePath}");

            foreach(var directory in directories)
            {
                var label = int.Parse(directory.Substring(directory.LastIndexOf('\\') + 1));

                foreach (var file in Directory.EnumerateFiles(directory, "*.png"))
                {
                    var sign = new TrafficSignInfo
                    {
                        Label = label,
                        ImgUrl = $"{relativePath}/{directory.Substring(directory.LastIndexOf('\\') + 1)}/{Path.GetFileName(file)}"
                    };

                    yield return sign;
                }
            }
        }
    }
}
