using System;
using System.Collections.Generic;
using System.Text;
using NewsPublish.Model.Response;
using NewsPublish.Model.Request;
using NewsPublish.Model.Entity;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace NewsPublish.Service
{
    public class CommentService
    {
        private Db _db;
        private NewsService _newsService;
        public CommentService(Db db, NewsService newsService)
        {
            this._db = db;
            this._newsService = newsService;
        }

        /// <summary>
        /// Add a new comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public ResponseModel AddComment(AddComment comment)
        {
            var news = _newsService.GetOneNews(comment.NewsId);
            if (news == null)
                return new ResponseModel { code = 0, result = "News " + comment.NewsId + "does not exist." };
            var cmt = new NewsComment { AddTime = DateTime.Now, NewsId = comment.NewsId, Content = comment.Content };
            _db.NewsComment.Add(cmt);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel
                {
                    code = 200,
                    result = "News comment is created successfully",
                    data = new  //due to the fact that Frontend will use JSON to pass data, and variable will start with small case word. So decide to not use our CommentModel
                    {
                        content = comment.Content,
                        floor = "#" + news.data.CommentCount + 1, //将在运行时解析，Dynamic
                        addTime = DateTime.Now
                    }
                    //data = new NewsCommentModel
                    //{
                    //    NewsId = comment.NewsId,
                    //    Content = comment.Content,
                    //    Floor = "#" + news.data.CommentCount + 1, //将在运行时解析，Dynamic
                    //    AddTime = DateTime.Now
                    //}
                };
            }
            return new ResponseModel { code = 0, result = "Fail to create news comment for News " + comment.NewsId };
        }
        /// <summary>
        /// Delete a news comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public ResponseModel DeleteComment(int commentId)
        {
            var cmt = _db.NewsComment.Find(commentId);
            if (cmt == null)
                return new ResponseModel { code = 0, result = "Comment " + commentId + "does not exist." };
            _db.NewsComment.Remove(cmt);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel
                {
                    code = 200,
                    result = "Comment " + commentId + "is deleted successfully",
                };
            return new ResponseModel { code = 0, result = "Fail to delete comment" + commentId };

        }
        /// <summary>
        /// Get comment list
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel GetCommentList(Expression<Func<NewsComment, bool>> where)
        {
            var comments = _db.NewsComment
                .Include("News")
                .Where(where)
                .OrderBy(c => c.AddTime)
                .ToList();
            var response = new ResponseModel
            {
                code = 200,
                result = "News comment list is found",
                data = new List<NewsCommentModel>()
            };
            int floor = 1;
            foreach (var comment in comments)
            {
                response.data.Add(new NewsCommentModel
                {
                    Id = comment.Id,
                    AddTime = DateTime.Now,
                    Content = comment.Content,
                    NewsId = comment.NewsId,
                    Remark = comment.Remark,
                    Floor = "#" + floor
                });
                floor += 1;  //floor++;
            }
            response.data.Reverse();

            return response;
        }
    }
}
