﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Copyright Konstantin Badanin.

namespace ArticleCommentary
{
    public class ArticleCommentsTree
        //Класс объекта модели данных: один экземпляр на рантайм.
    {
        private static ArticleCommentsTree instance;
        private ArticleCommentsTree(ref List<ArticleNode> tree, ref List<User> users)
        {
            Users = users;
            Tree = tree;
        }
        public static ArticleCommentsTree GetInstance()
        {
            if (instance == null)
            {
                throw (new Exception());
            }
            else
            {
                return instance;
            }
        }
        public static ArticleCommentsTree GetInstance(ref List<ArticleNode> tree, ref List<User> users)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    instance = new ArticleCommentsTree(ref tree, ref users);
                }
            }
            return instance;
        }
        private static readonly object syncRoot = new Object();
        private static readonly object locker = new Object();
        public object Locker
        {
            get
            {
                return locker;
            }
        }
        public List<User> Users{ get; private set; }
        public List<ArticleNode> Tree { get; private set; }

        public static bool AddByParentId(ref CommentNode comment)
            //Возврат: true добавлен коммент в дерево, false не добавлен.
        {
            if (comment == null) throw new ArgumentNullException(paramName: nameof(comment));
            foreach (ArticleNode article in instance.Tree)
            {
                if (article.LeftComment == null)
                {
                    if ((comment.Parent == null) && (comment.Article == article.Id))
                    {
                        article.SetLeftComment(ref comment);
                        return true;
                    }
                }
                else
                {
                    return article.LeftComment.RecInsertByParentId(ref comment);
                }
                if (article.RightComment == null)
                {
                    if ((comment.Parent == null) && (comment.Article == article.Id))
                    {
                        article.SetRightComment(ref comment);
                        return true;
                    }
                }
                else
                {
                    return article.RightComment.RecInsertByParentId(ref comment);
                }
            }
            return false;
        }
        public static bool DeleteCommentById(int comId)
        {
            throw new NotImplementedException();
        }
    }
}