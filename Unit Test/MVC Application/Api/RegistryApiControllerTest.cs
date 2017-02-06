﻿using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Monei.DataAccessLayer.Interfaces;
using Monei.Entities;
using Monei.MvcApplication.Api;
using Monei.MvcApplication.Api.PostDataObjects;
using NUnit.Framework;
using Should;

namespace Monei.Test.UnitTest.MvcApplication.Api
{
    [TestFixture, Category("Web API"), Category("Registry")]
    internal class RegistryApiControllerTest : ApiControllerTestBase<RegistryApiController>
    {
        private RegistryApiController controller;

        [SetUp]
        public void SetUp()
        {
            controller = CreateController();

            Account account = new Account() {
                Id=1,
                Guid = Guid.NewGuid(),
                Username = "test",
            };
            A.CallTo(() => controller.AccountRepository.Read(A.Dummy<string>())).Returns(account);
        }

        [Test]
        public void Create_when_DateIsNotDefined_should_ThrowAnException()
        {
            RegistryNewRecordPostData postData = new RegistryNewRecordPostData()
            {
                Amount = 1,
                Operation = OperationType.Income,
                CategoryId = 1,
                Note = "aaa",
            };
            
            AssertExceptionIsRaised(() => controller.Create(postData), new Exception("Date is not defined"));
        }
        
        [Test]
        [TestCase(OperationType.Income)]
        [TestCase(OperationType.Outcome)]
        public void Create_when_AmountIsZeroAndOperationTypeIsNotTransfer_should_ThrowAnException(OperationType operation)
        {
            RegistryNewRecordPostData postData = new RegistryNewRecordPostData()
            {
                Date = DateTime.Now,
                Operation = operation,
                Amount = 0,                
                CategoryId = 1,
                Note = "aaa",
            };

            AssertExceptionIsRaised(() => controller.Create(postData), new Exception("Amount is zero"));
        }

        [Test]
        public void Create_when_CategoryIsNotDefined_should_ThrowAnException()
        {
            RegistryRecord record = new RegistryRecord();

            RegistryNewRecordPostData postData = new RegistryNewRecordPostData()
            {
                Date = DateTime.Now,
                Operation = OperationType.Income,
                Amount = 1,
                CategoryId = 0,
                Note = "aaa",
            };
            
            AssertExceptionIsRaised(() => controller.Create(postData), new Exception("Category is not defined"));            
        }

        [Test]
        public void Create_should_CallRepositoryWithRightData()
        {
            RegistryRecord record = new RegistryRecord();

            RegistryNewRecordPostData postData = new RegistryNewRecordPostData()
            {
                Date = DateTime.Now,
                Amount = 1m,
                Operation = OperationType.Income,
                CategoryId = 1,
                Note = "aaa",
            };

            // execute
            controller.Create(postData);

            A.CallTo( () => controller.RegistryRepository.Create(
                A<RegistryRecord>
                .That.Matches(r =>
                r.Date == postData.Date
                && r.OperationType == postData.Operation
                && r.Amount == postData.Amount
                && r.Category.Id == postData.CategoryId
            ))).MustHaveHappened();            
        }
        
    }
}
