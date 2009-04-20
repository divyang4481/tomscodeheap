//========================================================================
//File:     AssertException.cs
//
//Author:   $Author$
//Date:     $LastChangedDate$
//Revision: $Revision$
//========================================================================
//Copyright [2009] [$Author$]
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//========================================================================
          

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ch.froorider.codeheap.Testing
{
    /// <summary>
    /// Contains assertion types for exceptions that are not provided with the standard MSTest assertions.
    /// </summary>
    /// <remarks>
    /// The standard test framework has an Attribute called <see cref="ExpectedExceptionAttribute">ExpectedExceptionAttribute</see>. This attribute has two
    /// main disadvantages:
    /// <para>
    /// 1. The unit test stops at the line which throws the expected exception. If you want to test a method which throws a bunch of exceptions
    /// you must write a test for each exception.
    /// 2. The attribute does not specify exactly where the exception has to be thrown. So if a method call earlier than expected throws
    /// suddenly the same exception, the whole test is still o.k.
    /// </para>
    /// So this class can be used like the common assertions. You can test a method at a sepcific line in the test for a specific exception.
    /// </remarks>
    public static class AssertException
    {
        #region asserts for methods with signature: void MethodName(parameter, ...)

        /// <summary>
        /// Checks to make sure that the input delegate throws a exception of type TException.
        /// <para>
        /// The input delegate must be a method with no parameters and return type void!
        /// </para>
        /// </summary>
        /// <typeparam name="TException">The type of exception expected.</typeparam>
        /// <param name="methodToExecute">The method to execute.</param>
        public static void Throws<TException>(Action methodToExecute) where TException : System.Exception
        {
            try
            {
                methodToExecute();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(TException), "Expected exception of type " + typeof(TException) + " but type of " + e.GetType() + " was thrown instead.");
                return;
            }
            Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
        }

        /// <summary>
        /// Checks to make sure that the input delegate throws a exception of type TException with a specific exception message.
        /// <para>
        /// The input delegate must be a method with no parameters and return type void!
        /// </para>
        /// </summary>
        /// <typeparam name="TException">The type of exception expected.</typeparam>
        /// <param name="expectedMessage">The expected exception message.</param>
        /// <param name="methodToExecute">The method to execute.</param>
        /// <remarks>
        /// This method asserts if the given message and the message of the thrown exception are not equal!
        /// </remarks>
        public static void Throws<TException>(string expectedMessage, Action methodToExecute) where TException : System.Exception
        {
            try
            {
                methodToExecute();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(TException), "Expected exception of type " + typeof(TException) + " but type of " + e.GetType() + " was thrown instead.");
                Assert.AreEqual(expectedMessage, e.Message, "Expected exception with a message of '" + expectedMessage + "' but exception with message of '" + e.Message + "' was thrown instead.");
                return;
            }
            Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
        }

        /// <summary>
        /// Checks to make sure that the input delegate throws a exception of type TException with a specific exception message.
        /// <para>
        /// The input delegate must be a method with no parameters and return type void!
        /// </para>
        /// </summary>
        /// <typeparam name="TException">The type of exception expected.</typeparam>
        /// <typeparam name="T">The type of the argument, that is passed to the method.</typeparam>
        /// <param name="methodToExecute">The method to execute.</param>
        /// <param name="argument">The argument.</param>
        /// <remarks>
        /// This method asserts if the given message and the message of the thrown exception are not equal!
        /// </remarks>
        public static void Throws<TException, T>(Action<T> methodToExecute, T argument) where TException : System.Exception
        {
            try
            {
                methodToExecute(argument);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(TException), "Expected exception of type " + typeof(TException) + " but type of " + e.GetType() + " was thrown instead.");
                return;
            }
            Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
        }


        #endregion

        #region asserts for methods with signature: returnValue MethodName(parameter, ....)

        /// <summary>
        /// Checks to make sure that the input delegate throws a exception of type TException with a specific exception message.
        /// <para>
        /// The input delegate must be a method with ONE parameter and return type!
        /// </para>
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T">The type of the input argument</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="methodToExecute">The method to execute.</param>
        /// <param name="argument">The argument to input.</param>
        public static void Throws<TException, T, TResult>(Func<T, TResult> methodToExecute, T argument)
            where TException : System.Exception
        {
            try
            {
                methodToExecute(argument);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(TException), "Expected exception of type " + typeof(TException) + " but type of " + e.GetType() + " was thrown instead.");
                return;
            }
            Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
        }

        #endregion

        #region asserts for constructors

        /// <summary>
        /// Checks the default constructor.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="typeToCreate">The type to create.</param>
        public static void Throws<TException>(Type typeToCreate)
            where TException : System.Exception
        {
            try
            {
                Activator.CreateInstance(typeToCreate);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.InnerException.GetType() == typeof(TException), "Expected exception of type " + typeof(TException) + " but type of " + e.InnerException.GetType() + " was thrown instead.");
                return;
            }
            Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
        }

        /// <summary>
        /// Checks the constructor which takes one argument.
        /// </summary>
        /// <typeparam name="TException">The type of the exception to check</typeparam>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="typeToCreate">The type to create.</param>
        /// <param name="argument">The argument itself.</param>
        public static void Throws<TException, T>(Type typeToCreate, T argument)
            where TException : System.Exception
        {
            try
            {
                object[] arguments = new object[1];
                arguments[0] = argument;
                Activator.CreateInstance(typeToCreate, arguments);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.InnerException.GetType() == typeof(TException), "Expected exception of type " + typeof(TException) + " but type of " + e.InnerException.GetType() + " was thrown instead.");
                return;
            }
            Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
        }

        #endregion
    }
}
