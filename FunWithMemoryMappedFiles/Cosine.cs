using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunWithMemoryMappedFiles
{
    static class Distance
    {

        static public float CalculateCosine(ref float[] Values,ref int[] Cols,ref int[] Rows, int RowAindex, int RowBindex)
        {
            int OffsetShortStart = 0;
            int OffsetLongStart = 0;
            int OffsetShortEnd = 0;
            int OffsetLongEnd = 0;


            // выбираем определение с наименьшим количеством лемм
            if ((Rows[RowAindex + 1] - Rows[RowAindex]) <= (Rows[RowBindex + 1] - Rows[RowBindex]))
            {
                OffsetShortStart = Rows[RowAindex];
                OffsetShortEnd = Rows[RowAindex + 1];

                OffsetLongStart = Rows[RowBindex];                
                OffsetLongEnd = Rows[RowBindex + 1];
            }
            else
            {
                OffsetShortStart = Rows[RowBindex];
                OffsetShortEnd = Rows[RowBindex + 1];
                
                OffsetLongStart = Rows[RowAindex];
                OffsetLongEnd = Rows[RowAindex + 1];
            }

         

            float Sum = 0;
            float SumSqrA = 0;
            float SumSqrB = 0;

            
            // перебираем леммы в коротком определении
            for (int i = OffsetShortStart; i < OffsetShortEnd; i++)
            {
                int first = OffsetLongStart;
                int last = OffsetShortEnd-1;

                int mid;
                /* Если просматриваемый участок непустой, first<last */
                while (first < last)
                {
                    mid = first + (last - first) / 2;

                    if (i <= Cols[mid])
                    {
                        last = mid;
                    }
                    else
                    {
                        first = mid + 1;
                    }
                }


                if (Cols[last] == i)
                {
                    
                    float valueA = Values[i];
                    float valueB = Values[last];

                    //накапливаем числитель и суммы квадратов в знаменателе
                    Sum += valueA * valueB;
                    SumSqrA += valueA * valueA;
                    SumSqrB += valueB * valueB;
               
                }
                            

            }
            
            return (float)(Sum / (Math.Sqrt(SumSqrA) * Math.Sqrt(SumSqrB)));
        }
        static public float CalculateOverlap(ref float[] Values, ref int[] Cols, ref int[] Rows, int RowAindex, int RowBindex)
        {
            int OffsetShortStart = 0;
            int OffsetLongStart = 0;
            int OffsetShortEnd = 0;
            int OffsetLongEnd = 0;

            int overlap =0;

            // выбираем определение с наименьшим количеством лемм
            if ((Rows[RowAindex + 1] - Rows[RowAindex]) <= (Rows[RowBindex + 1] - Rows[RowBindex]))
            {
                OffsetShortStart = Rows[RowAindex];
                OffsetShortEnd = Rows[RowAindex + 1];

                OffsetLongStart = Rows[RowBindex];
                OffsetLongEnd = Rows[RowBindex + 1];
            }
            else
            {
                OffsetShortStart = Rows[RowBindex];
                OffsetShortEnd = Rows[RowBindex + 1];

                OffsetLongStart = Rows[RowAindex];
                OffsetLongEnd = Rows[RowAindex + 1];
            }

            // перебираем леммы в коротком определении
            for (int i = OffsetShortStart; i < OffsetShortEnd; i++)
            {
                int first = OffsetLongStart;
                int last = OffsetShortEnd - 1;

                int mid;
                /* Если просматриваемый участок непустой, first<last */
                while (first < last)
                {
                    mid = first + (last - first) / 2;

                    if (i <= Cols[mid])
                    {
                        last = mid;
                    }
                    else
                    {
                        first = mid + 1;
                    }
                }


                if (Cols[last] == i)
                {
                    overlap++;
                }


            }

            return (float)2 * overlap / (Rows[RowAindex + 1] - Rows[RowAindex]) * (Rows[RowBindex + 1] - Rows[RowBindex]);
        }
    }
}
