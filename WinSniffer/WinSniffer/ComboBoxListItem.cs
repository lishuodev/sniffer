using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer
{
    public class ComboBoxListItem
    {
        private int id;
        private string name = string.Empty;
        public ComboBoxListItem(int id, string sname)
        {
            this.id = id;
            this.name = sname;
        }

        public override string ToString()
        {
            return this.name;
        }

        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}
