using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnnMutualKnn
{
    class CSRMatrix
    {
        
        #region Private Members
            
            public int[] _rowPtrArr;
            public int[] _colIdArr;
            public float[] _values;

        #endregion

        #region Public Members


        public CSRMatrix()
        {             
        }
        public CSRMatrix(int[] rowPtr, int[] colIdArr, float[] values)
        {
            this._rowPtrArr = rowPtr;
            this._colIdArr = colIdArr;
            this._values = values;        
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Values : [ ");
            foreach (float value in this._values)
            {
                sb.AppendFormat(" {0} ",value);
            }

            sb.AppendFormat("]\n");
            sb.Append("Columns : [");

            foreach(int colid in this._colIdArr)
            {
                sb.AppendFormat(" {0} ", colid);
            }

            sb.AppendFormat("]\n");
            sb.Append("RowPtr : [");

            foreach (int rowPtr in this._rowPtrArr)
            {
                sb.AppendFormat(" {0} ", rowPtr);
            }

            sb.AppendFormat("]\n");

            return sb.ToString();
        }
        #endregion     

    }
}
