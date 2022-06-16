using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.ValueObjects
{
    public class CommentValue : ValueObject
    {
        public string Comment { get; private set; }

        private CommentValue(string comment)
        {
            this.Comment = comment;
        }

        public static CommentValue Create(string comment)
        {
            CheckRule(new StringRules(comment, nameof(Comment)));
            return new CommentValue(comment);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Comment;
        }
    }
}