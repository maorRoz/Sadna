using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.AdminView;

namespace SystemViewTests.AdminViewApiTest
{
    [TestClass]
    public class RemoveUserTests
    {
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IAdminDL> adminDbMocker;
    }
}
