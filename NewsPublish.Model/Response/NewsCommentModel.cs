using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPublish.Model.Response
{
    public class NewsCommentModel
    {
        public int Id { get; set; }
        public int NewsId { get; set; }
        public string Content { get; set; }
        public DateTime AddTime { get; set; }
        public string Remark { get; set; }  
        public string Floor { get; set; }
    }
}
