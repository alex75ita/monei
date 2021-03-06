﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monei.DataAccessLayer.Interfaces;
using Monei.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Monei.DataAccessLayer.SqlServer
{
    public class CurrencyRepository :RepositoryBase<int, Currency>, ICurrencyRepository
    {
        public const string EUR_CODE = "EUR";

        public CurrencyRepository(ISessionFactoryProvider sessionFactoryProvider) : base(sessionFactoryProvider)
        {
        }

        public Currency Read(string code)
        {
            using (ISession session = OpenSession())
            {
                return session.Query<Currency>().Where(c => c.Code == code).FirstOrDefault();
            }
        }

        public new Currency Create(Currency currency)
        {
            throw new NotImplementedException();
        }

        public void Delete(string code)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class AA<Tkey, Tentity> : RepositoryBase<Tkey, Tentity>
    {
        public AA(ISessionFactoryProvider sessionFactoryProvider) : base(sessionFactoryProvider)
        {
        }
    }
}
