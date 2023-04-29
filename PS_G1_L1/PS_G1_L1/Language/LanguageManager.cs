using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Resources;
using System.Text;

namespace PS_G1_L1.Language
{
    public static class LanguageManager
    {
        private static readonly ResourceManager resourceManagerEn = new ResourceManager("PS_G1_L1.Language.Resources", Assembly.GetExecutingAssembly());

        public static string GetString(string key)
        {
            string text = resourceManagerEn.GetString(key);
            if (text == string.Empty)
            {
                return "Ressource non trouvé";
            }
            else
            {
                return text!;
            }
        }
    }
}
