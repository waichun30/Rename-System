using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class fileClass
    {
        private string name;
        private string location;


        public fileClass(string name,string location)
        {
            this.name = name;
            this.location = location;
        }

        public string nameV
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        public string locationV
        {
            get
            {
                return this.location;
            }

            set
            {
                this.location = value;
            }
        }
    }
}
