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
    public class NewsService
    {
        private Db _db;
        public NewsService(Db db)
        {
            this._db = db;
        }
        /// <summary>
        /// Add NewsClassify
        /// </summary>
        /// <param name="newsClassify"></param>
        /// <returns></returns>
        public ResponseModel AddNewsClassify(AddNewsClassify newsClassify)
        {
            var exist = _db.NewsClassify.FirstOrDefault(c => c.Name == newsClassify.Name) != null;
            if (exist)
                return new ResponseModel { code = 0, result = "The newsClassify already exists." };
            var classify = new NewsClassify { Name = newsClassify.Name, Sort = newsClassify.Sort, Remark = newsClassify.Remark };
            _db.NewsClassify.Add(classify);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "NewsClassify is created successfully" };
            return new ResponseModel { code = 0, result = "Fail to create new NewsClassify" };
        }
        /// <summary>
        /// Get a newsClassify(use ID)
        /// </summary>
        /// <param name="NewsClassifyID"></param>
        /// <returns></returns>
        public ResponseModel GetOneNewsClassify(int NewsClassifyID)
        {
            var newsClassify = _db.NewsClassify.Find(NewsClassifyID);
            if (newsClassify is null)
                return new ResponseModel { code = 0, result = "The newsClassify does not exist." };
            var classify = new NewsClassifyModel { Id = newsClassify.Id, Name = newsClassify.Name, Sort = newsClassify.Sort, Remark = newsClassify.Remark };
            return new ResponseModel { code = 200, result = "NewsClassify " + classify.Name + "exists.", data = classify };
        }
        /// <summary>
        /// Get a newsClassify(use condition)
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        private NewsClassify GetOneNewsClassify(Expression<Func<NewsClassify, bool>> where)
        {
            return _db.NewsClassify.FirstOrDefault(where);
        }
        /// <summary>
        /// Edit a newsClassify
        /// </summary>
        /// <param name="newsClassify"></param>
        /// <returns></returns>
        public ResponseModel EditNewsClassify(EditNewsClassify newsClassify)
        {
            var classify = this.GetOneNewsClassify(c => c.Id == newsClassify.Id); //use condition to get
            if (newsClassify is null)
                return new ResponseModel { code = 0, result = "The newsClassify " + newsClassify.Name + "does not exist." };
            classify.Name = newsClassify.Name;
            classify.Sort = newsClassify.Sort;
            classify.Remark = newsClassify.Remark;
            _db.NewsClassify.Update(classify);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "NewsClassify" + newsClassify.Name + " is updated successfully" };
            return new ResponseModel { code = 0, result = "Fail to update NewsClassify" + newsClassify.Name };
        }
        /// <summary>
        /// Get a newsClassify list
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetNewsClassifyList()
        {
            var classifys = _db.NewsClassify.OrderByDescending(c => c.Sort).ToList();
            var response = new ResponseModel { code = 200, result = "Get news classify list successfully" };
            response.data = new List<NewsClassifyModel>();
            foreach (var classify in classifys)
            {
                response.data.Add(new NewsClassifyModel
                {
                    Id = classify.Id,
                    Name = classify.Name,
                    Sort = classify.Sort,
                    Remark = classify.Remark
                });
            }
            return response;
        }
        /// <summary>
        /// Add news
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        public ResponseModel AddNews(AddNews news)
        {
            var classify = this.GetOneNewsClassify(c => c.Id == news.NewsClassifyId);
            if (classify == null)
                return new ResponseModel { code = 0, result = "The news does not exist" };
            var n = new News
            {
                NewsClassifyId = news.NewsClassifyId,
                Title = news.Title,
                Contents = news.Contents,
                Image = news.Image,
                PublishDate = news.PublishDate,
                Remark = news.Remark,
            };
            _db.News.Add(n);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "News is created successfully" };
            return new ResponseModel { code = 0, result = "Fail to create new News" };
        }
        /// <summary>
        /// Get a News
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel GetOneNews(int id)
        {
            var news = _db.News.Include("NewsClassify")
                //NewsModel has ClassifyName and CommentCount, which cannot be found in News class, so use "Include"
                //"NewsClassify" is from News class: public virtual NewsClassify NewsClassify { get; set; }
                .Include("NewsComment")
                //"NewsComment" is from News class: public virtual ICollection<NewsComment> NewsComment { get; set; }
                .FirstOrDefault(c => c.Id == id);
            if (news == null)
                return new ResponseModel { code = 0, result = "The news does not exist" };
            return new ResponseModel
            {
                code = 200,
                result = "Successfully get the news" + news.Id,
                data = new NewsModel
                {
                    Id = news.Id,
                    ClassifyName = news.NewsClassify.Name,
                    Contents = news.Contents,
                    CommentCount = news.NewsComment.Count,
                    Image = news.Image,
                    PublishDate = news.PublishDate.ToShortDateString(),
                    Remark = news.Remark,
                    Title = news.Title
                }
            };

        }
        /// <summary>
        /// Delete a news
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteOneNews(int id)
        {
            var news = _db.News.Find(id);
            if (news == null)

                return new ResponseModel { code = 0, result = "The news" + id + " does not exist" };
            _db.News.Remove(news);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "News" + id + " is deleted successfully" };
            return new ResponseModel { code = 0, result = "Fail to delete News" + id };
        }
        /// <summary>
        /// 分页查询新闻
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel NewsPageQuery(int pageSize, int pageIndex, out int total, List<Expression<Func<News, bool>>> where)
        {
            var list = _db.News.Include("NewsClassify").Include("NewsComment");
            foreach (var item in where)
            {
                list = list.Where(item);
            }
            total = list.Count();

            var pageData = list.OrderByDescending(c => c.PublishDate)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .ToList();

            var response = new ResponseModel
            {
                code = 200,
                result = "NewsPage data is extracted successfully."
            };
            response.data = new List<NewsModel>();
            foreach (var newsmodel in pageData)
            {
                response.data.Add(new NewsModel
                {
                    Id = newsmodel.Id,
                    ClassifyName = newsmodel.NewsClassify.Name,
                    CommentCount = newsmodel.NewsComment.Count,
                    Contents = newsmodel.Contents,
                    Image = newsmodel.Image,
                    PublishDate = newsmodel.PublishDate.ToString("yyyy-mm-dd"),
                    Remark = newsmodel.Remark,
                    Title = newsmodel.Title
                });
            }
            return response;
        }
        /// <summary>
        /// Query the list of news
        /// </summary>
        /// <param name="where"></param>
        /// <param name="countTop"></param>
        /// <returns></returns>
        public ResponseModel GetNewsList(Expression<Func<News, bool>> where, int topCount)
        {
            var list = _db.News.Include("NewsClassify").Include("NewsComment")
                .Where(where).OrderByDescending(c => c.PublishDate)
                .Take(topCount);
            if (list == null)
                return new ResponseModel
                {
                    code = 0,
                    result = "Fail to get News list."
                };
            var response = new ResponseModel
            {
                code = 200,
                result = "News list is extracted successfully."
            };
            response.data = new List<NewsModel>();
            foreach (var newsmodel in list)
            {
                response.data.Add(new NewsModel
                {
                    Id = newsmodel.Id,
                    ClassifyName = newsmodel.NewsClassify.Name,
                    CommentCount = newsmodel.NewsComment.Count,
                    Contents = newsmodel.Contents.Length > 50 ? newsmodel.Contents.Substring(0, 50) : newsmodel.Contents, //in case to many contents show in the index.html
                    Image = newsmodel.Image,
                    PublishDate = newsmodel.PublishDate.ToString("yyyy-mm-dd"),
                    Remark = newsmodel.Remark,
                    Title = newsmodel.Title
                });
            }
            return response;
        }
        /// <summary>
        /// Get latest comment news list
        /// </summary>
        /// <param name="where"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public ResponseModel GetNewCommentNewsList(Expression<Func<News, bool>> where, int topCount)
        {
            var newsIds = _db.NewsComment
                .OrderByDescending(c => c.AddTime)
                .GroupBy(c => c.NewsId) //group all comment by NewsId, so one newsId only select the latest comment
                .Select(c => c.Key) //key is NewsId
                .Take(topCount);
            var list = _db.News.Include("NewsClassify").Include("NewsComment")
                .Where(c => newsIds.Contains(c.Id))
                .OrderByDescending(c => c.PublishDate);
            if (list == null)
                return new ResponseModel
                {
                    code = 0,
                    result = "Fail to get new comment news list."
                };
            var response = new ResponseModel
            {
                code = 200,
                result = "New comment News list is extracted successfully."
            };
            response.data = new List<NewsModel>();
            foreach (var newsmodel in list)
            {
                response.data.Add(new NewsModel
                {
                    Id = newsmodel.Id,
                    ClassifyName = newsmodel.NewsClassify.Name,
                    CommentCount = newsmodel.NewsComment.Count,
                    Contents = newsmodel.Contents.Length > 50 ? newsmodel.Contents.Substring(0, 50) : newsmodel.Contents, //in case to many contents show in the index.html
                    Image = newsmodel.Image,
                    PublishDate = newsmodel.PublishDate.ToString("yyyy-mm-dd"),
                    Remark = newsmodel.Remark,
                    Title = newsmodel.Title
                });
            }
            return response;
        }
        /// <summary>
        /// Search one news
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel GetSearchOneNews(Expression<Func<News, bool>> where)
        {
            var news = _db.News.Where(where).FirstOrDefault();
            if (news == null)
                return new ResponseModel { code = 0, result = "News search failed" };
            return new ResponseModel
            {
                code = 200,
                result = "Search News successfully",
                data = news.Id
            };
        }
        /// <summary>
        /// Get news count
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel GetNewsCount(Expression<Func<News, bool>> where)
        {
            var count = _db.News.Where(where).Count();
            return new ResponseModel
            {
                code = 200,
                result = "Get news count successfully",
                data = count
            };
        }
        /// <summary>
        /// Get recomment news list (same news classify)
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public ResponseModel GetRecommentNewsList(int newsId)
        {
            var news = _db.News.FirstOrDefault(c=>c.Id==newsId);
            if (news == null)
                return new ResponseModel { code = 0, result = "News does not exist" };
            var newsList = _db.News.Include("NewsClassify")
                .Where(c => c.NewsClassifyId == news.NewsClassifyId && c.Id != news.Id)
                .OrderByDescending(c => c.PublishDate)
                .OrderByDescending(c => c.NewsComment.Count)
                .Take(6)
                .ToList();
            var response = new ResponseModel
            {
                code = 200,
                result = "Recommend news list is extracted successfully."
            };
            response.data = new List<NewsModel>();
            foreach (var newsmodel in newsList)
            {
                response.data.Add(new NewsModel
                {
                    Id = newsmodel.Id,
                    ClassifyName = newsmodel.NewsClassify.Name,
                    CommentCount = newsmodel.NewsComment.Count,
                    Contents = newsmodel.Contents.Length > 50 ? newsmodel.Contents.Substring(0, 50) : newsmodel.Contents, //in case to many contents show in the index.html
                    Image = newsmodel.Image,
                    PublishDate = newsmodel.PublishDate.ToString("yyyy-mm-dd"),
                    Remark = newsmodel.Remark,
                    Title = newsmodel.Title
                });
            }
            return response;
        }
    }

}
