using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using NewsPublish.Model.Entity;

namespace NewsPublish.Service
{
    /// <summary>
    /// 数据库访问上下文
    /// </summary>
    public class Db : DbContext
    {
        public Db() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder); //base: mother class=DbContext
            optionsBuilder.UseSqlServer("Data Source=LAPTOP-33O7GCQ2; Initial Catalogue=NewsPublish; User Id=tongtong; Password:123456",
                b => b.UseRowNumberForPaging());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public virtual DbSet<Banner> Banner { get; set; }
        public virtual DbSet<NewsClassify> NewsClassify { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsComment> NewsComment { get; set; }
        //protected virtual DbSet<Banner> Banner { get; set; }
        //protected virtual DbSet<NewsClassify> NewsClassify { get; set; }
        //protected virtual DbSet<News> News { get; set; }
        //protected virtual DbSet<NewsComment> NewsComment { get; set; }
    }
}
