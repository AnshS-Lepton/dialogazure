using BusinessLogics;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartInventory.Settings
{
    public class EmailSettings
    {
        public static List<EmailSettingsModel> AllEmailSettings { get; set; }
        public static void InitializeEmailSettings()
        {
            AllEmailSettings = new BLMisc().GetAllEmailSettings();
        }
    }
}