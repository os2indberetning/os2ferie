﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OS2Indberetning.Startup))]

namespace OS2Indberetning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
            ConfigureAuth(app);
        }
    }
}
