using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Vseosvita
{
    class VseosvitaUser
    {
        public string Name { get; }
        public string CSRF { get; private set; }

        public VseosvitaUser(string name)
        {
            Name = name;
        }
    }
}
