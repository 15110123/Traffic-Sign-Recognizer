﻿using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Models.Utils
{
    public static class BitmapUtils
    {
        //Return matrix of pixel for grayscaled bitmap
        public static Matrix<int> AsMatrix(this Bitmap bitmap, int dividend = 1)
        {
            var pixelRows = System.Buffers.ArrayPool<IEnumerable<int>>.Shared.Rent(bitmap.Height);
            int[] pixelCols = null;

            for (var i = 0; i < bitmap.Height; i++)
            {
                pixelCols = System.Buffers.ArrayPool<int>.Shared.Rent(bitmap.Width);
                for (var j = 0; j < bitmap.Width; j++)
                {
                    pixelCols[j] = bitmap.GetPixel(j, i).R / dividend;
                }
                pixelRows[i] = pixelCols.Take(bitmap.Width);
            }

            return new Matrix<int>(pixelRows.Take(bitmap.Height), bitmap.Width, bitmap.Height);
        }

        public static Bitmap ToGrayScale(this Bitmap bitmap)
        {
            var newImage = new Bitmap(bitmap.Width, bitmap.Height);
            for (var i = 0; i < bitmap.Height; i++)
            {
                for (var j = 0; j < bitmap.Width; j++)
                {
                    var pixel = bitmap.GetPixel(j, i);
                    var grayColor = (pixel.R + pixel.G + pixel.B) / 3;
                    newImage.SetPixel(j, i, Color.FromArgb(grayColor, grayColor, grayColor));
                }
            }
            return newImage;
        }

        public static Bitmap ToBitmap(this string base64) => new Bitmap(new MemoryStream(Convert.FromBase64String(base64)));

        public static Bitmap ToBitmap(this int[][] matrix)
        {
            var image = new Bitmap(matrix.Length, matrix[0].Length);

            for (var i = 0; i < matrix.Length; i++)
            {
                for (var j = 0; j < matrix[0].Length; j++)
                {
                    image.SetPixel(i, j, Color.FromArgb(matrix[i][j], matrix[i][j], matrix[i][j]));
                }
            }

            return image;
        }

        public static Bitmap ToBitmap(this Matrix<int> matrix)
        {
            var image = new Bitmap(matrix.Width, matrix.Height);

            var rowEnumerator = matrix.GetEnumerator();
            IEnumerator<int> colEnumerator = null;

            for (var i = 0; i < matrix.Height && rowEnumerator.MoveNext(); i++)
            {
                colEnumerator = rowEnumerator.Current.GetEnumerator();
                for (var j = 0; j < matrix.Width && colEnumerator.MoveNext(); j++)
                {
                    var color = colEnumerator.Current > 255 ?
                        255 :
                        colEnumerator.Current < 0 ?
                        0 :
                        colEnumerator.Current;
                    image.SetPixel(j, i, Color.FromArgb(color, color, color));
                }
            }

            return image;
        }

        public static Base64Image ToBase64Image(this Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);

                return new Base64Image
                {
                    Base64 = Convert.ToBase64String(stream.ToArray())
                };
            }
        }

        public static async Task<byte[]> GetBitmapFromUrl(string relativePath, IHostingEnvironment env)
        {
            var absolutePath = $"{env.WebRootPath}{relativePath}";
            return await File.ReadAllBytesAsync(relativePath);
        }
    }
}
