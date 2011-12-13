using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnnMutualKnn
{
    static  class Distance
    {
        #region Cosine
        static public float Cosine(ref float[] Values,ref int[] Cols,ref int[] Rows, int RowAindex, int RowBindex)
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
                int last = OffsetLongEnd-1;

                int mid;
                /* Если просматриваемый участок непустой, first<last */
                while (first < last)
                {
                    mid = first + (last - first) / 2;

                    if (Cols[i] <= Cols[mid])
                    {
                        last = mid;
                    }
                    else
                    {
                        first = mid + 1;
                    }
                }


                if (Cols[last] == Cols[i])
                {
                    
                    float valueA = Values[i];
                    float valueB = Values[last];

                    //накапливаем числитель и суммы квадратов в знаменателе
                    Sum += valueA * valueB;
                    SumSqrA += valueA * valueA;
                    SumSqrB += valueB * valueB;
               
                }
                            

            }
            if (Sum ==0) return 0;
            return (float)(Sum / (Math.Sqrt(SumSqrA) * Math.Sqrt(SumSqrB)));
        }
        #endregion 

        #region Overlap
        static public float Overlap(ref float[] Values, ref int[] Cols, ref int[] Rows, int RowAindex, int RowBindex)
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
                int last = OffsetLongEnd - 1;

                int mid;

                /* Если просматриваемый участок непустой, first<last */
                while (first < last)
                {
                    mid = first + (last - first) / 2;

                    if (Cols[i] <= Cols[mid])
                    {
                        last = mid;
                    }
                    else
                    {
                        first = mid + 1;
                    }
                }


                if (Cols[last] == Cols[i])
                {
                    
                    overlap++;
                }


            }
            if (overlap == 0) return 0;
            return (float)2 * overlap /( OffsetShortEnd-OffsetShortStart)*(OffsetLongEnd-OffsetLongStart);
        }
        #endregion

        #region Karaulov
        public static float Karaulov(ref float[] Values, ref int[] Cols, ref int[] Rows, int RowAindex, int RowBindex,int t1,int t2,int t3,ref SortedDictionary<string,byte> _fterms,ref List<string> _lemms)
        {
            int OffsetShortStart = 0;
            int OffsetLongStart = 0;
            int OffsetShortEnd = 0;
            int OffsetLongEnd = 0;

            int overlap = 0;
            int fij = 0;

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
                int last = OffsetLongEnd - 1;

                int mid;
                
                /* Если просматриваемый участок непустой, first<last */
                
                while (first < last)
                {
                    mid = first + (last - first) / 2;

                    if (Cols[i] <= Cols[mid])
                    {
                        last = mid;
                    }
                    else
                    {
                        first = mid + 1;
                    }
                }


                if (Cols[last] == Cols[i])
                {
                     overlap ++;

                     if ( overlap >= t1) return 1;
                     if (overlap >= t2 )
                     {
                         int temp = _fterms[_lemms[Cols[i]]];
                         if (temp > fij) fij = temp;
                         if(fij>=t3) return 1;
                     }                                

                }


            }

                  
            return 0; 

        
        
        
        }
        #endregion

    }
}
