using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneGen
{
    public static class Extensions
    {

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
        public static T[,] Insert<T>(this T[,] mat, T[][] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0 || items[0].Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.Length; i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items[0].Length; j++, j2++)
                {
                    if (i < 0 || j < 0 || i >= mat.GetLength(0) || j >= mat.GetLength(1))
                        continue;
                    mat[i, j] = items[i1][j2];
                }
            }
            return mat;
        }
        public static char[,] Insert(this char[,] mat, string[] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0 || items[0].Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.Length; i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items[0].Length; j++, j2++)
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
