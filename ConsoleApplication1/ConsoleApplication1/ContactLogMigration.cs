using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ContactLogMigration
    {

        static Entities _db;
        

        public static void ImportContactLog()
        {
            using (_db = new Entities())
            {
                //Read in the Contact Log data from the MPLOY table:


                //For each record, transform the data and insert into our DB:

                //


            }
        }
    }
}
