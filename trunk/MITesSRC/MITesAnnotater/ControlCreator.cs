using System;
using System.Collections.Generic;
using System.Text;

namespace MitesAnnotater
{
    public interface ControlCreator
    {
        void SetText(string label, int control_id);
    }
}
