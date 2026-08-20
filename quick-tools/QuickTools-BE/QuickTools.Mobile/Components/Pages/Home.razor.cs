namespace QuickTools.Mobile.Components.Pages
{
    public partial class Home
    {
        protected string Keyword { get; set; } = "";

        protected List<Product> Products { get; set; } = [];

        protected void Search()
        {
            // Xử lý search
            Products =
            [
                new Product { Name = "iPhone" },
                new Product { Name = "MacBook" }
            ];
        }

        protected class Product
        {
            public string Name { get; set; } = "";
        }
    }
}