using System;
using System.Collections.Generic;

namespace KVA.Cinema.Models.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public DateTime PublishedOn { get; set; }

        //public Guid? ParentCommentId { get; set; }

        public Guid VideoId { get; set; }

        public Guid UserId { get; set; }

        //public virtual ICollection<Comment> Comment1 { get; set; } дочерние комменты

        //public virtual Comment ParentComment { get; set; }

        public virtual User User { get; set; }

        public virtual Video Video { get; set; }

        public virtual ICollection<CommentMark> CommentMarks { get; set; }

        public Comment()
        {
            //Comment1 = new HashSet<Comment>();
            CommentMarks = new HashSet<CommentMark>();
        }
    }
}
