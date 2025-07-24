public class Category
{
    private string categoryCode;
    private string categoryName;

    public Category(string code, string name)
    {
        categoryCode = code;
        categoryName = name;
    }

    public string getCode()
    {
        return categoryCode;
    }

    public string getName()
    {
        return categoryName;
    }
}