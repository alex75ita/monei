﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monei.DataAccessLayer.Interfaces;
using Monei.Entities;
using Monei.MvcApplication.Api;
using Should;

namespace Monei.Test.UnitTest.MvcApplication.Api
{
	[TestClass]
	public class CategoryControllerTest
	{
		[TestMethod, TestCategory("Web")]
		public void Get()
		{
			// Arrange
			CategoryController controller = new CategoryController();

			ICategoryRepository categoryRepository = A.Fake<ICategoryRepository>();
			IList<Category> data = new List<Category>();
			data.Add(new Category() { Id = 1, Name = "Test" });
			A.CallTo(() => categoryRepository.List()).Returns(data);
			controller.CategoryRepository = categoryRepository;

			// Act
			IEnumerable<Category> list = controller.Get();

			// Assert
			list.ShouldNotBeEmpty();
		}


	}
}