using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;



namespace KnnMutualKnn
{
    class Program
    {
        static void help()
        {
            
            Console.WriteLine("please type 'knnmutualknn -c= -d= -s= -k= '");
            Console.WriteLine(@"-c : concepts file path");
            Console.WriteLine(@"-d : defintion file path");
            Console.WriteLine(@"-s : stoplist file path");
            Console.WriteLine(@"-k : nearest neighbours ");
            Console.WriteLine(@"[-knn] : output knn file ");
            Console.WriteLine(@"[-mknn] : output mutual knn file ");
        }
        static void Main(string[] args)
        {
            //путь к файлу с определениями 
            string dictpath = string.Empty ;
            //путь к файлу со стоп-словами
            string stoplistfile=string.Empty;
            //путь  к файлу между которыми необходимо найти семантические отношения
            string conceptsfile=string.Empty;
            //количество соседей для поиска
            int k=0;
            //выходной файл для первого алгоритма
            string knnfile=@"knn";
            //выходной файл для второго алгоритма
            string mknnfile=@"mknn";

            #region console 
            System.Console.WriteLine("parameters count = {0}", args.Length);

            for (int i = 0; i < args.Length; i++)
            {
                System.Console.WriteLine("Arg[{0}] = [{1}]", i, args[i]);
            }
            if (args.Length <4 || args.Length>6)
            {
                Console.WriteLine("not enough params");
                help();
                
                return;
            }
            else
            {

                foreach (var arg in args)
                {
                    var param = arg.Split('=');
                    var key = param[0];
                    var value = param[1];
                    switch (key)
                    {
                        case "-c":
                        {
                            conceptsfile = value;
                            break;
                        }
                        case "-s":
                        {
                            stoplistfile = value;
                            break;
                        }
                        case "-d":
                        {
                            dictpath = value;
                            break;
                        }
                        case "-k":
                        {
                            k = int.Parse(value);
                            break;
                        }
                        case "-knn":
                        {
                            knnfile = value;
                            break;
                        }
                        case "-mknn":
                        {
                            mknnfile = value;
                            break;
                        }
                        default:
                        {
                            Console.WriteLine("uknown param : {0}", key);
                            help();
                            return;
                            
                        }
                        
                    }
                }
            }

            #endregion


                                   
            //парсинг словаря , исключение стоп-слов из входных данных
            Dict.Create(dictpath, stoplistfile);    

            
            //для файла концпетов строим разреженную матрицу в CSR
            //берем первые частотных  5k лемм           
            CSRMatrix conceptsMtrx = Dict.getCSRConceptsMatrix(conceptsfile,5000);        
            //количество полученных концептов для анализа
            int ConceptsCnt = conceptsMtrx._rowPtrArr.Count()-1; 
            
            
            PriorityQueue<float, int>[] result;
            result = CalCulateAllDistAllMetrics(conceptsMtrx);             
            

            
            int[][] knn;
            //первый алгоритм
            getKnn(knnfile,out knn, result, ConceptsCnt, k);    
            
            //второй алгоритм
            getMutualKnn(mknnfile, knn);


            
            

        }


        #region knn
        static void getKnn(string resultknnfile,out int[][] knn,PriorityQueue<float,int>[] result,int conceptscnt,int k)
        {
            knn = new int[conceptscnt][];
            using (StreamWriter knnwriter = new StreamWriter(resultknnfile))
            {
                for (int i = 0; i < conceptscnt; i++)
                {
                    var temp = new int[k];
                    for (int j = 0; j < k; j++)
                    {
                        temp[j] = result[i].DequeueValue();
                        knnwriter.WriteLine("<{0};{1}>", Dict.Words[i], Dict.Words[temp[j]]);
                    }
                    knn[i] = temp;
                }
            }
        }
        #endregion

        #region mutual knn
        /// <summary>
        /// mutual knn results in text file where each line is in <wordi;wordj>  fomat
        /// </summary>
        /// <param name="restultknnfile">result text file </param>
        /// <param name="knn">matrix containing  top k neighbours</param>
        static void getMutualKnn(string restultknnfile, int[][] knn)
        {
            using (StreamWriter mutualknnwriter = new StreamWriter(restultknnfile))
            {

                for (int i = 0; i < knn.Length; i++)
                {
                    foreach (var neighbour in knn[i])
                    {
                        if (knn[neighbour].Contains(i))
                        {
                            mutualknnwriter.WriteLine("<{0};{1}>", Dict.Words[i], Dict.Words[neighbour]);
                        };
                    }
                }


            }      
        
        }
        #endregion

        #region Distance 
        /// <summary>
        /// returns array of  priority queues for current distance metric
        /// </summary>
        /// <param name="ConceptsCnt"></param>
        /// <param name="conceptsMtrx"></param>
        /// <returns></returns>
        static PriorityQueue<float,int>[] CalCulateAllDistAllMetrics(CSRMatrix conceptsMtrx)
        {
            PriorityQueue<float, int>[] pqs = new PriorityQueue<float, int>[conceptsMtrx._rowPtrArr.Length-1];
            for (int i = 0; i < pqs.Length; i++)
            {
                pqs[i] = new PriorityQueue<float, int>();
            }

            for(int i = 0;i< conceptsMtrx._rowPtrArr.Length - 1;i++)
            {             
                    
                for (int j = 0; j < conceptsMtrx._rowPtrArr.Length - 1; j++)
                {
                    if (i >= j) continue;                      
                    float cos = Distance.Cosine(ref conceptsMtrx._values, ref conceptsMtrx._colIdArr, ref conceptsMtrx._rowPtrArr, i, j);
                    pqs[i].Enqueue(1-cos, j);
                    pqs[j].Enqueue(1-cos, i);                       
                    
                }                                          

            };


            return pqs;
        }
        #endregion
    }
}


