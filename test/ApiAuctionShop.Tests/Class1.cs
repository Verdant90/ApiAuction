using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ApiAuctionShop.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Class1
    {
        [Fact]
        public void PassingTest()
        {
            int a = 2;
            int b = 2;

            int sum = a + b;

            Assert.Equal(4,sum);
        }
    }
}
