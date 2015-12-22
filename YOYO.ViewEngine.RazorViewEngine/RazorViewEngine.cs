﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using YOYO.Mvc;
using YOYO.Owin;

namespace YOYO.ViewEngine.RazorViewEngine
{
    public class RazorViewEngine : IViewEngine
    {
        private IRazorEngineService Service = null;


        private AppDomain createSandboxFunc()
        {
            var ev = new Evidence();
            ev.AddHostEvidence(new Zone(SecurityZone.Internet));
            var permissionSet = SecurityManager.GetStandardSandbox(ev);
            // We have to load ourself with full trust 
            var razorEngineAssembly = typeof(RazorEngineService).Assembly.Evidence.GetHostEvidence<StrongName>();
            var razorAssembly = typeof(RazorTemplateEngine).Assembly.Evidence.GetHostEvidence<StrongName>();
            var appDomainSetup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase };
            var sandbox = AppDomain.CreateDomain("Sandbox", null, appDomainSetup, permissionSet, razorEngineAssembly, razorAssembly);
            return sandbox;
        }


        public RazorViewEngine()
        {
            var configCreator = new YOYOConfigCreator();
            this.Service = IsolatedRazorEngineService.Create(configCreator, createSandboxFunc);
        }

        ~RazorViewEngine()
        {
            this.Service.Dispose();
        }

        public string ExtensionName
        {
            get
            {
                return ".cshtml";
            }
        }



        private TemplateServiceConfiguration configFunc()
        {
            var config = new TemplateServiceConfiguration();

            return config;
        }



        public string RenderView(IOwinContext context, string viewName, object model)
        {
            string result = string.Empty;

            string templatePath = HostingEnvronment.GetMapPath(viewName);
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("not found view template . " + viewName);

            using (StreamReader reader = new StreamReader(new FileStream(templatePath, FileMode.Open,FileAccess.Read), Encoding.UTF8))
            {
                string templateContent = reader.ReadToEnd();

                result = Service.RunCompile(templateContent, viewName, null, model, null);
            }

            return result;
            
            
        }
    }

    [Serializable]
    public class YOYOConfigCreator : IsolatedRazorEngineService.IConfigCreator
    {
        public YOYOConfigCreator()
        {

        }


        public ITemplateServiceConfiguration CreateConfiguration()
        {
            var config = new TemplateServiceConfiguration();

            return config;
        }
    }


}
