﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TracerLib
{
    public class XmlSerializer : ISerialize
    {
        public void SaveTraceResult(TextWriter textWriter, TracerResult traceResult)
        {
            XDocument doc = new XDocument(
                new XElement("TracerApp", from threadTracerResult in traceResult.dThreadTracerResults.Values
                                          select SaveThread(threadTracerResult)
                ));

            using (XmlTextWriter xmlWriter = new XmlTextWriter(textWriter))
            {
                xmlWriter.Formatting = Formatting.Indented;
                doc.WriteTo(xmlWriter);
            }
        }

        private XElement SaveThread(ThreadTracerResult threadTracer)
        {
            return new XElement("Thread",
                new XAttribute("Id", threadTracer.Id),
                new XAttribute("Time", threadTracer.Time.Milliseconds + "ms"),
                from methodTracerResult in threadTracer.lFirstLvlMethodTracersResult
                select SaveMethod(methodTracerResult)
                );
        }

        private XElement SaveMethod(MethodTracerResult methodTracer)
        {
            XElement savedTracedMetod = new XElement("Method",
                new XAttribute("Name", methodTracer.MethodName),
                new XAttribute("Time", methodTracer.Time.Milliseconds + "ms"),
                new XAttribute("Class", methodTracer.ClassName));

            if (methodTracer.lInnerMethodTracerResults.Any())
                savedTracedMetod.Add(from innerMethod in methodTracer.lInnerMethodTracerResults
                                     select SaveMethod(innerMethod));
            return savedTracedMetod;
        }
    }
}
