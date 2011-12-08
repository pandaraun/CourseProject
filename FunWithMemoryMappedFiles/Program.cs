using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;



namespace FunWithMemoryMappedFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            

           //путь к файлу с определениями 
            string path =@"C:\test\definitions.csv";  
           //путь к файлу со стоп словами
            string stoplistfile = @"C:\test\stoplist.csv";
            //путь  к файлу между которыми необходимо найти семантические отношения
            string conceptsfile = @"C:\test\concepts.csv";

            var s1 = Stopwatch.StartNew();
            Dict.Create(path, stoplistfile);
            s1.Stop();



            Console.WriteLine("dict created in {0} ms", s1.ElapsedMilliseconds);

            Console.WriteLine("nonezeros : {0}", Dict.NoneZeros);
            Console.WriteLine("sparsity : {0} ", (float)Dict.NoneZeros * 100 / (Dict.Words.Count * Dict.Lemms.Count));
            Console.WriteLine("total words: {0}, total terms: {1}", Dict.Words.Count, Dict.Lemms.Count);

            //добавлен комментарий
            CSRMatrix conceptsMtrx = Dict.getCSRConceptsMatrix(conceptsfile);

            Console.WriteLine("nonezeros : {0}", conceptsMtrx.Values.Count());
            Console.WriteLine("colids : {0}", conceptsMtrx.ColIdArr.Count());
            Console.WriteLine("rowptrs : {0}", conceptsMtrx.RowPtr.Count());
            

            //Console.WriteLine(conceptsMtrx);


            int[] _colids = conceptsMtrx.ColIdArr;
            int[] _rowptrs = conceptsMtrx.RowPtr;
            float[] _values = conceptsMtrx.Values;


            var Clocks = Stopwatch.StartNew();
            for (int i = 0; i < _rowptrs.Length - 1; i++)
            {
                for (int j = 0; j < _rowptrs.Length - 1; j++)
                {
                    
                    if (i >= j) continue;
                    
                    Distance.CalculateOverlap(ref _values, ref _colids, ref _rowptrs, i, j);
                }
                
            }

            Clocks.Stop();
            Console.WriteLine("elapsed : {0} ms", Clocks.ElapsedMilliseconds);
            
     

            
            
            
                
            Console.ReadLine();

        }
    }
}


