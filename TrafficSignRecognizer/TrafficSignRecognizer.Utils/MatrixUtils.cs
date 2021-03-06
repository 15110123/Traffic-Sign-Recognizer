﻿using System;
using System.Collections.Generic;
using System.Linq;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.Utils
{
    public static class MatrixUtils
    {
        public static Matrix<int> ConvoluteWith(this Matrix<int> matrix, Matrix<int> filter)
        {
            var filterArray = filter
                .Select(x => x.ToArray())
                .ToArray();

            //Result matrix's row count
            var rowCount = matrix.Height / filterArray.Length;

            //Result matrix's column count
            var colCount = matrix.Width / filterArray[0].Length;

            //Rent array as result
            var result = System.Buffers.ArrayPool<int[]>.Shared.Rent(rowCount);

            var rowEnumerator = matrix.GetEnumerator();
            IEnumerator<int> colEnumerator = null;

            for (var i = 0; i < matrix.Height && rowEnumerator.MoveNext(); i++)
            {
                colEnumerator = rowEnumerator.Current.GetEnumerator();
                for (var j = 0; j < matrix.Width && colEnumerator.MoveNext(); j++)
                {
                    var position = FindPosition(j, i);
                    if (result[position.Y] == null)
                    {
                        result[position.Y] = System.Buffers.ArrayPool<int>.Shared.Rent(colCount);
                    }
                    result[position.Y][position.X] += colEnumerator.Current * filterArray[position.remainingY][position.remainingX];
                }
            }

            return new Matrix<int>(result.Take(rowCount), colCount, rowCount);

            //Local Function
            (int X, int Y, int remainingX, int remainingY) FindPosition(int currentColIndex, int currentRowIndex)
            {
                return (
                    (int)Math.Round((double)currentColIndex / filterArray[0].Length, MidpointRounding.AwayFromZero),
                    (int)Math.Round((double)currentRowIndex / filterArray.Length, MidpointRounding.AwayFromZero),
                    currentColIndex % filterArray[0].Length,
                    currentRowIndex % filterArray.Length
                    );
            }
        }
    }
}
