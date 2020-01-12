using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPublish.Model.Request
{
    public class AddComment
    {
        public int NewsId { get; set; }
        public string Content { get; set; }
    }
}
