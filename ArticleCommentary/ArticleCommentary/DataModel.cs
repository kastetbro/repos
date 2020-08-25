﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//copyright Konstantin Badanin
namespace ArticleCommentary
{
    public class ArticleNode:Article
        //Модель-список статей, у каждой статьи список "узлов" комментариев.
    {
        private CommentNode rightComment;
        private CommentNode leftComment;
        public CommentNode RightComment
        {
            get
            {
                return rightComment;
            }
            private set
            {
                rightComment = value;
            }
        }
        public CommentNode LeftComment
        {
            get
            {
                return leftComment;
            }
            private set
            {
                leftComment = value;
            }
        }
        public void SetRightComment(ref CommentNode comment)
        {
            RightComment = comment;
        }
        public void SetLeftComment(ref CommentNode comment)
        {
            LeftComment = comment;
        }
        public ArticleNode(int id, string title, string text) : base(id, title, text)
        {
            RightComment = LeftComment = null;
        }
        public ArticleNode(Article arg) : base(arg)
        {
            if (arg == null) throw (new NullReferenceException());
            RightComment = LeftComment = null;
        }
    }
    public class CommentNode : Comment
    //"Узел" комментариев- элемент бинарного дерева.
    {
        private CommentNode derivedLeftCommentNode;
        private CommentNode derivedRightCommentNode;

        public CommentNode DerivedLeftCommentNode
        {
            get
            {
                return derivedLeftCommentNode;
            }
            private set
            {
                derivedLeftCommentNode = value;
            }
        }

        public CommentNode DerivedRightCommentNode
        {
            get
            {
                return derivedRightCommentNode;
            }
            private set
            {
                derivedRightCommentNode = value;
            }
        }

        public void SetDerivedLeftCommentNode(ref CommentNode comment)
        {
            DerivedLeftCommentNode = comment;
        }
        public void SetDerivedRightCommentNode(ref CommentNode comment)
        {
            DerivedRightCommentNode = comment;
        }
        public CommentNode(int id, string text, int user, int article, int? parent) : base(id, text, user, article, parent)
        {
            DerivedRightCommentNode = DerivedLeftCommentNode = null;
        }
        public CommentNode(Comment arg) : base(arg)
        {
            if (arg == null) throw (new NullReferenceException());
            DerivedRightCommentNode = DerivedLeftCommentNode = null;
        }
        public List<string> GetAllDerivedComments()
        //Возвращает список текстов наследованных комментариев.
        {
            List<string> DerivedComments = new List<string>();
            if (this == null) return DerivedComments;
            if (DerivedLeftCommentNode != null)
            {
                int IdToFind = DerivedLeftCommentNode.UserId;
                DerivedComments.Add("Comment" + " " + DerivedLeftCommentNode.Id + " " +
                    ArticleCommentsTree.GetInstance().Users.Find(x => x.Id == IdToFind).Name + " " + DerivedLeftCommentNode.ComText + " ");
                foreach (string comment in DerivedLeftCommentNode.GetAllDerivedComments())
                {
                    DerivedComments.Add(comment);
                }
            }
            if (DerivedRightCommentNode != null)
            {
                int IdToFind = DerivedRightCommentNode.UserId;
                DerivedComments.Add("Comment" + " " + DerivedRightCommentNode.Id + " " +
                    ArticleCommentsTree.GetInstance().Users.Find(x => x.Id == IdToFind).Name + " " + DerivedRightCommentNode.ComText + " ");
                foreach (string comment in DerivedRightCommentNode.GetAllDerivedComments())
                {
                    DerivedComments.Add(comment);
                }
            }
            return DerivedComments;
        }
        public void LoadFromDBToModel(string ConnectionString)
        {
            if (ConnectionString is null)
            {
                throw new ArgumentNullException(nameof(ConnectionString));
            }
            var Interactor = new DBInteraction(ConnectionString);
            List<Comment> tmp = Interactor.FindCommentsByParentId(Id);
            int count = tmp.Count;
            if (count == 0)
            {
                return;
            }
            if (count == 1)
            {
                DerivedLeftCommentNode = new CommentNode(tmp[0]);
                DerivedRightCommentNode = null;
                DerivedLeftCommentNode.LoadFromDBToModel(ConnectionString);
            }
            if (count == 2)
            {
                DerivedLeftCommentNode = new CommentNode(tmp[0]);
                DerivedRightCommentNode = new CommentNode(tmp[1]);
                DerivedLeftCommentNode.LoadFromDBToModel(ConnectionString);
                DerivedRightCommentNode.LoadFromDBToModel(ConnectionString);
            }
        }
        public bool RecInsertByParentId(ref CommentNode arg)
        //True добавлен, false не добавлен.
        {
            if (arg == null) throw new ArgumentNullException(paramName: nameof(arg));
            if (this == null) return false;
            if (DerivedLeftCommentNode == null)
            {
                if (arg.Parent == Id)
                {
                    SetDerivedLeftCommentNode(ref arg);
                    return true;
                }
            }
            else
            {
                if (DerivedLeftCommentNode.RecInsertByParentId(ref arg)) return true;
            }
            if (DerivedRightCommentNode == null)
            {
                if (arg.Parent == Id)
                {
                    SetDerivedRightCommentNode(ref arg);
                    return true;
                }
            }
            else
            {
                if (RecInsertByParentId(ref arg)) return true;
            }
            return false;
        }
    }
}