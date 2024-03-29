﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Runtime.InteropServices;
using FakeItEasy;
using NUnit.Framework;

namespace EdFi.Db.Deploy.Tests
{
    [TestFixture]
    public abstract class TestFixtureBase
    {
        public static string RootPath { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "C:\\"
            : "/";

        [SetUp]
        public virtual void SetupFixture() { }

        [TearDown]
        public virtual void TearDownFixture() { }

        private Exception _actualException;
        private bool _actualExceptionInspected;

        protected Exception ActualException
        {
            get
            {
                _actualExceptionInspected = true;
                return _actualException;
            }
            set
            {
                _actualExceptionInspected = false;
                _actualException = value;
            }
        }

        [OneTimeSetUp]
        public virtual void RunOnceBeforeAny()
        {
            //Arrange
            try
            {
                Arrange();
            }
            catch (Exception ex)
            {
                var handled = HandleArrangeException(ex);

                if (!handled)
                {
                    throw;
                }
            }

            //Act
            try
            {
                Act();
            }
            catch (Exception ex)
            {
                ActualException = ex;
            }
        }

        [OneTimeTearDown]
        public virtual void RunOnceAfterAll()
        {
            // Make sure exception was inspected.
            if (_actualException != null && !_actualExceptionInspected)
            {
                throw new AssertionException(
                    $"The exception of type '{_actualException.GetType().Name}' was not inspected by the test:\r\n {_actualException}.");
            }
        }

        protected virtual void Arrange() { }

        /// <summary>
        /// Executes the code to be tested.
        /// </summary>
        protected virtual void Act() { }

        protected T Stub<T>()
            where T : class
            => A.Fake<T>();

        protected virtual bool HandleArrangeException(Exception ex) => false;
    }
}
