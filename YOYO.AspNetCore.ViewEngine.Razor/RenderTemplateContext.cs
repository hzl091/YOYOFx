﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YOYO.Mvc;

namespace YOYO.AspNetCore.ViewEngine.Razor
{
    public class RenderTemplateContext : IDisposable
    {
        public RenderTemplateContext()
        {
            IsRenderLayout = false;
            Writer = new StringWriter();
        }

        public Type ModelType { set; get; }
        public object Model { set; get; }

        public DynamicDictionary ViewBag { set; get; }


        public bool IsRenderLayout { set; get; }


        public string Result {
            get {
                return Writer.ToString(); ;
            }
        }


        public override string ToString()
        {
            return Result;
        }

      

        public TextWriter Writer {  set; get; }


        public string TemplateName { set; get; }

        public string Path { set; get; }

        public CodeGenerateResult GenerateResult { set; get; }

        public CompileResult BuildResult { set; get; }


        public void Dispose()
        {
            
            this.Writer.Flush();
            this.Writer.Dispose();
        }


    }
}
