using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunWithMemoryMappedFiles
{
    class CSRMatrix
    {
        
        #region Private Members

            private int[] _rowPtrArr;
            private int[] _colIdArr;
            private float[] _values;

        #endregion

        #region Public Members

        public int[] RowPtr
        {
            get
            {
                return this._rowPtrArr;
            }
            set
            {
                this._rowPtrArr=value;
            }
        }
        public int[] ColIdArr
        {
            get
            {
                return this._colIdArr;
            }
            set
            {
                this._colIdArr=value;
            }
        }
        public float[] Values
        {
            get             
            {
                return this._values;
            }
            set
            {
                this._values = value;
            }
        
        }

        public CSRMatrix()
        {             
        }
        public CSRMatrix(int[] rowPtr, int[] colIdArr, float[] values)
        {
            RowPtr = rowPtr;
            ColIdArr = colIdArr;
            Values = values;        
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Values : [ ");
            foreach (float value in Values)
            {
                sb.AppendFormat(" {0} ",value);
            }

            sb.AppendFormat("]\n");
            sb.Append("Columns : [");

            foreach(int colid in ColIdArr)
            {
                sb.AppendFormat(" {0} ", colid);
            }

            sb.AppendFormat("]\n");
            sb.Append("RowPtr : [");

            foreach (int rowPtr in RowPtr)
            {
                sb.AppendFormat(" {0} ", rowPtr);
            }

            sb.AppendFormat("]\n");

            return sb.ToString();
        }
        #endregion     

    }
}
