using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Timers;
using System.Diagnostics;

namespace XmlSDK
{
    class Docm
    {

        public void WriteDocmDocument(string DocmFilePath)
        {
            WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(DocmFilePath, true);  // Open a WordprocessingDocument for editing using the DocmFilePath.
            Body body = wordprocessingDocument.MainDocumentPart.Document.Body;  // Assign a reference to the existing document body.
            Paragraph para = body.AppendChild(new Paragraph());     // Add new text.
            Run run = para.AppendChild(new Run());
            string txt = "New paragraph";
            run.AppendChild(new Text(txt));
            wordprocessingDocument.Close(); // Close the handle explicitly.
        }

        public string ReadDocmDocument(string DocmFilePath, DocBook db)
        {
            

            StringBuilder sb = new StringBuilder();
            WordprocessingDocument package = WordprocessingDocument.Open(DocmFilePath, true); // Open a WordprocessingDocument for editing using the DocmFilePath.
            OpenXmlElement element = package.MainDocumentPart.Document.Body;
            if (element == null)
            {
                return string.Empty;
            }
            sb.Append(GetText(element, db));
            
            package.Close();
            return sb.ToString();


        }

        public string GetText(OpenXmlElement element, DocBook db)
        {
            StringBuilder PlainTextInWord = new StringBuilder();
            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    // Text 
                    case "t":
                        PlainTextInWord.Append(section.InnerText);
                       // db.addPara(PlainTextInWord, "t");
                        break;


                    case "cr":                          // Carriage return 
                    case "br":                          // Page break 
                        PlainTextInWord.Append(Environment.NewLine);
                        //db.addPara(PlainTextInWord, "br");
                        break;


                    // Tab 
                    case "tab":
                        PlainTextInWord.Append("\t");
                        break;


                    // Paragraph 
                    case "p":
                        PlainTextInWord.Append(GetText(section,db));
                        PlainTextInWord.AppendLine(Environment.NewLine);
                        break;


                    default:
                        PlainTextInWord.Append(GetText(section,db));
                        break;
                }
            }


            return PlainTextInWord.ToString();
        }
    }
}
