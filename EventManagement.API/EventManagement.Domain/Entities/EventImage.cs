using System;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.Common;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Domain.Entities
{
    public class EventImage : Entity
    {
        public int EventId { get; private set; }
        public string ImagePath { get; private set; }
        public string ImageTitle { get; private set; }
        public bool IsMain { get; private set; }
        public ImageDescription Description { get; private set; }


        private EventImage(int eventId, string path, string title, bool isMain, ImageDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (title == null)
            {
                throw new ArgumentNullException(nameof(description));
            }


            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            (this.EventId, this.ImagePath, this.ImageTitle, this.IsMain, this.Description) =
                (eventId, path, title, isMain, description);
        }

        private EventImage(int id, int eventId, string path, string title, bool isMain, ImageDescription description) :
            this(eventId, path, title, isMain, description)
        {
            this.Id = id;
        }

        public static EventImage LoadImage(int id, int eventId, string path, string title, bool isMain,
            ImageDescription description)
        {
            return new EventImage(id, eventId, path, title, isMain, description);
        }
        public static EventImage Create(int eventId, string path, string title, bool isMain,
            ImageDescription description)
        {
            CheckRule(new StringRules(path, "Path"));

            CheckRule(new StringRules(title, "Title", title.Length is >= 5 and <= 50,
                customError: "The file title length should be between 5 and 50."));

            return new EventImage(eventId, path, title, isMain, description);
        }

        public void ChangeStatusMain()
        {
            this.IsMain = !this.IsMain;
        }
    }
}