using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SavingObjectsToDatabase.Database
{
   public static class DataBaseTools
    {

        #region Declarations

        public static SQLiteConnection dbcon = new SQLiteConnection((App.Current as App).DatabaseFileName + ".db");

        #endregion
        #region Constructors
        static DataBaseTools()
        {
            DatabaseFieldList = new ObservableCollection<DatabaseField>();
        }
        #endregion
        #region Properties

        public static String TableName
        {
            get { return (App.Current as App).DatabaseTableName; }
        }

        public static ObservableCollection<DatabaseField> DatabaseFieldList { get; set; }

        #endregion
        #region External Methods
        #region Create Database


        public static String CreateDatabase()
        {
            String fieldList = "";

            foreach(DatabaseField dbf in DatabaseFieldList)
            {
                fieldList = fieldList + ", " + dbf.FieldName + " " + dbf.FieldType + " Not Null";
            }

            String sSql = String.Format(@"CREATE TABLE IF NOT EXISTS {0}
                                            (ID Integer Primary Key AutoIncrement Not Null
                                              {1});", TableName, fieldList);

            ISQLiteStatement cnStatement = dbcon.Prepare(sSql);
            cnStatement.Step();

            return sSql;
        }
        #endregion
        #region Write To DB


        public static void WriteRecord()
        {

            String fieldList = "";
            String fieldValues = "";

            foreach (DatabaseField dbf in DatabaseFieldList)
            {
                if(fieldList == "" || fieldValues == "")
                {
                    fieldList = "[" + dbf.FieldName + "]";
                    fieldValues = "'" + dbf.Value + "'";
                }
                else
                {
                    fieldList = fieldList + ",[" + dbf.FieldName + "]";
                    fieldValues = fieldValues + ",'" + dbf.Value + "'";
                }
            }

            String sSql = String.Format(@"INSERT INTO [{0}]({1}) VALUES({2});", TableName, fieldList,fieldValues);

            dbcon.Prepare(sSql).Step();
            
        }

        public static DatabaseField InsertObjectIntoField<myObjectType>(myObjectType myObject,DatabaseField Field)
        {
            String myObjectString = ConvertObjectToString<myObjectType>(myObject);

            Field.Value = myObjectString;
            return Field;
        }

        #endregion
        #region Build Field Objects


        public static DatabaseField  BuildFieldObject(String tp, String name)
        {
            var myField = new DatabaseField()
            {
                FieldName = name,
                FieldType = tp
            };
            return myField;
        }


        public static void FieldListBuilder<myObjectType>(myObjectType FieldObject, String FieldName)
        {

            var field = new DatabaseField()
            {
                FieldName = FieldName,
                FieldType = "nvarchar"
            };

            field = InsertObjectIntoField<myObjectType>(FieldObject, field);
            DatabaseFieldList.Add(field);

          
        }
        #endregion

        #region Read From Database


        public static ObservableCollection<myObjectType> ReturnAllRecords<myObjectType>(String FieldName, String myObjectPath)
        {

            var myObjectList = new ObservableCollection<myObjectType>();

            String myObjectString = "";

            String sSql = String.Format(@"SELECT * FROM {0};",TableName);

            ISQLiteStatement cnStatement = dbcon.Prepare(sSql);

            while(cnStatement.Step() == SQLiteResult.ROW)
            {
                myObjectString = cnStatement[FieldName].ToString();
                myObjectList.Add(ConvertStringToObject<myObjectType>(myObjectString, myObjectPath));
            }

            return myObjectList;
        }



        public static myObjectType SearchRecords<myObjectType>(String FieldName, String myObjectPath, String FieldToSearch, String SearchString)
        {

            String myObjectString = String.Empty;

            String sSql = String.Format(@"SELECT * FROM {0};", TableName);
            ISQLiteStatement cnStatement = dbcon.Prepare(sSql);

            while(cnStatement.Step() == SQLiteResult.ROW)
            {
                myObjectString = cnStatement[FieldName].ToString();

                myObjectType myObject = (myObjectType)Activator.CreateInstance(Type.GetType(myObjectPath));

                myObject = ConvertStringToObject<myObjectType>(myObjectString, myObjectPath);

                PropertyInfo prop = typeof(myObjectType).GetProperty(FieldToSearch);

                if(prop.GetValue(myObject).ToString() == SearchString)
                {
                    return myObject;
                }
            }

            return (myObjectType)Activator.CreateInstance(Type.GetType(myObjectPath));
           
        }
        #endregion


        #endregion
        #region Internatl Methods
        #region Convert Object to Json String

        private static String ConvertObjectToString<myObjectType>(myObjectType myObject)
        {
            String content = String.Empty;


            var js = new DataContractJsonSerializer(typeof(myObjectType));

            var ms = new MemoryStream();

            js.WriteObject(ms, myObject);
            ms.Position = 0;

            var reader = new StreamReader(ms);

            content = reader.ReadToEnd();


            return content;
        }
        #endregion
        #region Convert String To Object

        private static myObjectType ConvertStringToObject<myObjectType>(String myObjectString, String myObjectPath)
        {

            myObjectType myObject = (myObjectType)Activator.CreateInstance(Type.GetType(myObjectPath));

            var js = new DataContractJsonSerializer(typeof(myObjectType));

            byte[] byteArray = Encoding.UTF8.GetBytes(myObjectString);

            var ms = new MemoryStream(byteArray);

            myObject = (myObjectType)js.ReadObject(ms);


            return myObject;
        }
        #endregion
        #endregion

    }


    #region Database Field Object Class
    public class DatabaseField
    {
        public String FieldType { get; set; }
        public String FieldName { get; set; }
        public String Value { get; set; }
    }
    #endregion

    #region Instructions
    // add your won instructions here as to how to copy this to a new app.
    #endregion

}
