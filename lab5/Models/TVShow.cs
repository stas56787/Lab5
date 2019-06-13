using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lab5.Models
{
    public class TVShow
    {
        [Key]
        public int TVShowID { get; set; }
        [Display(Name = "Название шоу")]
        public string NameShow { get; set; }
        [Display(Name = "Длительность шоу")]
        public string Duration { get; set; }
        [Display(Name = "Рейтинг")]
        public string Rating { get; set; }
        [Display(Name = "Описание")]
        public string DescriptionShow { get; set; }
        [Display(Name = "GenreID")]
        public int? GenreID { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual ICollection<ScheduleForWeek> ScheduleForWeeks { get; set; }

        public TVShow() { }

        public TVShow(int TVShowID, string NameShow, string Duration, string Rating, string DescriptionShow,
            int? GenreID)
        {
            this.TVShowID = TVShowID;
            this.NameShow = NameShow;
            this.Duration = Duration;
            this.Rating = Rating;
            this.DescriptionShow = DescriptionShow;
            this.GenreID = GenreID;
        }

        public override bool Equals(object obj)
        {
            var item = obj as TVShow;

            if (obj == null)
            {
                return false;
            }
            if (obj == this)
            {
                return true;
            }

            return this.TVShowID == item.TVShowID;
        }

        public override int GetHashCode()
        {
            return this.TVShowID.GetHashCode();
        }
    }
}
