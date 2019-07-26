namespace Breakfast.Api.Models
{
    public class CreateOrUpdateBreakfastItemRequest
    {
        public string Name { get; set; }

        public int? Rating { get; set; }
    }
}
