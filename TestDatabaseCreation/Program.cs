using Microsoft.Office.Interop.Access.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.IO.Path;
using static System.Reflection.Assembly;
using static Microsoft.Office.Interop.Access.Dao.LanguageConstants;
using static System.IO.File;
using static Microsoft.Office.Interop.Access.Dao.DatabaseTypeEnum;

namespace TestDatabaseCreation {
    class Program {
        static void Main(string[] args) {
            var filename = "test.accdb";
            var dbFilePath = Combine(GetDirectoryName(GetExecutingAssembly().Location), filename);

            var engine = new DBEngine();
            var dbs = engine.CreateDatabase(dbFilePath, dbLangGeneral, dbVersion120);
            dbs.Close();
            dbs = null;

            Delete(dbFilePath);
        }
    }
}
