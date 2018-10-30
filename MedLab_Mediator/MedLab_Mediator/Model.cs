using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MedLab_Mediator
{
    //abstract class for getting info from service
    abstract class Model
    {
        public String host;
        public String uri;
        public String soapAction;
       
        /* 
        static void Main(string[] args)
        {
            //creating object of program class to access methods    
            //Model obj = new Model();

            //Calling GetKnowledgeBase method
            obj.host = "degel.ise.bgu.ac.il";
            obj.uri = "/DeGeLIV/DeGeLogic/KnowledgeLibraryWS.asmx";
            obj.soapAction = "http://tempuri.org/GetKnowledgeBase";
            obj.GetKnowledgeBase(90);
            

            //Calling GetDataByConcept method
            obj.host = "dam.ise.bgu.ac.il";
            obj.uri = "/dime/aKontroller.asmx";
            obj.soapAction = "http://tempuri.org/GetDataByConcept";
            obj.GetDataByConcept(2, "Bands");
        
        }
        */

        public HttpWebRequest CreateSOAPWebRequest()
        {
            //Making Web Request    
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(@"http://" + host + uri);
            //SOAPAction    
            Req.Headers.Add(@"SOAPAction:" + soapAction);
            //Content_type    
            Req.ContentType = "text/xml;charset=\"utf-8\"";
            Req.Accept = "text/xml";
            //HTTP method    
            Req.Method = "POST";
            //return HttpWebRequest    
            return Req;
        }
    }

    //get all KI
    class KnowledgeItems : Model
    {
        int kbid;

        //constructor
        public KnowledgeItems() 
        {
            host= "degel.ise.bgu.ac.il";
            uri= "/DeGeLIV/DeGeLogic/KnowledgeLibraryWS.asmx";
            soapAction= "http://tempuri.org/GetKnowledgeBase";
            kbid = 90;
        }

        //get all knowledge items with kbid=90
        public String[] GetKnowledgeItems()
        {
            //Calling CreateSOAPWebRequest method    
            HttpWebRequest request = CreateSOAPWebRequest();
            XmlDocument SOAPReqBody = new XmlDocument();
            //SOAP Body Request
            SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-   instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">  
             <soap:Body>  
                <GetKnowledgeBase xmlns = ""http://tempuri.org/"">  
                  <KBID>" + kbid + @"</KBID>  
                </GetKnowledgeBase>  
              </soap:Body>  
            </soap:Envelope>");

            using (Stream stream = request.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }
            //Geting response from request    
            using (WebResponse Serviceres = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                {
                    //reading stream    
                    var ServiceResult = rd.ReadToEnd();
                    //writting stream result on console    
                    //Console.WriteLine(ServiceResult);
                    //Console.ReadLine();
                    var parts = Regex.Split(ServiceResult, @"(<KnowledgeItem>[\s\S]+?<\/KnowledgeItem>)").Where(l => l != string.Empty).ToArray();
                    return parts;
                }
            }
        }
    }

    //extract all DP to given concept
    class DataPoints : Model
    {
        int projectId;
        String conceptName;

        //constructor
        public DataPoints(String conceptName)
        {
            host = "dam.ise.bgu.ac.il";
            uri = "/dime/aKontroller.asmx";
            soapAction = "http://tempuri.org/GetDataByConcept";
            projectId = 2;
            this.conceptName = conceptName;
        }

        //get all data items by concept name with projectId=2;
        public string[] GetDataByConcept()
        {
            //Calling CreateSOAPWebRequest method    
            HttpWebRequest request = CreateSOAPWebRequest();
            XmlDocument SOAPReqBody = new XmlDocument();
            //SOAP Body Request
            SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-   instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">  
             <soap:Body>  
                <GetDataByConcept xmlns = ""http://tempuri.org/"">  
                  <projectID>" + projectId + @"</projectID>
                  <conceptName>" + conceptName + @"</conceptName>    
                </GetDataByConcept>  
              </soap:Body>  
            </soap:Envelope>");

            using (Stream stream = request.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }
            //Geting response from request    
            using (WebResponse Serviceres = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                {
                    //reading stream    
                    var ServiceResult = rd.ReadToEnd();
                    //writting stream result on console    
                    //Console.WriteLine(ServiceResult);
                    //Console.ReadLine();
                    var parts = Regex.Split(ServiceResult, @"(<DataPoint>[\s\S]+?<\/DataPoint>)").Where(l => l != string.Empty).ToArray();
                    return parts;
                }
            }
        }
    }

    //represant a DataPoint
    class DataPoint
    {
        String PatientID;
        public DateTime StartTime;
        public DateTime EndTime;
        double Value;

        //constructor - extract the datapoint from string (by tags)
        public DataPoint(String data, double GB, double GA, string timeUnit)
        {
            //patientId
            Match m = Regex.Match(data, @"<PatientID>\s*(.+?)\s*</PatientID>");
            if (m.Success)
            {
                PatientID = m.Groups[1].Value;
            }
            //startTime - adds goodBefor
            m = Regex.Match(data, @"<StartTime>\s*(.+?)\s*</StartTime>");
            if (m.Success)
            {
                string start = m.Groups[1].Value;
                DateTime temp = DateTime.ParseExact(start, "s", null);
                StartTime = addTime(temp, -1 * GB, timeUnit);
            }
            //endTime - adds goodAfter
            m = Regex.Match(data, @"<EndTime>\s*(.+?)\s*</EndTime>");
            if (m.Success)
            {
                string end = m.Groups[1].Value;
                DateTime temp = DateTime.ParseExact(end, "s", null);
                EndTime = addTime(temp, GA, timeUnit);
            }
            //value
            m = Regex.Match(data, @"<Value>\s*(.+?)\s*</Value>");
            if (m.Success)
            {
                Value = double.Parse(m.Groups[1].Value);
            }
        }

        //get value
        public double getValue()
        {
            return Value;
        }

        //add time acording to the time unit
        private DateTime addTime(DateTime dt, double timeToAdd, string timeUnit)
        {
            if (timeUnit.Equals("Days"))
            {
                return dt.AddDays(timeToAdd);
            }
            else if (timeUnit.Equals("Hours"))
            {
                return dt.AddHours(timeToAdd);
            }
            else if (timeUnit.Equals("Milliseconds"))
            {
                return dt.AddMilliseconds(timeToAdd);
            }
            else if (timeUnit.Equals("Minutes"))
            {
                return dt.AddMinutes(timeToAdd);
            }
            else if (timeUnit.Equals("Months"))
            {
                return dt.AddMonths((int)timeToAdd);
            }
            else if (timeUnit.Equals("Seconds"))
            {
                return dt.AddSeconds(timeToAdd);
            }
            else if (timeUnit.Equals("Ticks"))
            {
                return dt.AddTicks((long)timeToAdd);
            }
            else if (timeUnit.Equals("Years"))
            {
                return dt.AddYears((int)timeToAdd);
            }
            else return dt;
        }
    }
}
