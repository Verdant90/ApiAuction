using ApiAuctionShop.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ApiAuctionShop.Tests
{
    public class StringCipherTest
    {
        [Fact]
        public void StringCipherTestPass()
        {
            string test = "test";
            string hash = "hash";
            string encryptedstring = StringCipher.Encrypt(test, hash);
            string[] decryptedstring = StringCipher.Decrypt(encryptedstring, hash);
            Assert.Equal(test, decryptedstring[0]);
        }
    }
}
