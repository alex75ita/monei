﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monei.DataAccessLayer.Interfaces;
using Monei.Entities;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Criterion;

namespace Monei.DataAccessLayer.SqlServer
{
	public class SubcategoryRepository :AbstractRepository<int, Subcategory>, ISubcategoryRepository
	{

		public List<Subcategory> List(int categoryId)
		{
			using (ISession session = OpenSession())
			{
				return session.Query<Subcategory>().Where(s => s.Category.Id == categoryId).ToList();
			}
		}				

	}
}
