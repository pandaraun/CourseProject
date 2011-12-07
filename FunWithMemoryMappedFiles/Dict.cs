using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FunWithMemoryMappedFiles
{
    static class Dict
    {

        #region Private Members

        private static HashSet<string> _stoplist;
        private static SortedDictionary<string, SortedDictionary<string, float>> _dict;
        private static int _nonezeros;
        //список термов
        private static List<string> _terms;
        //список слов - документов
        private static List<string> _words;

        //инициализация полей словаря
        static Dict()
        {
            _stoplist = new HashSet<string>();
            _dict = new SortedDictionary<string, SortedDictionary<string, float>>();
            _nonezeros = 0;
            //список термов
            _terms = new List<string>();
            //список слов - документов
            _words = new List<string>();
        }

        #endregion

        #region Public Members
        public static int NoneZeros { get { return _nonezeros; } }
        public static List<string> Lemms { get { return _terms; } }
        public static List<string> Words { get { return _words; } }
        public static SortedDictionary<string, SortedDictionary<string, float>> GetDict { get { return _dict; } }
        #endregion        

        #region Methods
        /// <summary>
        /// статичный метод класса создает 
        /// словарь для каджого слова в слова создается сортированный словарь лемма - частота
        /// </summary>
        /// <param name="dictpath">путь к словарю</param>
        /// <param name="stoplistfile">путь к файлу со стоп-словами</param>
        public static void Create(string dictpath, string stoplistfile)
        {     

            try
            {
                _stoplist = new HashSet<string>(File.ReadLines(stoplistfile));
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }

            //отсекаем только нужные части речи
            Regex rx = new Regex(@"(VV#|VVN#|VVP#|JJ#|NN#|NNS|#NP)(?<lemma>\w+\b)", RegexOptions.Compiled);
                        
            char[] separator = { ';' };

            //построение словаря

            GC.Collect(1, GCCollectionMode.Forced);            

            string line = string.Empty;
            using (StreamReader sr = new StreamReader(dictpath))
            {

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;

                    string[] result = line.Split(separator);

                    //для слов с несколькими определениями берем первое, остальные пропускаем//
                    if (_dict.ContainsKey(result[0])) { Console.WriteLine(result[0]); continue; }



                    //получаем коллекцию совпадений по частям речи
                    MatchCollection matches = rx.Matches(result[1]);
                    var lemm = string.Empty;
                    //создаем словарь термов текущего слова - документа
                    var term = new SortedDictionary<string, float>();


                    foreach (Match m in matches)
                    {
                        lemm = m.Result("${lemma}");
                        //исключаем из словарика все стоп - слова
                        if (_stoplist.Contains(lemm)) continue;

                        
                        //выполняем инкремент частоты старой леммы
                        if (term.ContainsKey(lemm)) term[lemm]++;
                            
                        else
                        {
                            //ставим единичную частоту для новой леммы
                            term.Add(lemm, 1);
                            //добавляем в список лемм, если ее там нет еще

                            if (!_terms.Contains(lemm)) _terms.Add(lemm);
                            //увеличваем число ненулевых элементов на размер ключей в словаре термов текущего слова - документа  
                            _nonezeros++;
                        }

                    };
                    //привязываем словарь лемм к текущему слову
                    _dict.Add(result[0], term);
                    //добавляем текущее слово в список слов - документов
                    _words.Add(result[0]);
                }
                //сортируем список лемм , для ускорения доступа.
                _terms.Sort();
            }
        }
        public static CSRMatrix getCSRConceptsMatrix(string conceptspath)
        {
            //индексы столбцы матрицы
            List<int> colIds= new List<int>();
            //номера начал строк 
            List<int>rowPtrs= new List<int>();
            //значения
            List<float> values= new List<float>();

            //заполняем матрицу концептов в CSR
            using(StreamReader conceptreader = new StreamReader(conceptspath))
            {
                string line = string.Empty;
                //указатель на начала строк
                int rowPtr = 0;
                
                while( (line = conceptreader.ReadLine())!= null)
                {
                    if (line == "") continue;
                    rowPtrs.Add(rowPtr);
                    
                    foreach(var lemm in GetDict[line].Keys)
                    {                        
                        //для каждой леммы концепта смотрим индекс в списке лемм словаря
                        colIds.Add(Lemms.IndexOf(lemm));

                        values.Add(GetDict[line][lemm]);                    
	                }                

                    rowPtr += GetDict[line].Count;                    
                }
                rowPtrs.Add(rowPtr);
            }

            return new CSRMatrix(rowPtrs.ToArray(),colIds.ToArray(),values.ToArray());        
        }
        #endregion
    }
}