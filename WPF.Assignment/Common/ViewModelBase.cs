using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF.Assignment
{
    public abstract class ViewModelBase
    {
        public EventHandler OnShowEvent;

        protected virtual void ShowEntity(EventArgs e)
        {
            EventHandler evt = OnShowEvent;
            if (evt != null)
                evt(this, e);
        }
    }
}
