namespace LogicLayer.Pruners
{
    public abstract class Pruner
    {
        protected readonly string HtmlString;

        protected Pruner(string htmlString)
        {
            HtmlString = htmlString;
        }

        public abstract string Prune();
    }
}