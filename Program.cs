using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Diagnostics;

namespace XmlSDK
{

    class Program
    {


        static void Main(string[] args)
        {
            string folderName = @"d:\1\";      //место для сохранения xml по умолчанию
            string DocmFileName;
            string DocmFilePath;
            

            Console.WriteLine("Enter name of docm file: ");
            DocmFileName = Console.ReadLine();
            DocmFilePath = folderName + DocmFileName + ".docm";
            string XmlFilePath = System.IO.Path.Combine(folderName, (DocmFileName+".xml"));
           // string DocmFilePath = System.IO.Path.Combine(folderName, DocmFileName);
            StringBuilder str = new StringBuilder();
            DocBook db = new DocBook();
            Docm dm = new Docm();
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            string st1 = dm.ReadDocmDocument(DocmFilePath, db);
            str.Append(st1);

          //  Console.WriteLine(str.ToString());
           // Console.WriteLine(str.Length);
            db.createXML(XmlFilePath, str);
            sWatch.Stop();
            Console.WriteLine("Parse time (milliseconds): " + sWatch.ElapsedMilliseconds.ToString());

          //  Console.WriteLine(db.ReadXmlDocument(XmlFilePath));
            
           // WriteDocmDocument();
            Console.ReadKey();

            

        }
    }
}
