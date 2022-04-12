using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Helper
{
    public class AesModel
    {
        public string KeyName { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public override string ToString()
        {
            return KeyName;
        }
    }
}
