using Administration.Models;
using CSGenio;
using CSGenio.framework;
using GenioServer.framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using IConfigurationManager = CSGenio.config.IConfigurationManager;

namespace Administration.Controllers
{
    public class ErrorLogController(IConfigurationManager configManager) : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            var appId = FromQuery("appId");
            var model = new ErrorLogModel();
            model.ResultMsg = "";
            model.ErrorLog = "";
            if (string.IsNullOrEmpty(appId)) appId = "Admin";

            model.Applications = ClientApplication.Applications.ToList();

            string path = "";

            //tentar ler do configuracoes.xml a localização do errlog
            try
            {
                var conf = configManager.GetExistingConfig();

                string pathApp = conf.GetPath(appId).pathApp;
                if (String.IsNullOrEmpty(pathApp))
                {
                    model.ResultMsg = Resources.Resources.O_CAMINHO_PARA_A_APL47115;
                    return Json(model);
                }
                path = Path.Combine(pathApp, @"temp\errlog.txt");                    

            }
            catch (Exception)
            {
                model.ResultMsg = Resources.Resources.ERRO_AO_LER_O_FICHEI14729;
                return Json(model);
            }

            try
            {
                if(System.IO.File.Exists(path))
                {
                // Open the text file using a stream reader.
                var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (StreamReader sr = new StreamReader(fs, true))
                {
                    // Read the stream to a string, and write the string to the console.
                    String fileReaded = sr.ReadToEnd();
                    model.ErrorLog = fileReaded;
                }
            }
                
            }
            catch (Exception)
            {
                model.ResultMsg = Resources.Resources.NAO_FOI_POSSIVEL_ENC08159 + "<br />" + path;
            }

            return Json(model);
        }
    }
}