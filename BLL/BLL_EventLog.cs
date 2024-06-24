﻿using BE;
using DAL;
using SERVICES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL
{
    public class BLL_EventLog
    {
        public static void RegisterEventLog(string username, string activity)
        {
            //HACE FALTA CREARLO ACA??? O ANTES??? USERNAME -> SESSION MANAGER ¿?desde BE???
            BE_EventLog eventlog = new BE_EventLog(username, activity);
            DAL_EventLog.RegisterEventLog(eventlog);
        }

        public static DataTable GetEventLog()
        {
            DataTable table = DAL_EventLog.GetEventLog(SessionManager.GetInstance.User.Language);
            return table;
        }

        public static DataTable GetActivityLevel(string language)
        {
            return DAL_EventLog.GetActivityLevel(language);
        }
    }
}
