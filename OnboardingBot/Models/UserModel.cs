using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingBot.Models
{
    public partial class User
    {
        public int Id { get; set; }

        public int TelegramId { get; set; }

        public int DirectionId { get; set; }
    }
}
