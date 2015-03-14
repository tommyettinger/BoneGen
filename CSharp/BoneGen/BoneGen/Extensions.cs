using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneGen
{
    /// <summary>
    /// Extension methods used by BoneGen, usable by others as well!
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Insert and replace a section of a larger 2D array with a smaller one.
        /// If coords are only partially in-bounds, only the section that is within the
        /// boundaries of the larger 'mat' will be replaced.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="mat">A 2d array, will be modified.</param>
        /// <param name="items">A (possibly) smaller 2D array to be inserted.</param>
        /// <param name="coord1">The coordinate to be given first as an index to mat and items.</param>
        /// <param name="coord2">The coordnate to be given second as an index to mat and items.</param>
        /// <returns></returns>
        public static T[,] Insert<T>(this T[,] mat, T[,] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.GetLength(0); i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items.GetLength(1); j++, j2++)
                {
                    if (i < 0 || j < 0 || i >= mat.GetLength(0) || j >= mat.GetLength(1))
                        continue;
                    mat[i, j] = items[i1, j2];
                }
            }
            return mat;
        }
        /// <summary>
        /// Insert and replace a section of a larger 2D array with a smaller one, here a jagged smaller array.
        /// If coords are only partially in-bounds, only the section that is within the
        /// boundaries of the larger 'mat' will be replaced.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="mat">A 2d array, will be modified.</param>
        /// <param name="items">A (possibly) smaller jagged array to be inserted.</param>
        /// <param name="coord1">The coordinate to be given first as an index to mat and items.</param>
        /// <param name="coord2">The coordnate to be given second as an index to mat and items.</param>
        /// <returns></returns>
        public static T[,] Insert<T>(this T[,] mat, T[][] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.Length; i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items[i1].Length; j++, j2++)
                {
                    if (i < 0 || j < 0 || i >= mat.GetLength(0) || j >= mat.GetLength(1))
                        continue;
                    mat[i, j] = items[i1][j2];
                }
            }
            return mat;
        }
        /// <summary>
        /// Insert and replace a section of a larger 2D char array with a smaller one here a 1D
        /// array of strings that is used as a 2D grid of chars.
        /// If coords are only partially in-bounds, only the section that is within the
        /// boundaries of the larger 'mat' will be replaced.
        /// </summary>
        /// <param name="mat">A 2d array, will be modified.</param>
        /// <param name="items">A (possibly) smaller (and possibly jagged) array of strings to be inserted.</param>
        /// <param name="coord1">The coordinate to be given first as an index to mat and items.</param>
        /// <param name="coord2">The coordnate to be given second as an index to mat and items.</param>
        /// <returns></returns>
        public static char[,] Insert(this char[,] mat, string[] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.Length; i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items[i1].Length; j++, j2++)
                {
                    if (i < 0 || j < 0 || i >= mat.GetLength(0) || j >= mat.GetLength(1))
                        continue;
                    mat[i, j] = items[i1][j2];
                }
            }
            return mat;
        }
    }
}
