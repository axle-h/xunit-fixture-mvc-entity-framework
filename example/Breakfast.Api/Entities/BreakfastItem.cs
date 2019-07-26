using System;

namespace Breakfast.Api.Entities
{
    public class BreakfastItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? Rating { get; set; }
    }
}
