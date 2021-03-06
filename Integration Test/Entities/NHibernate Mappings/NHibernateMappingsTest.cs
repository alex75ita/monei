﻿using System;
using System.Reflection;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Monei.Test.IntegrationTest.Entities.NHibernateMappings
{
    [TestFixture, Category("NHibernate")]
    public class NHibernateMappingsTest
    {
        [Test]
        public void AccountMappingTest()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Monei.Entities.Account));

            NHibernate.Cfg.Configuration configuration = new NHibernate.Cfg.Configuration();
            configuration.Configure();

            Assert.IsNotEmpty(configuration.ClassMappings, "None class mapping loaded.");

            foreach(var mapping in configuration.ClassMappings)
            {
                Type type = mapping.MappedClass;
            }

            SchemaExport export = new SchemaExport(configuration);
            export.SetOutputFile("NHibernate mappings.sql");
            //export.Create()
            export.Execute(true, false, false);
        }
    }
}
