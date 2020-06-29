using System; 
using System.Collections; 
using System.Data;

namespace MasterMESWS
{
    /**/
    /**/
    /**/
    /// <summary> 
    /// DataSet助手 
    /// </summary> 
    public class DataSetHelper
    {
        private class FieldInfo
        {
            public string RelationName;
            public string FieldName;
            public string FieldAlias;
            public string Aggregate;
        }

        private DataSet ds;
        private ArrayList m_FieldInfo;
        private string m_FieldList;
        private ArrayList GroupByFieldInfo;
        private string GroupByFieldList;

        public DataSet DataSet
        {
            get { return ds; }
        }

        #region Construction

        public DataSetHelper()
        {
            ds = null;
        }

        public DataSetHelper(ref DataSet dataSet)
        {
            ds = dataSet;
        }

        #endregion

        #region Private Methods

        private bool ColumnEqual(object objectA, object objectB)
        {
            if (objectA == DBNull.Value && objectB == DBNull.Value)
            {
                return true;
            }
            if (objectA == DBNull.Value || objectB == DBNull.Value)
            {
                return false;
            }
            return (objectA.Equals(objectB));
        }

        private bool RowEqual(DataRow rowA, DataRow rowB, DataColumnCollection columns)
        {
            bool result = true;
            for (int i = 0; i < columns.Count; i++)
            {
                result &= ColumnEqual(rowA[columns[i].ColumnName], rowB[columns[i].ColumnName]);
            }
            return result;
        }

        private void ParseFieldList(string fieldList, bool allowRelation)
        {
            if (m_FieldList == fieldList)
            {
                return;
            }
            m_FieldInfo = new ArrayList();
            m_FieldList = fieldList;
            FieldInfo Field;
            string[] FieldParts;
            string[] Fields = fieldList.Split(',');
            for (int i = 0; i <= Fields.Length - 1; i++)
            {
                Field = new FieldInfo();
                FieldParts = Fields[i].Trim().Split(' ');
                switch (FieldParts.Length)
                {
                    case 1:
                        //to be set at the end of the loop 
                        break;
                    case 2:
                        Field.FieldAlias = FieldParts[1];
                        break;
                    default:
                        return;
                }
                FieldParts = FieldParts[0].Split('.');
                switch (FieldParts.Length)
                {
                    case 1:
                        Field.FieldName = FieldParts[0];
                        break;
                    case 2:
                        if (allowRelation == false)
                        {
                            return;
                        }
                        Field.RelationName = FieldParts[0].Trim();
                        Field.FieldName = FieldParts[1].Trim();
                        break;
                    default:
                        return;
                }
                if (Field.FieldAlias == null)
                {
                    Field.FieldAlias = Field.FieldName;
                }
                m_FieldInfo.Add(Field);
            }
        }

        private DataTable CreateTable(string tableName, DataTable sourceTable, string fieldList)
        {
            DataTable dt;
            if (fieldList.Trim() == "")
            {
                dt = sourceTable.Clone();
                dt.TableName = tableName;
            }
            else
            {
                dt = new DataTable(tableName);
                ParseFieldList(fieldList, false);
                DataColumn dc;
                foreach (FieldInfo Field in m_FieldInfo)
                {
                    dc = sourceTable.Columns[Field.FieldName];
                    DataColumn column = new DataColumn();
                    column.ColumnName = Field.FieldAlias;
                    column.DataType = dc.DataType;
                    column.MaxLength = dc.MaxLength;
                    column.Expression = dc.Expression;
                    dt.Columns.Add(column);
                }
            }
            if (ds != null)
            {
                ds.Tables.Add(dt);
            }
            return dt;
        }

        private void InsertInto(DataTable destTable, DataTable sourceTable, string fieldList, string rowFilter, string sort)
        {
            ParseFieldList(fieldList, false);
            DataRow[] rows = sourceTable.Select(rowFilter, sort);
            DataRow destRow;
            foreach (DataRow sourceRow in rows)
            {
                destRow = destTable.NewRow();
                if (fieldList == "")
                {
                    foreach (DataColumn dc in destRow.Table.Columns)
                    {
                        if (dc.Expression == "")
                        {
                            destRow[dc] = sourceRow[dc.ColumnName];
                        }
                    }
                }
                else
                {
                    foreach (FieldInfo field in m_FieldInfo)
                    {
                        destRow[field.FieldAlias] = sourceRow[field.FieldName];
                    }
                }
                destTable.Rows.Add(destRow);
            }
        }

        private void ParseGroupByFieldList(string FieldList)
        {
            if (GroupByFieldList == FieldList)
            {
                return;
            }
            GroupByFieldInfo = new ArrayList();
            FieldInfo Field;
            string[] FieldParts;
            string[] Fields = FieldList.Split(',');
            for (int i = 0; i <= Fields.Length - 1; i++)
            {
                Field = new FieldInfo();
                FieldParts = Fields[i].Trim().Split(' ');
                switch (FieldParts.Length)
                {
                    case 1:
                        //to be set at the end of the loop 
                        break;
                    case 2:
                        Field.FieldAlias = FieldParts[1];
                        break;
                    default:
                        return;
                }

                FieldParts = FieldParts[0].Split('(');
                switch (FieldParts.Length)
                {
                    case 1:
                        Field.FieldName = FieldParts[0];
                        break;
                    case 2:
                        Field.Aggregate = FieldParts[0].Trim().ToLower();
                        Field.FieldName = FieldParts[1].Trim(' ', ')');
                        break;
                    default:
                        return;
                }
                if (Field.FieldAlias == null)
                {
                    if (Field.Aggregate == null)
                    {
                        Field.FieldAlias = Field.FieldName;
                    }
                    else
                    {
                        Field.FieldAlias = Field.Aggregate + "of" + Field.FieldName;
                    }
                }
                GroupByFieldInfo.Add(Field);
            }
            GroupByFieldList = FieldList;
        }
        private FieldInfo LocateFieldInfoByName(System.Collections.ArrayList FieldList, string Name)
        {
            //Looks up a FieldInfo record based on FieldName
            foreach (FieldInfo Field in FieldList)
            {
                if (Field.FieldName == Name)
                    return Field;
            }
            return null;
        }

        private object Min(object a, object b)
        {
            //Returns MIN of two values - DBNull is less than all others
            if ((a is DBNull) || (b is DBNull))
                return DBNull.Value;
            if (((IComparable)a).CompareTo(b) == -1)
                return a;
            else
                return b;
        }

        private object Max(object a, object b)
        {
            //Returns Max of two values - DBNull is less than all others
            if (a is DBNull)
                return b;
            if (b is DBNull)
                return a;
            if (((IComparable)a).CompareTo(b) == 1)
                return a;
            else
                return b;
        }

        private object Add(object a, object b)
        {
            try
            {
                //Adds two values - if one is DBNull, then returns the other
                if (a is DBNull)
                    return b;
                if (b is DBNull)
                    return a;
                return (double.Parse(a.ToString()) + double.Parse(b.ToString()));
            }
            catch(System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return 0;
            }
        }

        //private DataTable CreateGroupByTable(string tableName, DataTable sourceTable, string fieldList)
        //{
        //    if (fieldList == null || fieldList.Length == 0)
        //    {
        //        return sourceTable.Clone();
        //    }
        //    else
        //    {
        //        DataTable dt = new DataTable(tableName);
        //        ParseGroupByFieldList(fieldList);
        //        foreach (FieldInfo Field in GroupByFieldInfo)
        //        {
        //            DataColumn dc = sourceTable.Columns[Field.FieldName];
        //            if (Field.Aggregate == null)
        //            {
        //                dt.Columns.Add(Field.FieldAlias, dc.DataType, dc.Expression);
        //            }
        //            else
        //            {
        //                dt.Columns.Add(Field.FieldAlias, dc.DataType);
        //            }
        //        }
        //        if (ds != null)
        //        {
        //            ds.Tables.Add(dt);
        //        }
        //        return dt;
        //    }
        //}
        #endregion

        #region Public Methods
        public DataTable CreateGroupByTable(string TableName, DataTable SourceTable, string FieldList)
        {
            /*
             * Creates a table based on aggregates of fields of another table
             * 
             * RowFilter affects rows before GroupBy operation. No "Having" support
             * though this can be emulated by subsequent filtering of the table that results
             * 
             *  FieldList syntax: fieldname[ alias]|aggregatefunction(fieldname)[ alias], ...
            */
            if (FieldList == null)
            {
                throw new ArgumentException("You must specify at least one field in the field list.");
                //return CreateTable(TableName, SourceTable);
            }
            else
            {
                DataTable dt = new DataTable(TableName);
                ParseGroupByFieldList(FieldList);
                foreach (FieldInfo Field in GroupByFieldInfo)
                {
                    DataColumn dc = SourceTable.Columns[Field.FieldName];
                    if (Field.Aggregate == null)
                        dt.Columns.Add(Field.FieldAlias, dc.DataType, dc.Expression);
                    else
                        dt.Columns.Add(Field.FieldAlias, dc.DataType);
                }
                if (ds != null)
                    ds.Tables.Add(dt);
                return dt;
            }
        }

        public void InsertGroupByInto(DataTable DestTable, DataTable SourceTable, string FieldList, string RowFilter, string GroupBy)
        {
            /*
             * Copies the selected rows and columns from SourceTable and inserts them into DestTable
             * FieldList has same format as CreateGroupByTable
            */
            if (FieldList == null)
                throw new ArgumentException("You must specify at least one field in the field list.");
            ParseGroupByFieldList(FieldList);	//parse field list
            ParseFieldList(GroupBy, false);			//parse field names to Group By into an arraylist
            DataRow[] Rows = SourceTable.Select(RowFilter, GroupBy);
            DataRow LastSourceRow = null, DestRow = null; bool SameRow; int RowCount = 0;
            foreach (DataRow SourceRow in Rows)
            {
                SameRow = false;
                if (LastSourceRow != null)
                {
                    SameRow = true;
                    foreach (FieldInfo Field in m_FieldInfo)
                    {
                        if (!ColumnEqual(LastSourceRow[Field.FieldName], SourceRow[Field.FieldName]))
                        {
                            SameRow = false;
                            break;
                        }
                    }
                    if (!SameRow)
                        DestTable.Rows.Add(DestRow);
                }
                if (!SameRow)
                {
                    DestRow = DestTable.NewRow();
                    RowCount = 0;
                }
                RowCount += 1;
                foreach (FieldInfo Field in GroupByFieldInfo)
                {
                    switch (Field.Aggregate)    //this test is case-sensitive
                    {
                        case null:        //implicit last
                        case "":        //implicit last
                        case "last":
                            DestRow[Field.FieldAlias] = SourceRow[Field.FieldName];
                            break;
                        case "first":
                            if (RowCount == 1)
                                DestRow[Field.FieldAlias] = SourceRow[Field.FieldName];
                            break;
                        case "count":
                            DestRow[Field.FieldAlias] = RowCount;
                            break;
                        case "sum":
                            DestRow[Field.FieldAlias] = Add(DestRow[Field.FieldAlias], SourceRow[Field.FieldName]);
                            break;
                        case "max":
                            DestRow[Field.FieldAlias] = Max(DestRow[Field.FieldAlias], SourceRow[Field.FieldName]);
                            break;
                        case "min":
                            if (RowCount == 1)
                                DestRow[Field.FieldAlias] = SourceRow[Field.FieldName];
                            else
                                DestRow[Field.FieldAlias] = Min(DestRow[Field.FieldAlias], SourceRow[Field.FieldName]);
                            break;
                    }
                }
                LastSourceRow = SourceRow;
            }
            if (DestRow != null)
                DestTable.Rows.Add(DestRow);
        }




        public DataTable SelectGroupByInto(string TableName, DataTable SourceTable, string FieldList,string RowFilter, string GroupBy)
        {
            /*
             * Selects data from one DataTable to another and performs various aggregate functions
             * along the way. See InsertGroupByInto and ParseGroupByFieldList for supported aggregate functions.
             */
            DataTable dt = CreateGroupByTable(TableName, SourceTable, FieldList);
            InsertGroupByInto(dt, SourceTable, FieldList, RowFilter, GroupBy);
            return dt;
        }

        public DataTable SelectDistinct(string TableName, DataTable SourceTable, string FieldName)
        {
            DataTable dt = new DataTable(TableName);
            dt.Columns.Add(FieldName, SourceTable.Columns[FieldName].DataType);

            object LastValue = null;
            foreach (DataRow dr in SourceTable.Select("", FieldName))
            {
                if (LastValue == null || !(ColumnEqual(LastValue, dr[FieldName])))
                {
                    LastValue = dr[FieldName];
                    dt.Rows.Add(new object[] { LastValue });
                }
            }
            if (ds != null)
                ds.Tables.Add(dt);
            return dt;
        }
        #endregion

    }
}