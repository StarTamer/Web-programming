using httpserver.attributes;

namespace httpserver.controllers
{
    [HttpController("products")]
    public class Products
    {
        [HttpGET("list")]
        public string getProducts(int id)
        {
            return "Good response";
        }

        [HttpGET("list3")]
        public string getProductByID(int id)
        {
            return "Good response";
        }

        [HttpGET("list")]
        public void getProducts(string id)
        {
            //return "Good response";
        }
    }
}
