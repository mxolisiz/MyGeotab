using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGeotabClassLibrary;

namespace MyGeotabTests
{
    [TestClass]
    public class APITests
    {

        MyGeotab myGeotab;
        string database = "";
        string password = "";
        string username = "";
        string server = "";

        [TestInitialize]
        public void TestInit()
        {
            myGeotab = new MyGeotab(new MyCredentials
            {
                Password = password,
                UserName = username,
                Server = server,
                Database = database,
                Authenticate = false
            });
        }

        [TestMethod]
        public void TestInitializeWithAuth_ShouldAuthUser()
        {
            //Arrange

            //Act
            var user = myGeotab.GetLoggedInUser();
            //Assert
            Assert.AreEqual(username, user.Name.ToLower());
        }

        [TestMethod]
        public void TestInitializeWithNoAuth_ShouldAuthUser()
        {
            //Arrange
            myGeotab = new MyGeotab(new MyCredentials
            {
                Password = password,
                UserName = username,
                Server = server,
                Database = database,
                Authenticate = false
            });
            //Act
            var user = myGeotab.GetLoggedInUser();
            //Assert
            Assert.AreEqual(username, user.Name.ToLower());
        }



        [TestMethod]
        public void Test_UTCDateConvert_ShouldReturnCurrentDate()
        {
            //Arrange
            DateTime date = DateTime.UtcNow;
            //Act
            var result = myGeotab.ToUserDate(date);
            //Assert
            Assert.AreEqual(date.ToLocalTime(), result);
        }

    }
}
