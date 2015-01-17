﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monei.DataAccessLayer.Filters;
using Monei.DataAccessLayer.Interfaces;

namespace Core
{
    public class SubcategoryManager
    {
		public IRegistryRepository RegistryRepository { get; set; }
		//public ICategoryRepository CategoryRepository { get; set; }
		public ISubcategoryRepository SubcategoryRepository { get; set; }

		/// <summary>
		/// Check if Subcategory is used in Registry.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public bool IsUsed(int userId, int subcategoryId)
		{
			return RegistryRepository.IsSubcategoryUsed(subcategoryId);
		}

    }
}
