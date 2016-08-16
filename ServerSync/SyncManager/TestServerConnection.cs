using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync
{
    public class TestServerConnection
    {
        public TestServerConnection()
        {

        }

        internal Task<bool> BeginTest()
        {
            return new Task<bool>(() => { return true; });
        }
    }
}
